using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Entities;

public class StageEntity
{
    [Display("Stage ID")] public string Id { get; set; }

    [Display("Created at")] public DateTime CreatedAt { get; set; }
    
    [Display("Workflow ID")] public string WorkflowId { get; set; }
    
    [Display("Workflow stage ID")] public string WorkflowStageId { get; set; }
}