using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Entities;

public class TaskEntity
{
    [Display("Task ID")] public string Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    [Display("Task type")] public string? TaskType { get; set; }
}