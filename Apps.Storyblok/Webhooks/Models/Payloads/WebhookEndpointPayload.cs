namespace Apps.Storyblok.Webhooks.Models.Payloads;

public class WebhookEndpointPayload
{
    public string Id { get; set; }
    
    public string Name { get; set; }

    public string Endpoint { get; set; }

    public string[] Actions { get; set; }
}