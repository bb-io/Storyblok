using Apps.Storyblok.Webhooks.Models.Payloads.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class StoryWebhookPayload : StoryblokWebhookPayload, IDownloadContentInput
{
    [Display("Story ID")] public string ContentId { get; set; }
}