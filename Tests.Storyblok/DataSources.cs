using Apps.Storyblok.DataSourceHandlers;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Storyblok.Base;

namespace Tests.Storyblok
{
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
            var handler = new TagsDataHandler(InvocationContext, new StoryRequest { SpaceId= "173562", StoryId= "76279249056122" });
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

    }
}
