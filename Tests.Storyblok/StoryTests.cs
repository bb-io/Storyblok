using Apps.Storyblok.Actions;
using Apps.Storyblok.Models.Request;
using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
using Storyblok.Base;

namespace Tests.Storyblok;

[TestClass]
public class StoryTests : TestBase
{
    [TestMethod]
    public async Task ImportStoryContent_ValidFile_ReturnsResponse()
    {
        // Arrange
        var client = new StoryActions(InvocationContext, FileManager);
        var input = new ImportStoryRequest
        {
            //SpaceId = "286695292049554",
            ContentId = "136024915452056",
            Content = new FileReference { Name = "test1.xlf" },
            Locale = "fr"
        };

        // Act
        var result = await client.ImportStoryContent(input);

        // Assert
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task ExportStoryContent_ValidFile_ReturnsResponse()
    {
        var client = new StoryActions(InvocationContext, FileManager);

        var input = new StoryRequest
        {
            SpaceId = "173562",
            ContentId = "677627238"

        };

        var input2 = new OptionalLanguage
        {
           
        };

        var result = await client.ExportStoryContent(input, input2);


        Assert.IsNotNull(result, "Result should not be null");       
    }

    [TestMethod]
    public async Task CreateStory_ValidFile_ReturnsResponse()
    {
        var client = new StoryActions(InvocationContext, FileManager);

        var input = new SpaceRequest
        {
            SpaceId = "340298",
        };

        var input2 = new CreateStoryInput
        {
        };

        var result = await client.CreateStory(input, input2);

        var resultJson = JsonConvert.SerializeObject(result, Formatting.Indented);
        Console.WriteLine(resultJson);
        Assert.IsNotNull(result, "Result should not be null");
    }


    //[TestMethod]
    //public async Task DeleteStoryContent_IsSuccess()
    //{
    //    var client = new StoryActions(InvocationContext, FileManager);

    //    var input = new StoryRequest
    //    {
    //        //SpaceId = "173562",
    //        //StoryId = "669571308"

    //    };

    //    await client.DeleteStory(input);
    //    Assert.IsTrue(true);
    //}

    [TestMethod]
    public async Task GetStoryContent_ValidFile_ReturnsResponse()
    {
        var client = new StoryActions(InvocationContext, FileManager);

        var input = new StoryRequest
        {
            SpaceId = "286695292049554",
            ContentId = "127280115581092"
        };

        var response = await client.GetStory(input);
        var resultJson = JsonConvert.SerializeObject(response, Formatting.Indented);
        Console.WriteLine(resultJson);

        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task AddStoryTags_ValidFile_ReturnsResponse()
    {
        var client = new StoryActions(InvocationContext, FileManager);

        var input = new StoryRequest
        {
            SpaceId = "173562",
            //StoryId = "76279249056122"
            ContentId = "90202101418479"
        };

        var response = await client.AddTagsToStory(input, new AddTagsToStoryRequest { Tags = ["Test KK v2"] });
        var resultJson = JsonConvert.SerializeObject(response, Formatting.Indented);
        Console.WriteLine(resultJson);

        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task RemoveTagFromStory_ReturnsResponse()
    {
        var client = new StoryActions(InvocationContext, FileManager);

        var input = new StoryRequest
        {
            SpaceId = "173562",
            ContentId = "76279249056122"
        };

        var response = await client.RemoveTagFromStory(input, "Test 4");
        var resultJson = JsonConvert.SerializeObject(response, Formatting.Indented);
        Console.WriteLine(resultJson);

        Assert.IsTrue(true);
    }

    [TestMethod]
    public async Task SetStoryLanguageSlug_ReturnsResponse()
    {
        var client = new StoryActions(InvocationContext, FileManager);

        var input = new StoryRequest
        {
            SpaceId = "286695292049554",
            ContentId = "127280115581092"
        };

        var response = await client.SetStoryLanguageSlug(input, new SetStoryLanguageSlugRequest { LanguageCode= "de", TranslatedSlug = "Test_new"});
        var resultJson = JsonConvert.SerializeObject(response, Formatting.Indented);
        Console.WriteLine(resultJson);

        Assert.IsTrue(true);
    }
}
