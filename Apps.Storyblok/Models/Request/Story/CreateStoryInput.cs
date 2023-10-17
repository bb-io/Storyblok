using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Story;

public class CreateStoryInput
{
    public string Name { get; set; }

    public string Slug { get; set; }

    [Display("Default root")] public string? DefaultRoot { get; set; }

    [Display("Is folder")] public bool? IsFolder { get; set; }

    [Display("Parent ID")] public string? ParentId { get; set; }

    [Display("Disable FE editor")] public bool? DisableFeEditor { get; set; }

    [Display("Is startpage")] public bool? IsStartPage { get; set; }

    public int? Position { get; set; }

    public string? Path { get; set; }

    public bool? Publish { get; set; }

    [Display("Content component")] public string? ContentComponent { get; set; }

    [Display("Release ID")] public string? ReleaseId { get; set; }
}