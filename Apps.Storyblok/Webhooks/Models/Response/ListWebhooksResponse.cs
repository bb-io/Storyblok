using Apps.Storyblok.Webhooks.Models.Payloads;

namespace Apps.Storyblok.Webhooks.Models.Response;

public class ListWebhooksResponse
{
    public IEnumerable<WebhookEndpointPayload> WebhookEndpoints { get; set; }
}