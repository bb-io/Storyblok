using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.User;

public class UserRolesUpdatedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "user.roles_updated";

    public UserRolesUpdatedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}