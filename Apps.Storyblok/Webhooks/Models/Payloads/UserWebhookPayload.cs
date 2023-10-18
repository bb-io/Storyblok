using Apps.Storyblok.Webhooks.Models.Payloads.Base;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class UserWebhookPayload : StoryblokWebhookPayload
{
    [Display("User ID")] public string UserId { get; set; }
}