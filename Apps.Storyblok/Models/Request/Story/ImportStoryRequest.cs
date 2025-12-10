using Apps.Storyblok.DataSourceHandlers;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Blackbird.Applications.Sdk.Common.Dictionaries;
using Apps.Storyblok.DataSourceHandlers.EnumHandlers;

namespace Apps.Storyblok.Models.Request.Story;

public class ImportStoryRequest : IUploadContentInput
{
    [Display("Translated content")]
    public FileReference Content { get; set; }

    [Display("Locale")]
    [StaticDataSource(typeof(LanguageDataHandler))]
    public string? Locale { get; set; }

    [Display("Use dimension localization strategy")]
    public bool? UseDimensionLocalizationStrategy { get; set; }
    
    [Display("Full slug", Description = "Specify this input instead of the locale if you are using dimensions and want to create a new dimension")]
    public string? FullSlug { get; set; }

    [Display("Story ID")]
    [DataSource(typeof(StoryDataHandler))]
    public string ContentId { get; set; }

    [Display("Space ID")]
    [DataSource(typeof(SpaceDataHandler))]
    public string SpaceId { get; set; }
}
