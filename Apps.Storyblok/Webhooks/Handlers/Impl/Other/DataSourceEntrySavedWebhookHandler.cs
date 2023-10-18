using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Webhooks.Handlers.Base;
using Blackbird.Applications.Sdk.Common.Webhooks;

namespace Apps.Storyblok.Webhooks.Handlers.Impl.Other;

public class DataSourceEntrySavedWebhookHandler : StoryblokWebhookHandler
{
    protected override string Event => "datasource.entries_updated";

    public DataSourceEntrySavedWebhookHandler([WebhookParameter] SpaceRequest space) : base(space)
    {
    }
}