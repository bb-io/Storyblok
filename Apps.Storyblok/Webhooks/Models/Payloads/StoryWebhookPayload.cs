using Apps.Storyblok.Webhooks.Models.Payloads.Base;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class StoryWebhookPayload : StoryblokWebhookPayload
{
    [Display("Story ID")] public string StoryId { get; set; }
}