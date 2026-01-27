using Apps.Storyblok.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Storyblok.Models.Request.Story
{
    public class SetStoryLanguageSlugRequest
    {
        [Display("Language code")]
        [StaticDataSource(typeof(LanguageDataHandler))]
        public string? LanguageCode { get; set; }

        [Display("Translated slug")]
        public string TranslatedSlug { get; set; }

        [Display("Translated name")]
        public string? TranslatedName { get; set; }

        [Display("Base slug")]
        public string? BaseSlug { get; set; }
    }
}
