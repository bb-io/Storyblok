using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.ReleaseReq;

public class ReleaseRequest
{
    [Display("Release ID")]
    public string ReleaseId { get; set; }
}