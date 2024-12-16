using Apps.Storyblok.Api;
using Apps.Storyblok.ContentConverters;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.Story;
using Apps.Storyblok.Models.Request;
using Apps.Storyblok.Models.Response.Story;
using Apps.Storyblok.Models.Response;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Net.Mime;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Newtonsoft.Json.Linq;
using Apps.Storyblok.Models.Response.Component;
using Blackbird.Applications.Sdk.Common.Exceptions;
using HtmlAgilityPack;
using ProseMirror.Serializer.Html;
using ProseMirror.Model;
using Markdig;
using Apps.Storyblok.ContentConverters.Constants;
using System.Text;

namespace Apps.Storyblok.Actions;
[ActionList]
public class FieldLocalizationActions : StoryblokInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    private readonly List<string> LocalizableContentTypes = ["text", "textarea", "richtext", "markdown", "asset"];

    public FieldLocalizationActions(InvocationContext invocationContext, 
        IFileManagementClient fileManagementClient) : base(invocationContext)
    {
        _fileManagementClient = fileManagementClient;
    }

    [Action("Get story as HTML for translation",
        Description = "Get story as HTML for translation (field level localization)")]
    public async Task<FileResponse> GetStoryAsHtml(
        [ActionParameter] StoryRequest story,
        [ActionParameter] OptionalLanguage language)
    {
        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        var html = await AssemblyStoryHtml(response.Story, story.SpaceId);
        using var stream = new MemoryStream(html);
        var file = await _fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{story.StoryId}.html");
        return new() { File = file };
    }

    [Action("Translate story from HTML file", Description = "Translate story from HTML file (field level localization)")]
    public async Task<StoryEntity> TranslateStoryWithHtml(
        [ActionParameter] StoryRequest story,
        [ActionParameter] ImportRequest import)
    {
        var fileStream = await _fileManagementClient.DownloadAsync(import.Content);
        var fileBytes = await fileStream.GetByteData();

        var json = StoryblokToJsonConverter.ToJson(fileBytes, story.StoryId);

        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}/import.json";
        var request = new StoryblokRequest(endpoint, Method.Put, Creds)
            .AddJsonBody(new { data = json });

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
    }

    private async Task<byte[]> AssemblyStoryHtml(StoryEntity storyEntity, string spaceId)
    {
        var content = JObject.Parse(storyEntity.Content.ToString());
        var result = await ProcessNestedBlocksRecursively(content, spaceId);

        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);
        htmlNode.AppendChild(htmlDoc.CreateElement(HtmlConstants.Head));

        var bodyNode = htmlDoc.CreateElement(HtmlConstants.Body);
        htmlNode.AppendChild(bodyNode);
        bodyNode.AppendChild(result);
        return Encoding.UTF8.GetBytes(htmlDoc.DocumentNode.OuterHtml);
    }

    private async Task<HtmlNode> ProcessNestedBlocksRecursively(JObject content, string spaceId)
    {
        var resultNode = HtmlNode.CreateNode("<div></div>");
        var componentName = content["component"].ToString();
        var component = await GetComponent(spaceId, componentName);
        var componentSchema = JObject.Parse(component.Schema.ToString()!);

        var allContentElements = componentSchema.Values().Where(x => x.Type == JTokenType.Object).Select(x => x.Value<JObject>()!).ToList();
        foreach (var contentElement in allContentElements)
        {
            var contentElementType = contentElement["type"].ToString();
            var componentKey = contentElement.Parent.ToObject<JProperty>().Name;

            if (contentElementType == "bloks")
            {
                var nestedBlock = content[componentKey].Values<JObject>();
                if (nestedBlock.Count() == 0)
                    continue;

                foreach(var blockElement in nestedBlock)
                {
                    var nestedBlockContent = await ProcessNestedBlocksRecursively(blockElement, spaceId);
                    if (!string.IsNullOrWhiteSpace(nestedBlockContent.InnerHtml))
                        resultNode.AppendChild(nestedBlockContent);
                }
            }
            else if (LocalizableContentTypes.Contains(contentElementType) && content[componentKey] != null)
            {
                HtmlNode localizableContentHtml = null;
                switch (contentElementType)
                {
                    case "text":
                    case "textarea":
                        localizableContentHtml = HtmlNode.CreateNode("<p></p>");
                        var textContent = content[componentKey].ToString();
                        if (string.IsNullOrWhiteSpace(textContent)) continue;
                        localizableContentHtml.InnerHtml = textContent;
                        break;
                    case "richtext":
                        localizableContentHtml = HtmlNode.CreateNode("<div></div>");
                        var richText = content[componentKey].ToString();
                        if (string.IsNullOrWhiteSpace(richText)) continue;
                        localizableContentHtml.InnerHtml = ConvertRichTextToHtml(richText);
                        break;
                    case "markdown":
                        localizableContentHtml = HtmlNode.CreateNode("<div></div>");
                        var markdownText = content[componentKey].ToString();
                        if (string.IsNullOrWhiteSpace(markdownText)) continue;
                        localizableContentHtml.InnerHtml = Markdown.ToHtml(markdownText);
                        break;
                    case "asset":
                        var asset = content[componentKey].ToObject<AssetEntity>();
                        if (asset.Id == null) continue;
                        localizableContentHtml = HtmlNode.CreateNode("<img></img>");
                        localizableContentHtml.Attributes.Add("src", asset.Filename);
                        localizableContentHtml.Attributes.Add("alt", asset.Alt);
                        break;
                    default:
                        break;
                }
                if(localizableContentHtml != null)
                    resultNode.AppendChild(localizableContentHtml);
            }  
        }
        return resultNode;
    }

    private async Task<ComponentEntity> GetComponent(
        string spaceId,
        string componentName)
    {
        var endpoint = $"/v1/spaces/{spaceId}/components";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);
        request.AddQueryParameter("search", componentName);

        var response = await Client.ExecuteWithErrorHandling<ListComponentsResponse>(request);
        await Task.Delay(400); // rate limit 3 requests per second

        var components = response.Components.Where(x => x.Name == componentName).ToList();
        if (components.Count == 0)
            throw new PluginMisconfigurationException($"Component with name \"{componentName}\" does not exist. Delete it from story if it does not exist anymore.");
        return components.First();
    }

    private string ConvertRichTextToHtml(string richText)
    {
        try
        {
            richText = richText.Replace("list_item", "listItem");
            var proseMirrorNode = ProseMirror.Serializer.JSon.JSonSerializer.Deserialize<Node>(richText);
            var resultHtml = HtmlSerializer.Serialize(proseMirrorNode);
            resultHtml = resultHtml.Replace("\r\n", "");
            return resultHtml;
        }
        catch (Exception ex)
        {
            throw new PluginApplicationException($"Error during converting rich text to html");
        }
    }
}