using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.User;

public class UserRemovedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "user.removed";

    public UserRemovedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}