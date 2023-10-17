using Blackbird.Applications.Sdk.Utils.Json.Converters;
using Newtonsoft.Json;

namespace Apps.Storyblok.Models.Request.ReleaseReq;

public class ReleasePayload
{
    public string Name { get; set; }

    public DateTime? ReleasedAt { get; set; }

    [JsonConverter(typeof(StringToIntConverter), nameof(BranchesToDeploy))]
    public IEnumerable<string>? BranchesToDeploy { get; set; }
}