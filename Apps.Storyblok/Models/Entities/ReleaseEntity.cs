using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Entities;

public class ReleaseEntity
{
    [Display("Release ID")] public string Id { get; set; }

    [Display("UUID")] public string Uuid { get; set; }

    public string Name { get; set; }

    [Display("Released at")] public DateTime? ReleasedAt { get; set; }

    [Display("Created at")] public DateTime CreatedAt { get; set; }

    public bool Released { get; set; }

    public string Timezone { get; set; }

    [Display("Owner ID")] public string? OwnerId { get; set; }
}