namespace Apps.Storyblok.Models.Request.ReleaseReq;

public class CreateReleaseRequest
{
    public ReleasePayload Release { get; set; }

    public CreateReleaseRequest(CreateReleaseInput input)
    {
        Release = new()
        {
            Name = input.Name,
            ReleasedAt = input.ReleasedAt,
            BranchesToDeploy = input.BranchesToDeploy
        };
    }
}