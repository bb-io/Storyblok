using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Task;

public class TaskRequest
{
    [Display("Task ID")]
    public string TaskId { get; set; }
}