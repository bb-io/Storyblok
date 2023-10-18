using Apps.Storyblok.Webhooks.Models.Payloads.Base;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class ReleaseWebhookPayload : StoryblokWebhookPayload
{
    [Display("Release ID")]
    public string ReleaseId { get; set; }
}