using System.Text;
using System.Web;
using Apps.Storyblok.ContentConverters.Constants;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Text.RegularExpressions;

namespace Apps.Storyblok.ContentConverters;

public static class StoryblokToJsonConverter
{
    public static string ToJson(string html, string pageId)
    {
        var htmlDoc = html.AsHtmlDocument();
        var bodyNode = htmlDoc.DocumentNode.SelectSingleNode("/html/body");
        if (bodyNode == null)
            throw new PluginMisconfigurationException("HTML document must contain a <body> element.");

        var textNodesDiv = bodyNode.SelectSingleNode("//div[@id='text_nodes']");
        if (textNodesDiv != null)
            textNodesDiv.Remove();

        var translatableNodes = bodyNode
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
                continue;

            var hrefMatch = Regex.Match(path, @"content\[\d+\]\.content\[\d+\]\.marks\[\d+\]\.attrs\.href$");
            if (hrefMatch.Success)
            {
                var markPath = path.Substring(0, path.LastIndexOf(".attrs.href"));
                var markToken = richText.SelectToken(markPath);
                if (markToken == null || markToken["type"]?.ToString() != "link" || markToken["attrs"]?["href"] == null)
                    continue;
            }

            var token = richText.SelectToken(path);
            if (token == null)
                continue;

            ((JValue)token).Value = HttpUtility.HtmlDecode(x.InnerHtml);
        }

        return richText.ToString();
    }

    private static string ConvertMarkdownToJson(HtmlNode node)
    {
        var markdown = new StringBuilder();

        var thead = node.SelectSingleNode("thead");
        var tbody = node.SelectSingleNode("tbody");

        if (thead == null || tbody == null)
            throw new PluginMisconfigurationException("Table must contain both <thead> and <tbody> elements.");

        var headerRow = thead.SelectSingleNode("tr");
        if (headerRow == null)
            throw new PluginMisconfigurationException("Table <thead> must contain a <tr> element.");

        var headers = headerRow.SelectNodes("th")?.Select(th => HttpUtility.HtmlDecode(th.InnerText.Trim())).ToList();
        if (headers == null || !headers.Any())
            throw new PluginMisconfigurationException("Table <thead> must contain at least one <th> element.");

        var rows = tbody.SelectNodes("tr")?.Select(tr =>
            tr.SelectNodes("td")?.Select(td => HttpUtility.HtmlDecode(td.InnerText.Trim())).ToList()
        ).ToList() ?? new List<List<string>>();

        var columnWidths = new int[headers.Count];
        for (int i = 0; i < headers.Count; i++)
        {
            columnWidths[i] = headers[i].Length;
            foreach (var row in rows)
            {
                if (row != null && i < row.Count)
                    columnWidths[i] = Math.Max(columnWidths[i], row[i].Length);
            }
        }

        markdown.Append("|");
        for (int i = 0; i < headers.Count; i++)
        {
            markdown.Append($" {headers[i].PadRight(columnWidths[i])} |");
        }
        markdown.AppendLine();

        markdown.Append("|");
        for (int i = 0; i < headers.Count; i++)
        {
            markdown.Append($" {new string('-', columnWidths[i])} |");
        }
        markdown.AppendLine();

        foreach (var row in rows)
        {
            if (row == null) continue;
            markdown.Append("|");
            for (int i = 0; i < headers.Count; i++)
            {
                var cell = i < row.Count ? row[i] : "";
                markdown.Append($" {cell.PadRight(columnWidths[i])} |");
            }
            markdown.AppendLine();
        }

        return markdown.ToString();
    }
}