using Newtonsoft.Json.Linq;

namespace Apps.Storyblok.Models.Response.Story;

public class StoryContentPayload
{
    public JObject Content { get; set; }
}