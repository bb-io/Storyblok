using Apps.Storyblok.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dictionaries;

namespace Apps.Storyblok.Models.Request.Story;

public class UpdateStoryInput
{
    [StaticDataSource(typeof(LanguageDataHandler))]
    public string Lang { get; set; }
}