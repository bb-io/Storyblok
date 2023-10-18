using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Asset;

public class AssetDeletedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "asset.deleted";

    public AssetDeletedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}