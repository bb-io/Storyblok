using System.Text;
using System.Web;
using Blackbird.Applications.Sdk.Utils.Html.Extensions;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Storyblok.Utils;

public static class StoryblokHtmlConverter
{
    private const string IdAttr = "id";
    private const string OriginalRichTextAttr = "original";
    private const string OriginalTableAttr = "original-table";
    private const string ComponentPath = "path";
    private const string StyleValueAttr = "style-value";

    private const string RichTextId = "richtext";
    private const string TableId = "table:table";
    private const string PageIdNode = "page";

    private static readonly IEnumerable<string> SkippableFields = new[] { "language", "url", "text_nodes", "page" };

    private static readonly IEnumerable<string> StyleFields = new[]
        { "text_color", "background_blur", "background_brightness", "image_layout", "author" };

    public static byte[] ParseJson(string json)
    {
        var translatableData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!
            .Where(x => !SkippableFields.Contains(x.Key))
            .ToList();

        var (doc, bodyNode) = PrepareEmptyHtmlDocument();

        translatableData.ForEach(x =>
        {
            var node = doc.CreateElement("div");

            node.SetAttributeValue(IdAttr, x.Key);
            bodyNode.AppendChild(node);

            if (x.Key.Contains(RichTextId))
            {
                ParseRichTextToHtml(doc, node, x.Value);
                return;
            }

            if (x.Key.EndsWith(TableId))
            {
                ParseTableToHtml(doc, node, x.Value);
                return;
            }

            if (StyleFields.Any(y => x.Key.EndsWith(y)))
            {
                node.SetAttributeValue(StyleValueAttr, x.Value);
                return;
            }

            node.InnerHtml = x.Value;
        });

        return Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml);
    }

    public static string ParseHtml(byte[] html, string pageId)
    {
        var htmlDoc = Encoding.UTF8.GetString(html).AsHtmlDocument();
        var translatableNodes = htmlDoc.DocumentNode.SelectSingleNode("/html/body")
            .ChildNodes
            .Where(x => x.Name != "#text")
            .ToArray();

        var dictionaryData = translatableNodes.Select(MapComponentToHtmlTag)
            .ToDictionary(x => x.Key, x => x.Value);

        dictionaryData[PageIdNode] = pageId;
        return JsonConvert.SerializeObject(dictionaryData);
    }

    private static KeyValuePair<string, string> MapComponentToHtmlTag(HtmlNode node)
    {
        var componentId = node.Attributes[IdAttr].Value.ToString();
        var styleValue = node.Attributes[StyleValueAttr]?.Value;

        if (node.Attributes[OriginalTableAttr] is not null)
            return ParseTableToJson(node);

        if (node.Attributes[OriginalRichTextAttr] is not null)
            return ParseRichTextToJson(node);

        if (styleValue != null)
            return new(componentId, styleValue);

        return new(componentId, node.InnerText);
    }

    private static KeyValuePair<string, string> ParseTableToJson(HtmlNode node)
    {
        var componentId = node.Attributes[IdAttr]!.Value.ToString();
        var originalAttr = node.Attributes[OriginalTableAttr]!.Value;
        
        var table = JObject.Parse(HttpUtility.HtmlDecode(originalAttr));
        var tableChildren = node.Descendants()
            .Where(x => x.Attributes.Contains(ComponentPath))
            .ToList();
        
        tableChildren.ForEach(x =>
        {
            var path = x.Attributes[ComponentPath].Value;
            var token = table.SelectToken(path)!;

            ((JValue)token).Value = x.InnerText;
        });

        return new(componentId, table.ToString());
    }
    
    private static KeyValuePair<string, string> ParseRichTextToJson(HtmlNode node)
    {
        var componentId = node.Attributes[IdAttr].Value.ToString();
        var originalRtAttr = node.Attributes[OriginalRichTextAttr]!.Value;

        var richText = JObject.Parse(HttpUtility.HtmlDecode(originalRtAttr));
        var richTextChildren = node.Descendants()
            .Where(x => x.Attributes.Contains(ComponentPath))
            .ToList();

        richTextChildren.ForEach(x =>
        {
            var path = x.Attributes[ComponentPath].Value;
            var token = richText.SelectToken(path)!;

            ((JValue)token).Value = x.InnerText;
        });

        return new(componentId, richText.ToString());
    }
    
    private static void ParseRichTextToHtml(HtmlDocument doc, HtmlNode richTextNode, string richTextJson)
    {
        richTextNode.SetAttributeValue(OriginalRichTextAttr, richTextJson);

        var richText = JObject.Parse(richTextJson);

        var contentNodes = richText
            .Descendants()
            .Where(x => (x as JProperty)?.Name == "content")
            .ToList();
        
        var linkNodes = richText
            .Descendants()
            .Where(x => (x as JProperty)?.Name == "href")
            .ToList();

        var linkNode = doc.CreateElement("div");
        richTextNode.AppendChild(linkNode);

        linkNodes.ForEach(x =>
        {
            var property = (x as JProperty)!;
            var node = doc.CreateElement("div");

            node.SetAttributeValue(ComponentPath, property.Path);
            node.InnerHtml = property.First!.Value<string>();

            linkNode.AppendChild(node);
        });

        contentNodes.ForEach(x =>
        {
            var type = x.Parent!["type"]!.ToString();
            if (type == "doc")
                return;

            var richTextContentNode = doc.CreateElement(GetHtmlTag(type));
            richTextNode.AppendChild(richTextContentNode);

            var translatableNodes = x.Parent
                .Descendants()
                .Where(x => (x as JProperty)?.Name is "text")
                .ToList();

            translatableNodes.ForEach(x =>
            {
                var property = (x as JProperty)!;
                var node = doc.CreateElement("span");

                node.SetAttributeValue(ComponentPath, property.Path);
                node.InnerHtml = property.First!.Value<string>();

                richTextContentNode.AppendChild(node);
            });
        });
    }

    private static void ParseTableToHtml(HtmlDocument doc, HtmlNode tableNode, string tableJson)
    {
        tableNode.SetAttributeValue(OriginalTableAttr, tableJson);

        var table = JObject.Parse(tableJson);

        var valueNodes = table
            .Descendants()
            .Where(x => (x as JProperty)?.Name == "value")
            .ToList();

        valueNodes.ForEach(x =>
        {
            var property = (x as JProperty)!;
            var talbeValueNode = doc.CreateElement("div");

            talbeValueNode.SetAttributeValue(ComponentPath, property.Path);
            talbeValueNode.InnerHtml = property.First!.Value<string>();
            tableNode.AppendChild(talbeValueNode);
        });
    }

    private static string GetHtmlTag(string type)
        => type switch
        {
            "heading" => "h1",
            "paragraph" => "p",
            _ => "div"
        };

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument()
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement("html");
        htmlDoc.DocumentNode.AppendChild(htmlNode);
        htmlNode.AppendChild(htmlDoc.CreateElement("head"));

        var bodyNode = htmlDoc.CreateElement("body");
        htmlNode.AppendChild(bodyNode);

        return (htmlDoc, bodyNode);
    }
}