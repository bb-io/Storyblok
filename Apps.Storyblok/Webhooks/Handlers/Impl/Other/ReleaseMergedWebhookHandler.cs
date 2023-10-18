using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Other;

public class ReleaseMergedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "release.merged";

    public ReleaseMergedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}