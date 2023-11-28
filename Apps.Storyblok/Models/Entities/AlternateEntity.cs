using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Entities;

public class AlternateEntity
{
    [Display("Story ID")] public string Id { get; set; }
    
    [Display("Full slug")] public string FullSlug { get; set; }
    
    [Display("Root folder")] public string RootFolder => FullSlug.Split("/").First();
    
    [Display("Is published")] public bool Published { get; set; }
}