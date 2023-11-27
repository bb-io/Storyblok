namespace Apps.Storyblok.Models.Request.Story;

public class CreateStoryRequest
{
    public StoryPayload Story { get; set; }

    public int? Publish { get; set; }

    public string? ReleaseId { get; set; }

    public CreateStoryRequest(CreateStoryInput input)
    {
        Story = new()
        {
            Name = input.Name,
            Slug = input.Slug,
            DefaultRoot = input.DefaultRoot,
            IsFolder = input.IsFolder,
            ParentId = input.ParentId,
            DisableFeEditor = input.DisableFeEditor,
            IsStartPage = input.IsStartPage,
            Position = input.Position,
            Path = input.Path,
        };
        Publish = input.Publish is true ? 1 : default;
        ReleaseId = input.ReleaseId;
    }
}