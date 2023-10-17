using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Component;

public class ComponentRequest
{
    [Display("Component ID")]
    public string ComponentId { get; set; }
}