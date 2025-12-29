using Apps.Storyblok.ContentConverters.Constants;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Text;
using Markdig;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

namespace Apps.Storyblok.ContentConverters;

public static class StoryblokToHtmlConverter
{
    private static readonly IEnumerable<string> SkippableFieldSuffixes = new[] { ":language", ":url", ":text_nodes", ":page", ":table" };
    private static readonly IEnumerable<string> UntranslatableFields = new[]
    {
        "text_color", "background_blur", "background_brightness", "image_layout", "author", "eventLabel", "eventAction", "eventElement"
    };

    private static readonly MarkdownPipeline MarkdownPipeline = new MarkdownPipelineBuilder()
        .UsePipeTables()
        .UseGridTables()
        .UseAdvancedExtensions()
        .Build();

    public static byte[] ToHtml(string json, string spaceId, string storyId)
    {
        var translatableData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!
            .Where(x => !SkippableFieldSuffixes.Any(suffix => x.Key.EndsWith(suffix)))
            .ToList();

        var (doc, bodyNode) = PrepareEmptyHtmlDocument(spaceId, storyId);

        foreach (var x in translatableData)
        {

            if (x.Key.EndsWith(ConverterConstants.MarkdownTableId) || x.Key.EndsWith(":markdownTable"))
            {
                var tableNode = ConvertMarkdownTableToHtml(doc, x.Value);
                tableNode.SetAttributeValue(ConverterConstants.IdAttr, x.Key);
                bodyNode.AppendChild(tableNode);
            }
            else
            {
                var node = doc.CreateElement(HtmlConstants.Div);
                node.SetAttributeValue(ConverterConstants.IdAttr, x.Key);
                bodyNode.AppendChild(node);

                if (x.Key.Contains(ConverterConstants.RichTextId))
                {
                    // Check if the value is a valid JSON object or plain text
                    if (IsValidJson(x.Value))
                    {
                        ConvertRichTextToHtml(doc, node, x.Value);
                    }
                    else
                    {
                        AddPrefixSuffixAttributes(node, x.Value);
                        node.InnerHtml = HtmlDocument.HtmlEncode(x.Value);
                    }
                }
                else if (x.Key.EndsWith(ConverterConstants.TableId))
                {
                    ConvertTableToHtml(doc, node, x.Value);
                }
                else if (x.Key.EndsWith("_editable"))
                {
                    node.InnerHtml = x.Value;
                }
                else if (UntranslatableFields.Any(y => x.Key.EndsWith(y)))
                {
                    node.SetAttributeValue(ConverterConstants.StyleValueAttr, x.Value);
                }
                else
                {
                    AddPrefixSuffixAttributes(node, x.Value);
                    node.InnerHtml = HtmlDocument.HtmlEncode(x.Value);
                }
            }
        }
        return Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml);
    }

    private static void ConvertRichTextToHtml(HtmlDocument doc, HtmlNode richTextNode, string richTextJson)
    {
        if (string.IsNullOrWhiteSpace(richTextJson))
            return;

        richTextNode.SetAttributeValue(ConverterConstants.OriginalRichTextAttr, richTextJson);
        var richText = JObject.Parse(richTextJson);
        var contentNodes = richText["content"]!.Children().OfType<JObject>().ToList();
        var linkNodes = richText.Descendants()
            .Where(x => (x as JProperty)?.Name == "href" && x.Parent!["linktype"]!.ToString() != "story")
            .Cast<JProperty>()
            .ToList();

        var linkContainer = doc.CreateElement(HtmlConstants.Div);
        richTextNode.AppendChild(linkContainer);
        foreach (var link in linkNodes)
        {
            var linkNode = doc.CreateElement(HtmlConstants.Div);
            linkNode.SetAttributeValue(ConverterConstants.ComponentPath, link.Path);
            linkNode.InnerHtml = HtmlDocument.HtmlEncode(link.Value.ToString());
            linkContainer.AppendChild(linkNode);
        }

        foreach (var content in contentNodes)
        {
            var type = content["type"]!.ToString();
            var contentNode = doc.CreateElement(GetRichTextContentTag(type));
            richTextNode.AppendChild(contentNode);
            if (type.Contains("list"))
            {
                ConvertListToHtml(doc, contentNode, content);
            }
            else
            {
                ConvertTranslatableDataToHtml(doc, contentNode, content);
            }
        }
    }

    private static void ConvertTableToHtml(HtmlDocument doc, HtmlNode tableNode, string tableJson)
    {
        if (string.IsNullOrWhiteSpace(tableJson))
            return;

        tableNode.SetAttributeValue(ConverterConstants.OriginalTableAttr, tableJson);
        var table = JObject.Parse(tableJson);
        var valueProps = table.Descendants().Where(x => (x as JProperty)?.Name == "value").Cast<JProperty>();
        foreach (var prop in valueProps)
        {
            var cellNode = doc.CreateElement(HtmlConstants.Div);
            cellNode.SetAttributeValue(ConverterConstants.ComponentPath, prop.Path);
            var cellValue = prop.Value.ToString();
            AddPrefixSuffixAttributes(cellNode, cellValue);
            cellNode.InnerHtml = HtmlDocument.HtmlEncode(cellValue);
            tableNode.AppendChild(cellNode);
        }
    }

    private static HtmlNode ConvertMarkdownToHtml(HtmlDocument doc, string markdown)
    {
        if (string.IsNullOrWhiteSpace(markdown))
        {
            return doc.CreateElement("table");
        }

        var lines = markdown.Split(new[] { '\n' }, StringSplitOptions.None)
                           .Select(line => line.Trim())
                           .Where(line => !string.IsNullOrWhiteSpace(line))
                           .ToList();
        bool isTable = lines.Any(line => line.StartsWith("|") && line.Contains("|") && !line.All(c => c == '-' || c == '|' || char.IsWhiteSpace(c)));

        if (isTable)
        {
            return ConvertMarkdownTableToHtml(doc, markdown);
        }
        else
        {
            var tableNode = doc.CreateElement("table");
            var htmlFragment = Markdown.ToHtml(markdown, MarkdownPipeline);
            tableNode.InnerHtml = htmlFragment;
            return tableNode;
        }
    }

    private static HtmlNode ConvertMarkdownTableToHtml(HtmlDocument doc, string markdown)
    {
        var tableNode = doc.CreateElement("table");

        var lines = markdown.Split(new[] { '\n' }, StringSplitOptions.None)
                           .Select(line => line.Trim())
                           .Where(line => !string.IsNullOrWhiteSpace(line))
                           .ToList();

        if (lines.Count < 2)
        {
            return tableNode;
        }

        var theadNode = doc.CreateElement("thead");
        tableNode.AppendChild(theadNode);
        var headerRow = doc.CreateElement("tr");
        theadNode.AppendChild(headerRow);

        var headers = lines[0].Trim('|').Split('|').Select(h => h.Trim()).ToList();
        foreach (var header in headers)
        {
            var thNode = doc.CreateElement("th");
            var headerHtml = Markdown.ToHtml(header, MarkdownPipeline);
            headerHtml = ProcessCustomMarkdown(headerHtml);
            thNode.InnerHtml = headerHtml;
            headerRow.AppendChild(thNode);
        }

        if (lines.Count < 3)
        {
            return tableNode;
        }

        var tbodyNode = doc.CreateElement("tbody");
        tableNode.AppendChild(tbodyNode);

        for (int i = 2; i < lines.Count; i++)
        {
            if (lines[i].All(c => c == '-' || c == '|' || char.IsWhiteSpace(c)))
            {
                continue;
            }
            var row = doc.CreateElement("tr");
            tbodyNode.AppendChild(row);
            var cells = lines[i].Trim('|').Split('|').Select(c => c.Trim()).ToList();
            foreach (var cell in cells)
            {
                var tdNode = doc.CreateElement("td");
                var cellHtml = Markdown.ToHtml(cell, MarkdownPipeline);
                cellHtml = ProcessCustomMarkdown(cellHtml);
                tdNode.InnerHtml = cellHtml;
                row.AppendChild(tdNode);
            }
        }

        return tableNode;
    }

    private static string ProcessCustomMarkdown(string html)
    {
        var newTabRegex = new Regex(@"\[<a[^>]+href=""([^""]+)""[^>]*>(.*?)</a>\]\s*\{\s*newtab\s*\}", RegexOptions.IgnoreCase);
        html = newTabRegex.Replace(html, match =>
        {
            var href = match.Groups[1].Value;
            var text = match.Groups[2].Value;
            return $"<a href=\"{href}\" target=\"_blank\">{text}</a>";
        });

        html = Regex.Replace(html, @"\{\s*newtab\s*\}", "");
        return html;
    }

    private static void ConvertListToHtml(HtmlDocument doc, HtmlNode listNode, JObject listContent)
    {
        foreach (var item in listContent["content"]!.OfType<JObject>())
        {
            var listItem = doc.CreateElement(HtmlConstants.Div);
            listNode.AppendChild(listItem);
            ConvertTranslatableDataToHtml(doc, listItem, item);
        }
    }

    private static void ConvertTranslatableDataToHtml(HtmlDocument doc, HtmlNode contentNode, JObject contentObj)
    {
        var textProps = contentObj.Descendants().Where(x => (x as JProperty)?.Name == "text").Cast<JProperty>();
        foreach (var prop in textProps)
        {
            var div = doc.CreateElement(HtmlConstants.Div);
            div.SetAttributeValue(ConverterConstants.ComponentPath, prop.Path);
            var textValue = prop.Value.ToString();
            AddPrefixSuffixAttributes(div, textValue);
            div.InnerHtml = HtmlDocument.HtmlEncode(textValue);
            contentNode.AppendChild(div);
        }
    }

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument(string spaceId, string storyId)
    {
        var htmlDoc = new HtmlDocument();
        var htmlRoot = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlRoot);
        
        var head = htmlDoc.CreateElement(HtmlConstants.Head);
        htmlRoot.AppendChild(head);
        
        var spaceIdMeta = htmlDoc.CreateElement("meta");
        spaceIdMeta.SetAttributeValue("name", "storyblok-space-id");
        spaceIdMeta.SetAttributeValue("content", spaceId);
        head.AppendChild(spaceIdMeta);
        
        var storyIdMeta = htmlDoc.CreateElement("meta");
        storyIdMeta.SetAttributeValue("name", "storyblok-story-id");
        storyIdMeta.SetAttributeValue("content", storyId);
        head.AppendChild(storyIdMeta);
        
        var body = htmlDoc.CreateElement(HtmlConstants.Body);
        htmlRoot.AppendChild(body);
        return (htmlDoc, body);
    }

    private static void AddPrefixSuffixAttributes(HtmlNode node, string text)
    {
        if (text == null || text == string.Empty) return;

        if (string.IsNullOrWhiteSpace(text))
        {
            node.SetAttributeValue("data-whitespace-only", "true");
            return;
        }

        // Only preserve whitespace characters (space and NBSP)
        // LLMs don't remove punctuation, only spaces
        var specialChars = new[] { ' ', '\u00A0' };

        var prefix = "";
        var suffix = "";

        int prefixEnd = 0;
        while (prefixEnd < text.Length && specialChars.Contains(text[prefixEnd]))
        {
            prefix += text[prefixEnd];
            prefixEnd++;
        }

        int suffixStart = text.Length - 1;
        while (suffixStart >= prefixEnd && specialChars.Contains(text[suffixStart]))
        {
            suffixStart--;
        }
        
        if (suffixStart < text.Length - 1)
        {
            suffix = text.Substring(suffixStart + 1);
        }

        if (!string.IsNullOrEmpty(prefix))
            node.SetAttributeValue("data-prefix", prefix);

        if (!string.IsNullOrEmpty(suffix))
            node.SetAttributeValue("data-suffix", suffix);
    }

    private static string GetRichTextContentTag(string type) => type switch
    {
        "heading" => HtmlConstants.H1,
        "paragraph" => HtmlConstants.Paragraph,
        _ => HtmlConstants.Div
    };

    private static bool IsValidJson(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            return false;

        text = text.Trim();
        if ((!text.StartsWith("{") || !text.EndsWith("}")) && 
            (!text.StartsWith("[") || !text.EndsWith("]")))
        {
            return false;
        }

        try
        {
            JToken.Parse(text);
            return true;
        }
        catch
        {
            return false;
        }
    }
}