using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Asset;

public class AssetReplacedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "asset.replaced";

    public AssetReplacedWebhookHandler([WebhookParameter(true)] SpaceRequest space) : base(space)
    {
    }
}