using Apps.Storyblok.Actions;
using Apps.Storyblok.Models.Request;
using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common.Files;
using Newtonsoft.Json;
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
                SpaceId= "173562",
                //StoryId = "662502925"
                StoryId = "676213989"
            };

            var fileName = "669571308.html";
            

            var input2 = new ImportRequest
            {
                Content= new FileReference
                {
                    Name = fileName
                }
            };

            var result =await client.ImportStoryContent(input,input2);

            Assert.AreEqual("676213989", result.Id, "Story ID should match the input ID");
            Assert.IsTrue(!string.IsNullOrEmpty(result.Name), "Story name should not be empty");
            Assert.IsTrue(result.Published, "Story should be published");
            Assert.IsTrue(result.CreatedAt <= DateTime.UtcNow, "Creation date should not be in the future");
        }

        [TestMethod]
        public async Task ExportStoryContent_ValidFile_ReturnsResponse()
        {
            var client = new StoryActions(InvocationContext, FileManager);

            var input = new StoryRequest
            {
                SpaceId = "173562",
                StoryId = "670488971"
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
                SpaceId = "173562",
            };

            var input2 = new CreateStoryInput
            {
                Name = "Test Story",
                Slug = "test-story",
                Path= "en/novalabel/checkliste-test",
                IsFolder =true,
            };

            var result = await client.CreateStory(input, input2);

            var resultJson = JsonConvert.SerializeObject(result, Formatting.Indented);
            Console.WriteLine(resultJson);
            Assert.IsNotNull(result, "Result should not be null");
        }
    }
}
