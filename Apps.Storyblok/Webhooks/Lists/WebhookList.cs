using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Webhooks.Handlers.Impl.Asset;
using Apps.Storyblok.Webhooks.Handlers.Impl.Other;
using Apps.Storyblok.Webhooks.Handlers.Impl.Story;
using Apps.Storyblok.Webhooks.Handlers.Impl.User;
using Apps.Storyblok.Webhooks.Models.Payloads;
using Apps.Storyblok.Webhooks.Models.Request;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Blackbird.Applications.SDK.Blueprints;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Storyblok.Webhooks.Lists;

[WebhookList]
public class WebhookList(InvocationContext invocationContext) : StoryblokInvocable(invocationContext)
{
    [BlueprintEventDefinition(BlueprintEvent.ContentCreatedOrUpdated)]
    [Webhook("On story published", typeof(StoryPublishedWebhookHandler),
        Description = "On a specific story published")]
    public Task<WebhookResponse<StoryWebhookPayload>> OnStoryPublished(WebhookRequest webhookRequest,
        [WebhookParameter] WebhookTagsFilter? filter = null)
        => HandleStoryWebhookWithTagsAny(webhookRequest, filter);

    [Webhook("On story unpublished", typeof(StoryUnpublishedWebhookHandler),
        Description = "On a specific story unpublished")]
    public Task<WebhookResponse<StoryWebhookPayload>> OnStoryUnpublished(WebhookRequest webhookRequest,
        [WebhookParameter] WebhookTagsFilter? filter = null)
        => HandleStoryWebhookWithTagsAny(webhookRequest, filter);

    [Webhook("On story moved", typeof(StoryMovedWebhookHandler),
        Description = "On a specific story moved")]
    public Task<WebhookResponse<StoryWebhookPayload>> OnStoryMoved(WebhookRequest webhookRequest,
        [WebhookParameter] WebhookTagsFilter? filter = null)
        => HandleStoryWebhookWithTagsAny(webhookRequest, filter);

    [Webhook("On story deleted", typeof(StoryDeletedWebhookHandler),
        Description = "On a specific story deleted")]
    public Task<WebhookResponse<StoryWebhookPayload>> OnStoryDeleted(WebhookRequest webhookRequest,
        [WebhookParameter] WebhookTagsFilter? filter = null)
        => HandleStoryWebhookWithTagsAny(webhookRequest, filter);

    [Webhook("On asset created", typeof(AssetCreatedWebhookHandler),
        Description = "On a new asset created")]
    public Task<WebhookResponse<AssetWebhookPayload>> OnAssetCreated(WebhookRequest webhookRequest)
        => HandleWebhook<AssetWebhookPayload>(webhookRequest);

    [Webhook("On asset deleted", typeof(AssetDeletedWebhookHandler),
        Description = "On a specific asset deleted")]
    public Task<WebhookResponse<AssetWebhookPayload>> OnAssetDeleted(WebhookRequest webhookRequest)
        => HandleWebhook<AssetWebhookPayload>(webhookRequest);

    [Webhook("On asset replaced", typeof(AssetReplacedWebhookHandler),
        Description = "On a specific asset replaced")]
    public Task<WebhookResponse<AssetWebhookPayload>> OnAssetReplaced(WebhookRequest webhookRequest)
        => HandleWebhook<AssetWebhookPayload>(webhookRequest);

    [Webhook("On asset restored", typeof(AssetRestoredWebhookHandler),
        Description = "On a specific asset restored")]
    public Task<WebhookResponse<AssetWebhookPayload>> OnAssetRestored(WebhookRequest webhookRequest)
        => HandleWebhook<AssetWebhookPayload>(webhookRequest);

    [Webhook("On user added", typeof(UserAddedWebhookHandler),
        Description = "On a new user added")]
    public Task<WebhookResponse<UserWebhookPayload>> OnUserAdded(WebhookRequest webhookRequest)
        => HandleWebhook<UserWebhookPayload>(webhookRequest);

    [Webhook("On user removed", typeof(UserRemovedWebhookHandler),
        Description = "On a specific user removed")]
    public Task<WebhookResponse<UserWebhookPayload>> OnUserRemoved(WebhookRequest webhookRequest)
        => HandleWebhook<UserWebhookPayload>(webhookRequest);

    [Webhook("On user roles updated", typeof(UserRolesUpdatedWebhookHandler),
        Description = "On a specific user roles updated")]
    public Task<WebhookResponse<UserWebhookPayload>> OnUserRolesUpdated(WebhookRequest webhookRequest)
        => HandleWebhook<UserWebhookPayload>(webhookRequest);

    [Webhook("On data source entry saved", typeof(DataSourceEntrySavedWebhookHandler),
        Description = "On a specific data source entry saved")]
    public Task<WebhookResponse<DataSourceWebhookPayload>> OnDataSourceEntrySaved(WebhookRequest webhookRequest)
        => HandleWebhook<DataSourceWebhookPayload>(webhookRequest);

    [Webhook("On pipeline deployed", typeof(PipelineDeployedWebhookHandler),
        Description = "On a specific pipeline deployed")]
    public Task<WebhookResponse<PipelineWebhookPayload>> OnPipelineDeployed(WebhookRequest webhookRequest)
        => HandleWebhook<PipelineWebhookPayload>(webhookRequest);

    [Webhook("On release merged", typeof(ReleaseMergedWebhookHandler),
        Description = "On a specific release merged")]
    public Task<WebhookResponse<ReleaseWebhookPayload>> OnReleaseMerged(WebhookRequest webhookRequest)
        => HandleWebhook<ReleaseWebhookPayload>(webhookRequest);

    [Webhook("On workflow changed", typeof(WorkflowChangedWebhookHandler),
        Description = "On a specific workflow changed")]
    public Task<WebhookResponse<WorkflowWebhookPayload>> OnWorkflowChanged(WebhookRequest webhookRequest)
        => HandleWebhook<WorkflowWebhookPayload>(webhookRequest);

    private Task<WebhookResponse<T>> HandleWebhook<T>(WebhookRequest request) where T : class
    {
        try
        {
            var payload = request.Body.ToString();
            ArgumentException.ThrowIfNullOrEmpty(payload);

            var data = JsonConvert.DeserializeObject<T>(payload, JsonConfig.Settings);

            if (data is null)
                throw new InvalidOperationException();

            return Task.FromResult(new WebhookResponse<T>()
            {
                Result = data
            });
        }
        catch (Exception ex)
        {
            InvocationContext.Logger?.LogError(
                $"[StoryblokWebhookEvent] Failed to handle webhook {ex.Message}.", [request.Body]);
            throw;
        }
    }

    private async Task<WebhookResponse<StoryWebhookPayload>> HandleStoryWebhookWithTagsAny(WebhookRequest request, WebhookTagsFilter? filter)
    {
        try
        {
            var payloadStr = request.Body.ToString();
            ArgumentException.ThrowIfNullOrEmpty(payloadStr);

            var data = JsonConvert.DeserializeObject<StoryWebhookPayload>(payloadStr, JsonConfig.Settings)
                       ?? throw new InvalidOperationException("Empty payload");

            var wanted = filter?.Tags?
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToArray();

            if (wanted == null || wanted.Length == 0)
                return new WebhookResponse<StoryWebhookPayload> { Result = data };

            var story = await FetchStory(data.SpaceId, data.ContentId);
            if (story?.TagList == null || !story.TagList.Any())
                throw new InvalidOperationException("Filtered out (story not found or no tags).");

            var storyTags = story.TagList
                .Where(t => !string.IsNullOrWhiteSpace(t))
                .Select(t => t.Trim())
                .ToHashSet(StringComparer.OrdinalIgnoreCase);

            var any = wanted.Any(storyTags.Contains);
            if (!any)
                throw new InvalidOperationException("Filtered out by tags (ANY).");

            return new WebhookResponse<StoryWebhookPayload> { Result = data };
        }
        catch (Exception ex)
        {
            InvocationContext.Logger?.LogError($"[StoryblokWebhookEvent] Tag filter blocked event: {ex.Message} - {request.Body}", null);
            throw;
        }
    }

    private async Task<Storyblok.Models.Entities.StoryEntity?> FetchStory(string spaceId, string storyId)
    {
        try
        {
            var endpoint = $"/v1/spaces/{spaceId}/stories/{storyId}";
            var req = new StoryblokRequest(endpoint, Method.Get, Creds);
            var resp = await Client.ExecuteWithErrorHandling<Apps.Storyblok.Models.Response.Story.StoryResponse>(req);
            return resp.Story;
        }
        catch
        {
            return null;
        }
    }
}