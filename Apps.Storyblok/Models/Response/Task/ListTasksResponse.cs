using Apps.Storyblok.Models.Entities;

namespace Apps.Storyblok.Models.Response.Task;

public record ListTasksResponse(List<TaskEntity> Tasks);