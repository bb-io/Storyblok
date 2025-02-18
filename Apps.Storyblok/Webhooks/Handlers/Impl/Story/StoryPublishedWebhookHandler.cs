using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Story;

public class StoryPublishedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "story.published";

    public StoryPublishedWebhookHandler([WebhookParameter(true)] SpaceRequest space) : base(space)
    {
    }
}