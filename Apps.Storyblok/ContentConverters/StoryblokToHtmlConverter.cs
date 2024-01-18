using System.Text;
using Apps.Storyblok.ContentConverters.Constants;
using HtmlAgilityPack;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace Apps.Storyblok.ContentConverters;

public static class StoryblokToHtmlConverter
{
    private static readonly IEnumerable<string> SkippableFields = new[] { "language", "url", "text_nodes", "page" };

    private static readonly IEnumerable<string> UntranslatableFields = new[]
        { "text_color", "background_blur", "background_brightness", "image_layout", "author" };

    public static byte[] ToHtml(string json)
    {
        var translatableData = JsonConvert.DeserializeObject<Dictionary<string, string>>(json)!
            .Where(x => !SkippableFields.Contains(x.Key))
            .ToList();

        var (doc, bodyNode) = PrepareEmptyHtmlDocument();

        translatableData.ForEach(x =>
        {
            var node = doc.CreateElement(HtmlConstants.Div);

            node.SetAttributeValue(ConverterConstants.IdAttr, x.Key);
            bodyNode.AppendChild(node);

            if (x.Key.Contains(ConverterConstants.RichTextId))
            {
                ConvertRichTextToHtml(doc, node, x.Value);
                return;
            }

            if (x.Key.EndsWith(ConverterConstants.TableId))
            {
                ConvertTableToHtml(doc, node, x.Value);
                return;
            }

            if (x.Key.EndsWith(ConverterConstants.MarkdownTableId))
            {
                ConvertMarkdownToHtml(doc, node, x.Value);
                return;
            }

            if (UntranslatableFields.Any(y => x.Key.EndsWith(y)))
            {
                node.SetAttributeValue(ConverterConstants.StyleValueAttr, x.Value);
                return;
            }

            node.InnerHtml = x.Value;
        });

        return Encoding.UTF8.GetBytes(doc.DocumentNode.OuterHtml);
    }

    private static void ConvertRichTextToHtml(HtmlDocument doc, HtmlNode richTextNode, string richTextJson)
    {
        richTextNode.SetAttributeValue(ConverterConstants.OriginalRichTextAttr, richTextJson);

        var richText = JObject.Parse(richTextJson);

        var contentNodes = richText["content"]!
            .Children()
            .OfType<JObject>()
            .ToList();

        var linkNodes = richText
            .Descendants()
            .Where(x => (x as JProperty)?.Name == "href" && x.Parent!["linktype"]!.ToString() == "url")
            .ToList();

        var linkNode = doc.CreateElement(HtmlConstants.Div);
        richTextNode.AppendChild(linkNode);

        linkNodes.ForEach(x =>
        {
            var property = (x as JProperty)!;
            var node = doc.CreateElement(HtmlConstants.Div);

            node.SetAttributeValue(ConverterConstants.ComponentPath, property.Path);
            node.InnerHtml = property.First!.Value<string>();

            linkNode.AppendChild(node);
        });

        contentNodes.ForEach(x =>
        {
            var type = x["type"]!.ToString();

            var richTextContentNode = doc.CreateElement(GetRichTextContentTag(type));
            richTextNode.AppendChild(richTextContentNode);

            var translatableNodes = x
                .Descendants()
                .Where(x => (x as JProperty)?.Name is "text")
                .ToList();

            translatableNodes.ForEach(x =>
            {
                var property = (x as JProperty)!;

                var nodeTag = GetRichTextItemTag(type);
                var node = doc.CreateElement(nodeTag);

                node.SetAttributeValue(ConverterConstants.ComponentPath, property.Path);
                node.InnerHtml = property.First!.Value<string>();

                richTextContentNode.AppendChild(node);
            });
        });
    }

    private static void ConvertTableToHtml(HtmlDocument doc, HtmlNode tableNode, string tableJson)
    {
        tableNode.SetAttributeValue(ConverterConstants.OriginalTableAttr, tableJson);

        var table = JObject.Parse(tableJson);

        var valueNodes = table
            .Descendants()
            .Where(x => (x as JProperty)?.Name == "value")
            .ToList();

        valueNodes.ForEach(x =>
        {
            var property = (x as JProperty)!;
            var tableValueNode = doc.CreateElement(HtmlConstants.Div);

            tableValueNode.SetAttributeValue(ConverterConstants.ComponentPath, property.Path);
            tableValueNode.InnerHtml = property.First!.Value<string>();
            tableNode.AppendChild(tableValueNode);
        });
    }

    private static void ConvertMarkdownToHtml(HtmlDocument doc, HtmlNode node, string markdownTable)
    {
        var tableElements = markdownTable.Split("|").SkipLast(1).Skip(1).ToList();
        tableElements.ForEach(x =>
        {
            var tableElementNode = doc.CreateElement(HtmlConstants.Div);

            tableElementNode.SetAttributeValue(ConverterConstants.LeadingSpacesAttr,
                (x.Length - x.TrimStart(' ').Length).ToString());
            tableElementNode.SetAttributeValue(ConverterConstants.TrailingSpacesAttr,
                (x.Length - x.TrimEnd(' ').Length).ToString());

            tableElementNode.InnerHtml = x.Trim(' ');
            node.AppendChild(tableElementNode);
        });
    }

    private static (HtmlDocument document, HtmlNode bodyNode) PrepareEmptyHtmlDocument()
    {
        var htmlDoc = new HtmlDocument();
        var htmlNode = htmlDoc.CreateElement(HtmlConstants.Html);
        htmlDoc.DocumentNode.AppendChild(htmlNode);
        htmlNode.AppendChild(htmlDoc.CreateElement(HtmlConstants.Head));

        var bodyNode = htmlDoc.CreateElement(HtmlConstants.Body);
        htmlNode.AppendChild(bodyNode);

        return (htmlDoc, bodyNode);
    }

    private static string GetRichTextContentTag(string type)
        => type switch
        {
            "heading" => HtmlConstants.H1,
            "paragraph" => HtmlConstants.Paragraph,
            _ => HtmlConstants.Div
        };

    private static string GetRichTextItemTag(string type)
    {
        if (type.Contains(ConverterConstants.ListId))
            return HtmlConstants.Div;

        return HtmlConstants.Span;
    }
}