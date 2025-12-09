using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Story
{
    public class AddTagsToStoryRequest
    {
        [Display("Tags", Description = "Tags to add to the story. Duplicates and empty values are ignored.")]
        public IEnumerable<string> Tags { get; set; } = Enumerable.Empty<string>();
    }
}
