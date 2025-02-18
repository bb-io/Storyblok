using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Other;

public class WorkflowChangedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "stage.changed";

    public WorkflowChangedWebhookHandler([WebhookParameter(true)] SpaceRequest space) : base(space)
    {
    }
}