using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Request.Space;

public class SpaceRequest
{
    //todo: add data source
    [Display("Space ID")]
    public string SpaceId { get; set; }
}