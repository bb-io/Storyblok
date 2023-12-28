using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Files;

namespace Apps.Storyblok.Models.Request.Story;

public class ImportRequest
{
    [Display("Translated content")]
    public FileReference Content { get; set; }
}