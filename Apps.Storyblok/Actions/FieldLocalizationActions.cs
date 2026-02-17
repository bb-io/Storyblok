using Apps.Storyblok.Api;
using Apps.Storyblok.ContentConverters.Constants;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.Story;
using Apps.Storyblok.Models.Response;
using Apps.Storyblok.Models.Response.Component;
using Apps.Storyblok.Models.Response.Story;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using Blackbird.Applications.Sdk.Utils.RichTextConverter;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using HtmlAgilityPack;
using Markdig;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Serialization;
using RestSharp;
using ReverseMarkdown;
using System.Net.Mime;
using System.Text;

namespace Apps.Storyblok.Actions;
[ActionList("Field localization")]
public class FieldLocalizationActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) 
    : StoryblokInvocable(invocationContext)
{
    private readonly List<string> LocalizableContentTypes = ["text", "textarea", "richtext", "markdown", "asset"];

    private const string BlackbirdBlockIdAttribute = "blackbird-block-id";
    private const string BlackbirdFieldNameAttribute = "blackbird-field-name";

    [Action("Get story as HTML for translation",
        Description = "Get story as HTML for translation (field level localization)")]
    public async Task<FileResponse> GetStoryAsHtml(
        [ActionParameter] StoryRequest storyRequest,
        [ActionParameter] GetStoryAsHtmlRequest getStoryAsHtmlRequest)
    {
        var endpoint = $"/v1/spaces/{storyRequest.SpaceId}/stories/{storyRequest.ContentId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        var html = await AssemblyStoryHtml(response.Story, storyRequest.SpaceId, getStoryAsHtmlRequest);
        using var stream = new MemoryStream(html);
        var file = await fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{storyRequest.ContentId}.html");
        return new() { Content = file };
    }

    [Action("Translate story from HTML file", Description = "Translate story from HTML file (field level localization)")]
    public async Task<StoryEntity> TranslateStoryWithHtml(
        [ActionParameter] StoryRequest storyRequest,
        [ActionParameter] LanguageRequest languageRequest,
        [ActionParameter] TranslateStoryWithHtmlRequest translateStoryWithHtmlRequest)
    {
        var fileStream = await fileManagementClient.DownloadAsync(translateStoryWithHtmlRequest.File);
        var fileBytes = await fileStream.GetByteData();
        var html = Encoding.UTF8.GetString(fileBytes);

        var endpoint = $"/v1/spaces/{storyRequest.SpaceId}/stories/{storyRequest.ContentId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        
        var publish = translateStoryWithHtmlRequest.PublishImmediately.HasValue && translateStoryWithHtmlRequest.PublishImmediately.Value;
        await TranslateStory(response.Story, storyRequest.SpaceId, languageRequest.Lang, html, publish);

        return response.Story;
    }

    private async Task<byte[]> AssemblyStoryHtml(StoryEntity storyEntity, string spaceId, GetStoryAsHtmlRequest getStoryAsHtmlRequest)
    {
        var content = JObject.Parse(storyEntity.Content.ToString());
        var result = await ProcessNestedBlocksRecursively(content, spaceId, getStoryAsHtmlRequest);

        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);
        htmlNode.AppendChild(htmlDoc.CreateElement(HtmlConstants.Head));

        var bodyNode = htmlDoc.CreateElement(HtmlConstants.Body);
        htmlNode.AppendChild(bodyNode);
        bodyNode.AppendChild(result);
        return Encoding.UTF8.GetBytes(htmlDoc.DocumentNode.OuterHtml);
    }

    private async Task<HtmlNode> ProcessNestedBlocksRecursively(JObject content, string spaceId, GetStoryAsHtmlRequest getStoryAsHtmlRequest)
    {
        var resultNode = HtmlNode.CreateNode("<div></div>");
        resultNode.Attributes.Add(BlackbirdBlockIdAttribute, content["_uid"].ToString());

        var componentName = content["component"].ToString();
        var component = await GetComponent(spaceId, componentName);
        var componentSchema = JObject.Parse(component.Schema.ToString()!);

        var allContentElements = componentSchema.Values().Where(x => x.Type == JTokenType.Object).Select(x => x.Value<JObject>()!).ToList();
        foreach (var contentElement in allContentElements)
        {
            var contentElementType = contentElement["type"].ToString();
            var componentKey = contentElement.Parent.ToObject<JProperty>().Name;

            if (getStoryAsHtmlRequest.ExcludedFields != null && getStoryAsHtmlRequest.ExcludedFields.Contains(componentKey))
                continue;

            var fieldType = contentElement["field_type"]?.ToString();

            if (contentElementType == "bloks")
            {
                var nestedBlock = content[componentKey].Values<JObject>();
                if (nestedBlock.Count() == 0)
                    continue;

                foreach(var blockElement in nestedBlock)
                {
                    var nestedBlockContent = await ProcessNestedBlocksRecursively(blockElement, spaceId, getStoryAsHtmlRequest);
                    if (!string.IsNullOrWhiteSpace(nestedBlockContent.InnerHtml))
                        resultNode.AppendChild(nestedBlockContent);
                }
            }
            else if (contentElementType == "custom" && fieldType == "seo-metatags")
            {
                if (content[componentKey] is JObject seoContent)
                {
                    var title = seoContent["title"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(title))
                    {
                        var titleNode = HtmlNode.CreateNode("<p></p>");
                        titleNode.Attributes.Add(BlackbirdFieldNameAttribute, $"{componentKey}.title");
                        titleNode.InnerHtml = title;
                        resultNode.AppendChild(titleNode);
                    }

                    var desc = seoContent["description"]?.ToString();
                    if (!string.IsNullOrWhiteSpace(desc))
                    {
                        var descNode = HtmlNode.CreateNode("<p></p>");
                        descNode.Attributes.Add(BlackbirdFieldNameAttribute, $"{componentKey}.description");
                        descNode.InnerHtml = desc;
                        resultNode.AppendChild(descNode);
                    }
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
                        localizableContentHtml.InnerHtml = ConvertRichTextToHtml(content[componentKey].ToObject<JObject>());
                        break;
                    case "markdown":
                        localizableContentHtml = HtmlNode.CreateNode("<div></div>");
                        var markdownText = content[componentKey].ToString();
                        if (string.IsNullOrWhiteSpace(markdownText)) continue;
                        localizableContentHtml.InnerHtml = Markdown.ToHtml(markdownText);
                        break;
                    case "asset":
                        localizableContentHtml = HtmlNode.CreateNode("<img></img>");
                        var asset = content[componentKey].ToObject<AssetEntity>();
                        if (asset.Id == null || IgnoreImages(getStoryAsHtmlRequest)) continue;   
                        localizableContentHtml.Attributes.Add("src", asset.Filename);
                        localizableContentHtml.Attributes.Add("alt", asset.Alt);
                        break;
                    default:
                        break;
                }
                if(localizableContentHtml != null)
                {
                    localizableContentHtml.Attributes.Add(BlackbirdFieldNameAttribute, componentKey);
                    resultNode.AppendChild(localizableContentHtml);
                }                    
            }  
        }
        return resultNode;
    }

    private async Task TranslateStory(StoryEntity storyEntity, string spaceId, string targetLanguage, string translatedHtml, bool publishImmediately)
    {
        var htmlBody = translatedHtml.AsHtmlDocument().DocumentNode.SelectSingleNode("/html/body") 
            ?? throw new PluginMisconfigurationException("HTML file does not contain body");
        var content = JObject.Parse(storyEntity.Content.ToString());
        await TranslateStoryContentRecursively(content, htmlBody.ChildNodes.Where(x => x.Name == "div").First(), spaceId, targetLanguage);

        var test = content.ToString();

        var endpoint = $"/v1/spaces/{spaceId}/stories/{storyEntity.ContentId}";
        var request = new StoryblokRequest(endpoint, Method.Put, Creds);
        request.AddStringBody(JsonConvert.SerializeObject(new
        {
            story = new
            {
                content = JsonConvert.DeserializeObject(content.ToString())
            },
            publish = publishImmediately ? 1 : 0
        }), DataFormat.Json);
        var res = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
    }

    private async Task TranslateStoryContentRecursively(JObject content, HtmlNode html, string spaceId, string targetLanguage)
    {
        if (html == null)
            return;

        var componentName = content["component"].ToString();
        var component = await GetComponent(spaceId, componentName);
        var componentSchema = JObject.Parse(component.Schema.ToString()!);

        var allContentElements = componentSchema.Values().Where(x => x.Type == JTokenType.Object).Select(x => x.Value<JObject>()!).ToList();
        foreach (var contentElement in allContentElements)
        {
            var contentElementType = contentElement["type"].ToString();
            var componentKey = contentElement.Parent.ToObject<JProperty>().Name;
            var fieldType = contentElement["field_type"]?.ToString();

            if (contentElementType == "bloks")
            {
                var nestedBlock = content[componentKey].Values<JObject>();
                if (nestedBlock.Count() == 0)
                    continue;

                foreach (var blockElement in nestedBlock)
                {
                    await TranslateStoryContentRecursively(blockElement, html.ChildNodes.FirstOrDefault(x => x.Attributes.Any(x => x.Name == BlackbirdBlockIdAttribute && x.Value == blockElement["_uid"].ToString())), spaceId, targetLanguage);
                }
            }
            else if (contentElementType == "custom" && fieldType == "seo-metatags")
            {
                var translatedFieldName = $"{componentKey}__i18n__{targetLanguage}";
                var titleElement = html.ChildNodes.FirstOrDefault(x => 
                    x.Attributes.Any(a => a.Name == BlackbirdFieldNameAttribute && a.Value == $"{componentKey}.title")
                );
                var descElement = html.ChildNodes.FirstOrDefault(x => 
                    x.Attributes.Any(a => a.Name == BlackbirdFieldNameAttribute && a.Value == $"{componentKey}.description")
                );

                if (titleElement != null || descElement != null)
                {
                    var originalSeo = content[componentKey] as JObject;
                    var newSeo = originalSeo != null ? (JObject)originalSeo.DeepClone() : [];

                    if (titleElement != null) newSeo["title"] = titleElement.InnerHtml;
                    if (descElement != null) newSeo["description"] = descElement.InnerHtml;

                    content[translatedFieldName] = newSeo;
                }
            }
            else if (LocalizableContentTypes.Contains(contentElementType) && content[componentKey] != null)
            {
                var translatedFieldName = $"{componentKey}__i18n__{targetLanguage}";
                var htmlElement = html.ChildNodes.FirstOrDefault(x => x.Attributes.Any(x => x.Name == BlackbirdFieldNameAttribute && x.Value == componentKey));
                if (htmlElement == null) continue;
                switch (contentElementType)
                {
                    case "text":
                    case "textarea":
                        content[translatedFieldName] = htmlElement.InnerHtml;
                        break;
                    case "richtext":
                        content[translatedFieldName] = ConvertHtmlToRichText(htmlElement.InnerHtml);
                        break;
                    case "markdown":
                        var reverseMarkdownConverter = new Converter();
                        content[translatedFieldName] = reverseMarkdownConverter.Convert(htmlElement.InnerHtml);
                        break;
                    case "asset":
                        var asset = content[componentKey].ToObject<AssetEntity>();
                        asset.Filename = htmlElement.Attributes["src"].Value;
                        asset.Alt = htmlElement.Attributes["alt"].Value;
                        content[translatedFieldName] = JObject.FromObject(asset, JsonSerializer.Create(JsonCamelCaseSettings));
                        break;
                    default:
                        break;
                }
            }
        }
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

    private string ConvertRichTextToHtml(JObject richtextObject)
    {
        var converter = new RichTextToHtmlConverter("type", "text", "marks", "type", "attrs");
        return converter.ToHtml(richtextObject);
    }

    private JObject ConvertHtmlToRichText(string html)
    {
        var converter = new HtmlToRichTextConverter("type", "text", "marks", "type", "attrs", "doc");
        return converter.ToRichText(html);
    }

    private bool IgnoreImages(GetStoryAsHtmlRequest getStoryAsHtmlRequest) => 
        (!getStoryAsHtmlRequest.IncludeImages.HasValue || !getStoryAsHtmlRequest.IncludeImages.Value);

    private JsonSerializerSettings JsonCamelCaseSettings => new JsonSerializerSettings
    {
        ContractResolver = new DefaultContractResolver
        {
            NamingStrategy = new CamelCaseNamingStrategy()
        }
    };
}