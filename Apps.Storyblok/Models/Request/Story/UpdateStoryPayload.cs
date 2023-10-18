using Newtonsoft.Json.Linq;

namespace Apps.Storyblok.Models.Request.Story;

public class UpdateStoryPayload
{
    public JObject Content { get; set; }
    
    public string Lang { get; set; }
}