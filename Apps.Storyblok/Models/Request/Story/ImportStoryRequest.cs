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
    [Display("Translated content", Description = "HTML or XLIFF file containing translated content to import")]
    public FileReference Content { get; set; }

    [Display("Locale", Description = "Language code for field-level localization (e.g., 'en', 'de'). Cannot be used together with 'Full slug'")]
    [StaticDataSource(typeof(LanguageDataHandler))]
    public string? Locale { get; set; }

    [Display("Use dimension localization strategy", Description = "Enable dimension-based localization. Dimensions create separate stories for each locale linked through alternates")]
    public bool? UseDimensionLocalizationStrategy { get; set; }
    
    [Display("Full slug", Description = "Full path for the dimension story (e.g., 'en/my-page'). Use with dimension localization strategy. Cannot be used together with 'Locale'")]
    public string? FullSlug { get; set; }

    [Display("Create dimension if not exists", Description = "Automatically create a new dimension story if it doesn't exist. Only applicable when using dimension localization strategy")]
    public bool? CreateDimensionIfNotExists { get; set; }

    [Display("Story ID")]
    [DataSource(typeof(StoryDataHandler))]
    public string ContentId { get; set; }

    [Display("Space ID", Description = "Optional. If not provided, will be read from the HTML file metadata")]
    [DataSource(typeof(SpaceDataHandler))]
    public string? SpaceId { get; set; }
}
