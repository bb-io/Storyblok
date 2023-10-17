namespace Apps.Storyblok.Models.Request.Story;

public class StoryPayload
{
    public string Name { get; set; }

    public string Slug { get; set; }

    public string? DefaultRoot { get; set; }

    public bool? IsFolder { get; set; }

    public string? ParentId { get; set; }

    public bool? DisableFeEditor { get; set; }

    public bool? IsStartPage { get; set; }

    public int? Position { get; set; }

    public string? Path { get; set; }
    
    public ContentPayload Content { get; set; }
}