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
                StoryId = "677627238"

            };

            var fileName = "677627238.html";
            

            var input2 = new ImportRequest
            {
                Content= new FileReference
                {
                    Name = fileName
                }
            };

            var result =await client.ImportStoryContent(input,input2);
            Assert.IsNotNull(result);
        }
       


        [TestMethod]
        public async Task ExportStoryContent_ValidFile_ReturnsResponse()
        {
            var client = new StoryActions(InvocationContext, FileManager);

            var input = new StoryRequest
            {
                SpaceId = "173562",
                StoryId = "677627238"

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
                SpaceId = "173562",
                StoryId = "677425912"
            };

            var response = await client.GetStory(input);
            var resultJson = JsonConvert.SerializeObject(response, Formatting.Indented);
            Console.WriteLine(resultJson);

            Assert.IsTrue(true);
        }
    }
}
