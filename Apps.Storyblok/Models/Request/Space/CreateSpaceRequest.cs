namespace Apps.Storyblok.Models.Request.Space;

public class CreateSpaceRequest
{
    public SpacePayload Space { get; set; }
    
    public CreateSpaceRequest(CreateSpaceInput input)
    {
        Space = new()
        {
            Name = input.Name,
            Domain = input.Domain,
            StoryPublishedHook = input.StoryPublishedHook,
            SearchblokId = input.SearchblokId
        };
    }
}