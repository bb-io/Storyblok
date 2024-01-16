using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Entities;

public class StoryEntity
{
    [Display("Name")] public string Name { get; set; }
    
    [Display("Alternates")] public IEnumerable<AlternateEntity>? Alternates { get; set; }

    [Display("Parent ID")] public string ParentId { get; set; }

    [Display("Created at")] public DateTime CreatedAt { get; set; }

    [Display("Deleted at")] public DateTime? DeletedAt { get; set; }

    [Display("Group ID")] public string GroupId { get; set; }

    [Display("Sort by date")] public DateTime? SortByDate { get; set; }

    [Display("Updated at")] public DateTime UpdatedAt { get; set; }

    [Display("Published at")] public DateTime? PublishedAt { get; set; }

    [Display("Story ID")] public string Id { get; set; }

    [Display("UUID")] public string Uuid { get; set; }

    [Display("Is folder")] public bool IsFolder { get; set; }

    [Display("Is published")] public bool Published { get; set; }

    [Display("Slug")] public string Slug { get; set; }

    [Display("Path")] public string Path { get; set; }

    [Display("Full slug")] public string FullSlug { get; set; }

    [Display("Root folder")] public string RootFolder => FullSlug.Split("/").First();

    [Display("Position")] public int Position { get; set; }

    [Display("Unpublished changes")] public bool UnpublishedChanges { get; set; }

    [Display("Is startpage")] public bool IsStartpage { get; set; }

    [Display("Pinned")] public bool Pinned { get; set; }

    [Display("Publish at")] public DateTime? PublishAt { get; set; }

    [Display("Expire at")] public DateTime? ExpireAt { get; set; }

    [Display("First published at")] public DateTime? FirstPublishedAt { get; set; }

    [Display("Release IDs")] public IEnumerable<string> ReleaseIds { get; set; }

    [Display("Stage")] public StageEntity? Stage { get; set; }

    [Display("Default Root")] public string DefaultRoot { get; set; }

    [Display("Disable FE editor")] public bool DisableFeEditor { get; set; }

    [Display("Last Author")] public UserEntity LastAuthor { get; set; }

    [Display("Content type")] public string ContentType { get; set; }

    [Display("Tag list")] public IEnumerable<string> TagList { get; set; }

    [Display("Cannot view")] public bool CannotView { get; set; }
}