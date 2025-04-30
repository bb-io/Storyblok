using Apps.Storyblok.Actions;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common.Files;
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

            var input = new StoryRequest
            {
                SpaceId= "337047",
                StoryId = "662502925"
            };

            var fileName = "662502925 (2).html";

            var input2 = new ImportRequest
            {
                Content= new FileReference
                {
                    Name = fileName
                }
            };

            var result =await client.ImportStoryContent(input,input2);


            Assert.IsNotNull(result, "Result should not be null");

            Assert.AreEqual("662502925", result.Id, "Story ID should match the input ID");
            Assert.IsTrue(!string.IsNullOrEmpty(result.Name), "Story name should not be empty");
            Assert.IsTrue(result.Published, "Story should be published");
            Assert.IsTrue(result.CreatedAt <= DateTime.UtcNow, "Creation date should not be in the future");
        }
    }
}
