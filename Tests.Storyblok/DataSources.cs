using Apps.Storyblok.DataSourceHandlers;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Storyblok.Base;

namespace Tests.Storyblok;

[TestClass]
public class DataSources : TestBase
{
    [TestMethod]
    public async Task StoryDataHandler_ReturnsValues()
    {
       var handler = new StoryDataHandler(InvocationContext, new StoryRequest
       {
           SpaceId = "173562"
       });
        var context = new DataSourceContext
        {
        };
        var result = await handler.GetDataAsync(context, CancellationToken.None);
        foreach (var item in result)
        {
            Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
        }
        Assert.IsNotNull(result);   
    }

    [TestMethod]
    public async Task SpaceDataHandler_ReturnsValues()
    {
        var handler = new SpaceDataHandler(InvocationContext);
        var context = new DataSourceContext
        {
        };
        var result = await handler.GetDataAsync(context, CancellationToken.None);

        foreach (var item in result)
        {
            Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
        }
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task TagsDataHandler_ReturnsValues()
    {
        var handler = new TagsDataHandler(InvocationContext, new StoryRequest { SpaceId= "173562", ContentId= "76279249056122" });
        var context = new DataSourceContext
        {
        };
        var result = await handler.GetDataAsync(context, CancellationToken.None);

        foreach (var item in result)
        {
            Console.WriteLine($"Key: {item.Key}, Value: {item.Value}");
        }
        Assert.IsNotNull(result);
    }

    [TestMethod]
    public async Task StoryFieldsDataHandler_ReturnsValues()
    {
        // Arrange
        var story = new StoryRequest { ContentId = "126856115522986", SpaceId = "102628" };
        var handler = new StoryFieldsDataHandler(InvocationContext, story);

        // Act
        var result = await handler.GetDataAsync(new DataSourceContext { SearchString = "" }, CancellationToken.None);

        // Assert
        foreach (var item in result)
            Console.WriteLine($"Key: {item.Value}, Value: {item.DisplayName}");
        Assert.IsNotNull(result);
    }
}
