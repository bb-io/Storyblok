using Apps.Storyblok.Actions;
using Storyblok.Base;

namespace Tests.Storyblok
{
    [TestClass]
    public class StoryTests : TestBase
    {
        [TestMethod]
        public async Task ImportStoryContent_ValidFile_ReturnsResponse()
        {
            var client = new StoryActions(InvocationContext,FileManager);

            var input = 
        }


    }
}
