using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Asset;

public class AssetRestoredWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "asset.restored";

    public AssetRestoredWebhookHandler([WebhookParameter(true)] SpaceRequest space) : base(space)
    {
    }
}