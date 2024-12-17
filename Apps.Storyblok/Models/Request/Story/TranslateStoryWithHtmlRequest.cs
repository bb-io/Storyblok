using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Storyblok.Models.Request.Story;
public class TranslateStoryWithHtmlRequest
{
    [Display("Translated HTML file")]
    public FileReference File { get; set; }

    [Display("Publish immediately")]
    public bool? PublishImmediately { get; set; }
}

