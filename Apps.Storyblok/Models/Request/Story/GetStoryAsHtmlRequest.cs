using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Story;
public class GetStoryAsHtmlRequest
{
    [Display("Include images")]
    public bool? IncludeImages { get; set; }
}

