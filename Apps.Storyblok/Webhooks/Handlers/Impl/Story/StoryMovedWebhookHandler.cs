using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Story;

public class StoryMovedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "story.moved";

    public StoryMovedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}