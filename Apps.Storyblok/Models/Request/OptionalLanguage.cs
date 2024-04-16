using Apps.Storyblok.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Storyblok.Models.Request;

public class OptionalLanguage
{
    [Display("Language")]
    [StaticDataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }
}