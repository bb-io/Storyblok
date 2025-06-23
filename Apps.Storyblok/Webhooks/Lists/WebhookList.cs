using Apps.Storyblok.Constants;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Webhooks.Handlers.Impl.Asset;
using Apps.Storyblok.Webhooks.Handlers.Impl.Other;
using Apps.Storyblok.Webhooks.Handlers.Impl.Story;
using Apps.Storyblok.Webhooks.Handlers.Impl.User;
using Apps.Storyblok.Webhooks.Models.Payloads;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Common.Webhooks;
using Newtonsoft.Json;

namespace Apps.Storyblok.Webhooks.Lists;

[WebhookList]
public class WebhookList: StoryblokInvocable
{
    public WebhookList(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Webhook("On story published", typeof(StoryPublishedWebhookHandler),
        Description = "On a specific story published")]
    public Task<WebhookResponse<StoryWebhookPayload>> OnStoryPublished(WebhookRequest webhookRequest)
        => HandleWebhook<StoryWebhookPayload>(webhookRequest);

    [Webhook("On story unpublished", typeof(StoryUnpublishedWebhookHandler),
        Description = "On a specific story unpublished")]
    public Task<WebhookResponse<StoryWebhookPayload>> OnStoryUnpublished(WebhookRequest webhookRequest)
        => HandleWebhook<StoryWebhookPayload>(webhookRequest);

    [Webhook("On story moved", typeof(StoryMovedWebhookHandler),
        Description = "On a specific story moved")]
    public Task<WebhookResponse<StoryWebhookPayload>> OnStoryMoved(WebhookRequest webhookRequest)
        => HandleWebhook<StoryWebhookPayload>(webhookRequest);

    [Webhook("On story deleted", typeof(StoryDeletedWebhookHandler),
        Description = "On a specific story deleted")]
    public Task<WebhookResponse<StoryWebhookPayload>> OnStoryDeleted(WebhookRequest webhookRequest)
        => HandleWebhook<StoryWebhookPayload>(webhookRequest);

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
}