using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Entities;

public class SpaceEntity
{
    [Display("Space ID")]
    public string Id { get; set; }

    public string Name { get; set; }
    
    public string? Domain { get; set; }
   
    public string? Plan { get; set; }
    
    public string? Role { get; set; }
   
    [Display("Owner ID")]
    public string? OwnerId { get; set; }
       
    [Display("Default root")]
    public string? DefaultRoot { get; set; }
    
    [Display("Created at")]
    public DateTime CreatedAt { get; set; }
}