using System.Text;
using System.Web;
using Apps.Storyblok.ContentConverters.Constants;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Apps.Storyblok.ContentConverters;

public static class StoryblokToJsonConverter
{
    public static string ToJson(string html, string pageId)
    {
        var htmlDoc = html.AsHtmlDocument();
        var translatableNodes = htmlDoc.DocumentNode.SelectSingleNode("/html/body")
            .ChildNodes
            .Where(x => x.Name != "#text")
            .ToArray();

        var dictionaryData = translatableNodes.Select(MapComponentToHtmlTag)
            .ToDictionary(x => x.Key, x => x.Value);

        dictionaryData[ConverterConstants.PageIdNode] = pageId;
        return JsonConvert.SerializeObject(dictionaryData);
    }

    private static KeyValuePair<string, string> MapComponentToHtmlTag(HtmlNode node)
    {
        if (node == null)
            throw new PluginMisconfigurationException("HtmlNode is null. Please check the input and ensure it is configured correctly.");

        if (!node.Attributes.Contains(ConverterConstants.IdAttr))
            throw new PluginMisconfigurationException($"The required attribute '{ConverterConstants.IdAttr}' is missing in the node of file. Please verify the input configuration.");

        var componentId = node.Attributes[ConverterConstants.IdAttr].Value;
        var styleValue = node.Attributes[ConverterConstants.StyleValueAttr]?.Value;

        if (node.Attributes[ConverterConstants.OriginalTableAttr] is not null)
            return new(componentId, ConvertTableToJson(node));

        if (node.Attributes[ConverterConstants.OriginalRichTextAttr] is not null)
            return new(componentId, ConvertRichTextToJson(node));

        if (componentId.EndsWith(ConverterConstants.MarkdownTableId))
            return new(componentId, ConvertMarkdownToJson(node));

        if (styleValue != null)
            return new(componentId, styleValue);

        var htmlContent = HttpUtility.HtmlDecode(node.InnerHtml);
        return new(componentId, htmlContent);
    }

    private static string ConvertTableToJson(HtmlNode node)
    {
        var originalAttr = node.Attributes[ConverterConstants.OriginalTableAttr]!.Value;

        var table = JObject.Parse(HttpUtility.HtmlDecode(originalAttr));
        var tableChildren = node.Descendants()
            .Where(x => x.Attributes.Contains(ConverterConstants.ComponentPath))
            .ToList();

        tableChildren.ForEach(x =>
        {
            var path = x.Attributes[ConverterConstants.ComponentPath].Value;
            var token = table.SelectToken(path)!;

            ((JValue)token).Value = HttpUtility.HtmlDecode(x.InnerHtml);
        });

        return table.ToString();
    }

    private static string ConvertRichTextToJson(HtmlNode node)
    {
        var originalRtAttr = node.Attributes[ConverterConstants.OriginalRichTextAttr]!.Value;

        var richText = JObject.Parse(HttpUtility.HtmlDecode(originalRtAttr));
        var richTextChildren = node.Descendants()
            .Where(x => x.Attributes.Contains(ConverterConstants.ComponentPath))
            .ToList();

        foreach (var x in richTextChildren)
        {
            var path = x.Attributes[ConverterConstants.ComponentPath].Value;

            if (string.IsNullOrWhiteSpace(path) || !path.StartsWith("$"))
            {
                continue;
            }

            var hrefMatch = Regex.Match(path, @"content\[\d+\]\.content\[\d+\]\.marks\[\d+\]\.attrs\.href$");
            if (hrefMatch.Success)
            {
                var markPath = path.Substring(0, path.LastIndexOf(".attrs.href"));
                var markToken = richText.SelectToken(markPath);
                if (markToken == null || markToken["type"]?.ToString() != "link" || markToken["attrs"]?["href"] == null)
                {
                    continue;
                }
            }

            var token = richText.SelectToken(path);
            if (token == null)
            {
                continue;
            }

            ((JValue)token).Value = HttpUtility.HtmlDecode(x.InnerHtml);
        }

        return richText.ToString();
    }

    private static string ConvertMarkdownToJson(HtmlNode node)
    {
        var tableElements = node.ChildNodes.Where(x => x.Name != "#text").ToList();

        var markdown = new StringBuilder();
        tableElements.ForEach(x =>
        {
            var leadingSpaces = int.Parse(x.Attributes[ConverterConstants.LeadingSpacesAttr].Value);
            var trailingSpaces = int.Parse(x.Attributes[ConverterConstants.TrailingSpacesAttr].Value);

            markdown.Append($"|{new(' ', leadingSpaces)}{x.InnerHtml}{new(' ', trailingSpaces)}");
        });

        return markdown + "|";
    }
}