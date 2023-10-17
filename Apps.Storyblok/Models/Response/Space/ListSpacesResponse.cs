using Apps.Storyblok.Models.Entities;

namespace Apps.Storyblok.Models.Response.Space;

public record ListSpacesResponse(List<SpaceEntity> Spaces);