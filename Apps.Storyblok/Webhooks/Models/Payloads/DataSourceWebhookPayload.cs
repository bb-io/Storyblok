using Apps.Storyblok.Webhooks.Models.Payloads.Base;
using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class DataSourceWebhookPayload : StoryblokWebhookPayload
{
    [Display("Datasource slug")] public string DatasourceSlug { get; set; }
}