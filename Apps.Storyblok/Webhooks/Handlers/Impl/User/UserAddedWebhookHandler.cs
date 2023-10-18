using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.User;

public class UserAddedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "user.added";

    public UserAddedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}