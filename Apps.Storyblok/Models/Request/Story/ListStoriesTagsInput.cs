using Apps.Storyblok.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Storyblok.Models.Request.Story
{
    public class ListStoriesTagsInput
    {
        [Display("Tags")]
        [DataSource(typeof(TagsDataHandler))]
        public IEnumerable<string>? Tags { get; set; }
    }
}
