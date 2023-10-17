namespace Apps.Storyblok.Models.Request.Space;

public class SpacePayload
{
    public string Name { get; set; }

    public string? Domain { get; set; }

    public string? StoryPublishedHook { get; set; }

    public string? SearchblokId { get; set; }
}