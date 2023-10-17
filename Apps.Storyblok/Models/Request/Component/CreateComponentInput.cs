using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Component;

public class CreateComponentInput
{
    public string Name { get; set; }
    
    [Display("Display name")]
    public string? DisplayName { get; set; }
    
    [Display("Image URL")]
    public string? Image { get; set; }
    
    [Display("Preview field")]
    public string? Preview { get; set; }
    
    [Display("Is root")]
    public bool? IsRoot { get; set; }
    
    [Display("Is nestable")]
    public bool? IsNestable { get; set; }
    
    [Display("Component group UUID")]
    public string? ComponentGroupUuid { get; set; }
}