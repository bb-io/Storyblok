using Apps.Storyblok.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Storyblok.Models.Request;

public class OptionalLanguage
{
    [Display("Language")]
    [DataSource(typeof(LanguageDataHandler))]
    public string? Language { get; set; }
}