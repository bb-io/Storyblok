using Apps.Storyblok.Models.Entities;

namespace Apps.Storyblok.Models.Response.ReleaseResp;

public record ListReleasesResponse(List<ReleaseEntity> Releases);