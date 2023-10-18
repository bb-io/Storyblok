using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Asset;

public class AssetCreatedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "asset.created";

    public AssetCreatedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}