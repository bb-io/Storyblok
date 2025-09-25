using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Webhooks.Models.Request
{
    public class WebhookTagsFilter
    {
        [Display("Tags", Description = "Trigger only if the story has ANY of these tags.")]
        public IEnumerable<string>? Tags { get; set; }
    }
}
