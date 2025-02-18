using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Other;

public class PipelineDeployedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "pipeline.deployed";

    public PipelineDeployedWebhookHandler([WebhookParameter(true)] SpaceRequest space) : base(space)
    {
    }
}