using Apps.Storyblok.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Storyblok.Models.Request.Story;
public class GetStoryAsHtmlRequest
{
    [Display("Include images")]
    public bool? IncludeImages { get; set; }

    [Display("Excluded fields"), DataSource(typeof(StoryFieldsDataHandler))]
    public IEnumerable<string>? ExcludedFields { get; set; }
}

