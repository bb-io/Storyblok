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
    private const string RichTextComponentPath = "path";
    private const string StyleValueAttr = "style-value";

    private const string RichTextId = "richtext";
    private const string PageIdNode = "page";

    private static readonly IEnumerable<string> SkippableFields = new[] { "language", "url", "text_nodes", "page" };

    private static readonly IEnumerable<string> StyleFields = new[]
        { "text_color", "background_blur", "background_brightness", "image_layout" };

    public static byte[] GetHtml(string json)
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
                ParseRichText(doc, node, x.Value);
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

    public static string GetJson(byte[] html, string pageId)
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
        var originalAttr = node.Attributes[OriginalRichTextAttr]?.Value;

        if (styleValue != null)
            return new(componentId, styleValue);

        if (originalAttr == null)
            return new(componentId, node.InnerText);

        var richText = JObject.Parse(HttpUtility.HtmlDecode(originalAttr));
        var richTextChildren = node.ChildNodes
            .Where(x => x.Name != "#text")
            .ToList();

        richTextChildren.ForEach(x =>
        {
            var path = x.Attributes[RichTextComponentPath].Value;
            var token = richText.SelectToken(path)!;

            ((JValue)token).Value = x.InnerText;
        });

        return new(componentId, richText.ToString());
    }

    private static void ParseRichText(HtmlDocument doc, HtmlNode richTextNode, string richTextJson)
    {
        richTextNode.SetAttributeValue(OriginalRichTextAttr, richTextJson);

        var richText = JObject.Parse(richTextJson);
        var translatableNodes = richText
            .Descendants()
            .Where(x => (x as JProperty)?.Name == "text")
            .ToList();

        translatableNodes.ForEach(x =>
        {
            var property = (x as JProperty)!;
            var node = doc.CreateElement("div");

            node.SetAttributeValue(RichTextComponentPath, property.Path);
            node.InnerHtml = property.First!.Value<string>();

            richTextNode.AppendChild(node);
        });
    }

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