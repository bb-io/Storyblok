using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Invocable;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Models.Request.Task;
using Apps.Storyblok.Models.Response.Pagination;
using Apps.Storyblok.Models.Response.Task;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Storyblok.Actions;

[ActionList]
public class TaskActions : StoryblokInvocable
{
    public TaskActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("List tasks", Description = "List all tasks in your space")]
    public async Task<ListTasksResponse> ListTasks(
        [ActionParameter] SpaceRequest space)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/tasks";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var items = await Client.Paginate<TasksPaginationResponse, TaskEntity>(request);
        return new(items);
    }

    [Action("Get task", Description = "Get details of a specific task")]
    public async Task<TaskEntity> GetTask(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] TaskRequest task)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/tasks/{task.TaskId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<TaskResponse>(request);
        return response.Task;
    }

    [Action("Create task", Description = "Create a new task")]
    public async Task<TaskEntity> CreateTask(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] CreateTaskInput input)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/tasks/";
        var request = new StoryblokRequest(endpoint, Method.Post, Creds)
            .WithJsonBody(new CreateTaskRequest(input), JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling<TaskResponse>(request);
        return response.Task;
    }

    [Action("Delete task", Description = "Delete specific task")]
    public Task DeleteTask(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] TaskRequest task)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/tasks/{task.TaskId}";
        var request = new StoryblokRequest(endpoint, Method.Delete, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }
}