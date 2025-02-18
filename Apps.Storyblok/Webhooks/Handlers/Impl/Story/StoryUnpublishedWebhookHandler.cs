using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Story;

public class StoryUnpublishedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "story.unpublished";

    public StoryUnpublishedWebhookHandler([WebhookParameter(true)] SpaceRequest space) : base(space)
    {
    }
}