using Apps.Storyblok.Models.Entities;

namespace Apps.Storyblok.Models.Response.Story;

public record ListStoriesResponse(List<StoryEntity> Stories);