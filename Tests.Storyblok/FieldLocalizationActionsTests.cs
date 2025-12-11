using Apps.Storyblok.Actions;
using Apps.Storyblok.Models.Request.Story;
using Storyblok.Base;

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
            SpaceId = "173562",
            ContentId = "662894163"
        };

        var languageRequest = new GetStoryAsHtmlRequest
        {
        };

        var result = await actions.GetStoryAsHtml(storyRequest, languageRequest);
        Assert.IsNotNull(result, "Result should not be null");
        Assert.IsNotNull(result.Content, "File should not be null");
    }
    
}