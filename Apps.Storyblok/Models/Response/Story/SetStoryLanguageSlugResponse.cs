using Apps.Storyblok.Models.Entities;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Response.Story
{
    public class SetStoryLanguageSlugResponse
    {
        [Display("Story")]
        public StoryEntity Story { get; set; }

        [Display("Full slug")]
        public string FullSlug { get; set; }
    }
}
