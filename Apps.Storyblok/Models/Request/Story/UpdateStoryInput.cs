using File = Blackbird.Applications.Sdk.Common.Files.File;

namespace Apps.Storyblok.Models.Request.Story;

public class UpdateStoryInput
{
    public File Content { get; set; }
    
    public string Lang { get; set; }
}