using Apps.Storyblok.Models.Request;
using Apps.Storyblok.Models.Request.Story;

namespace Apps.Storyblok.Localization;
public interface ILocalizationProvider
{
    Task<byte[]> ExportStoryContent(StoryRequest storyRequest, OptionalLanguage language);
}
