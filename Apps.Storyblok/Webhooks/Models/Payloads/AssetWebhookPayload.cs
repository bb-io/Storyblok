using Apps.Storyblok.Webhooks.Models.Payloads.Base;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class AssetWebhookPayload : StoryblokWebhookPayload
{
    [Display("Asset ID")] public string AssetId { get; set; }
}