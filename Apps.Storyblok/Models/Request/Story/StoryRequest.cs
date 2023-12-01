using Apps.Storyblok.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Storyblok.Models.Request.Story;

public class StoryRequest
{
    [Display("Space")]
    [DataSource(typeof(SpaceDataHandler))]
    public string SpaceId { get; set; }

    [Display("Story")]
    [DataSource(typeof(StoryDataHandler))]
    public string StoryId { get; set; }
}