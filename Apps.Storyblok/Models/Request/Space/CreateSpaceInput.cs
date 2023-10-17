using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Space;

public class CreateSpaceInput
{
    public string Name { get; set; }
    
    public string? Domain { get; set; }
    
    [Display("Published webhook URL")]
    public string? StoryPublishedHook { get; set; }
    
    [Display("Searchblok ID")]
    public string? SearchblokId { get; set; }
}