using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.SDK.Blueprints.Interfaces.CMS;

namespace Apps.Storyblok.Models.Response;

public class FileResponse : IDownloadContentOutput
{
    public FileReference Content { get; set; }
}