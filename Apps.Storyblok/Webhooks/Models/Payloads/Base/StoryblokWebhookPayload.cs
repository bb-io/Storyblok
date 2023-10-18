using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Payloads.Base;

public class StoryblokWebhookPayload
{
    public string Text { get; set; }

    [Display("Space ID")] public string SpaceId { get; set; }
}