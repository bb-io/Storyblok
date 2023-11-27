using Apps.Storyblok.DataSourceHandlers.EnumHandlers;
using Blackbird.Applications.Sdk.Common.Dynamic;

namespace Apps.Storyblok.Models.Request.Story;

public class UpdateStoryInput
{
    [DataSource(typeof(LanguageDataHandler))]
    public string Lang { get; set; }
}