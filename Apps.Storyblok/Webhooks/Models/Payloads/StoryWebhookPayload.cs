using Apps.Storyblok.Webhooks.Models.Payloads.Base;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;
using Newtonsoft.Json;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class StoryWebhookPayload : StoryblokWebhookPayload, IDownloadContentInput
{
    [Display("Story ID"), JsonProperty("story_id")] public string ContentId { get; set; }
}