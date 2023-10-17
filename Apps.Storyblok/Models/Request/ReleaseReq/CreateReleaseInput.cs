using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.ReleaseReq;

public class CreateReleaseInput
{
    public string Name { get; set; }
    
    [Display("Released at")]
    public DateTime? ReleasedAt { get; set; }
    
    [Display("Branches to deploy IDs")]
    public IEnumerable<string>? BranchesToDeploy { get; set; }
}