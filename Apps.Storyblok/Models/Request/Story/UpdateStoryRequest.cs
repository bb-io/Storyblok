using System.Text;
using Newtonsoft.Json.Linq;

namespace Apps.Storyblok.Models.Request.Story;

public class UpdateStoryRequest
{
    public UpdateStoryPayload Story { get; set; }

    public UpdateStoryRequest(UpdateStoryInput input)
    {
        var json = Encoding.UTF8.GetString(input.Content.Bytes);
        Story = new()
        {
            Content = JObject.Parse(json),
            Lang = input.Lang
        };
    }
}