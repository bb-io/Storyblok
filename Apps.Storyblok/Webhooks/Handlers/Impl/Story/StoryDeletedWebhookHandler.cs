using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Story;

public class StoryDeletedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "story.deleted";

    public StoryDeletedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}