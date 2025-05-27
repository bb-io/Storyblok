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
            Assert.IsTrue(result.Count > 0, "Result should contain items");
            Assert.IsTrue(result.All(x => x.Value.Contains("test", StringComparison.OrdinalIgnoreCase)),
                "All values should contain the search string");
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

    }
}
