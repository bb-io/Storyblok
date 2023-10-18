using Apps.Storyblok.Webhooks.Models.Payloads.Base;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class PipelineWebhookPayload : StoryblokWebhookPayload
{
    [Display("Branch ID")]
    public string BranchId { get; set; }
}