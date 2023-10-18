using Apps.Storyblok.Webhooks.Models.Payloads;

namespace Apps.Storyblok.Webhooks.Models.Request;

public class WebhookEndpointRequest
{
    public WebhookEndpointPayload WebhookEndpoint { get; set; }
}