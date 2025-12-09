using Apps.Storyblok.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Storyblok.Models.Request.Story;

public class StoryRequest : IDownloadContentInput
{
    [Display("Space ID")]
    [DataSource(typeof(SpaceDataHandler))]
    public string SpaceId { get; set; }

    [Display("Story ID")]
    [DataSource(typeof(StoryDataHandler))]
    public string ContentId { get; set; }
}