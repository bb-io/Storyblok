using Storyblok.Base;
using Newtonsoft.Json;
using Apps.Storyblok.Actions;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common.Files;

namespace Tests.Storyblok;

[TestClass]
public class FieldLocalizationActionsTests : TestBase
{
    [TestMethod]
    public async Task GetStoryAsHtml_ValidStory_ReturnsFileResponse()
    {
        var actions = new FieldLocalizationActions(InvocationContext, FileManager);
        var storyRequest = new StoryRequest
        {
            SpaceId = "102628",
            ContentId = "119062369061925"
        };

        var languageRequest = new GetStoryAsHtmlRequest
        {
        };

        var result = await actions.GetStoryAsHtml(storyRequest, languageRequest);
        Assert.IsNotNull(result, "Result should not be null");
        Assert.IsNotNull(result.Content, "File should not be null");
    }
    
    [TestMethod]
    public async Task TranslateStoryWithHtml_ValidStory_ReturnsResponse()
    {
        // Arrange
        var actions = new FieldLocalizationActions(InvocationContext, FileManager);
        var story = new StoryRequest { SpaceId = "102628", ContentId = "119062369061925" };
        var language = new LanguageRequest { Lang = "en" };
        var request = new TranslateStoryWithHtmlRequest 
        { 
            File = new FileReference { Name = "119062369061925.html" },
            PublishImmediately = false,
        };

        // Act
        var result = await actions.TranslateStoryWithHtml(story, language, request);

        // Assert
        Console.WriteLine(JsonConvert.SerializeObject(result));
        Assert.IsNotNull(result);
    }
}