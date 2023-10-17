using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Task;

public class CreateTaskInput
{
    public string Name { get; set; }

    [Display("Webhook URL")] public string WebhookUrl { get; set; }

    public string? Description { get; set; }
}