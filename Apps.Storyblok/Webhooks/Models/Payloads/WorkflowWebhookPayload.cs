using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class WorkflowWebhookPayload : StoryWebhookPayload
{
    [Display("Workflow name")] public string WorkflowName { get; set; }

    [Display("Workflow stage name")] public string WorkflowStageName { get; set; }
}