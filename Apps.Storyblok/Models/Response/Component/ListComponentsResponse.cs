using Apps.Storyblok.Models.Entities;

namespace Apps.Storyblok.Models.Response.Component;

public record ListComponentsResponse(List<ComponentEntity> Components);