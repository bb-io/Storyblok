using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Story;

public class StoryRequest
{
    [Display("Story ID")]
    public string StoryId { get; set; }
}