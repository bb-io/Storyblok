using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Models.Request;
using Apps.Storyblok.Webhooks.Models.Response;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Storyblok.Webhooks.Handlers.Base;

public abstract class StoryblokWebhookHandler : IWebhookEventHandler
{
    protected abstract string Event { get; }

    private string SpaceId { get; }
    private StoryblokClient Client { get; }

    public StoryblokWebhookHandler([WebhookParameter(true)] SpaceRequest space)
    {
        SpaceId = space.SpaceId;
        Client = new();
    }

    public Task SubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds, Dictionary<string, string> values)
    {
        var endpoint = $"/v1/spaces/{SpaceId}/webhook_endpoints";
        var request = new StoryblokRequest(endpoint, Method.Post, creds)
            .WithJsonBody(new WebhookEndpointRequest()
            {
                WebhookEndpoint = new()
                {
                    Name = $"Blackbird-{Event}-{DateTimeOffset.Now.ToString()}",
                    Endpoint = values["payloadUrl"],
                    Actions = new[] { Event }
                }
            }, JsonConfig.Settings);

        return Client.ExecuteWithErrorHandling(request);
    }

    public async Task UnsubscribeAsync(IEnumerable<AuthenticationCredentialsProvider> creds,
        Dictionary<string, string> values)
    {
        var allWebhooks = await GetAllWebhooks(creds);

        var webhookToDelete = allWebhooks.WebhookEndpoints.FirstOrDefault(x => x.Endpoint == values["payloadUrl"]);

        if (webhookToDelete is null)
            return;

        var endpoint = $"/v1/spaces/{SpaceId}/webhook_endpoints/{webhookToDelete.Id}";
        var request = new StoryblokRequest(endpoint, Method.Delete, creds);

        await Client.ExecuteWithErrorHandling(request);
    }

    private Task<ListWebhooksResponse> GetAllWebhooks(IEnumerable<AuthenticationCredentialsProvider> creds)
    {
        var endpoint = $"/v1/spaces/{SpaceId}/webhook_endpoints";
        var request = new StoryblokRequest(endpoint, Method.Get, creds);

        return Client.ExecuteWithErrorHandling<ListWebhooksResponse>(request);
    }
}