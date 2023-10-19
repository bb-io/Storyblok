using System.Net.Mime;
using System.Text;
using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Models.Request.Story;
using Apps.Storyblok.Models.Response;
using Apps.Storyblok.Models.Response.Pagination;
using Apps.Storyblok.Models.Response.Story;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using RestSharp;

namespace Apps.Storyblok.Actions;

[ActionList]
public class StoryActions : StoryblokInvocable
{
    public StoryActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("List stories", Description = "List all stories in your space")]
    public async Task<ListStoriesResponse> ListStories(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] ListStoriesRequest query)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories".WithQuery(query);
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var items = await Client.Paginate<StoriesPaginationResponse, StoryEntity>(request);
        return new(items);
    }

    [Action("Get story", Description = "Get details of a specific story")]
    public async Task<StoryEntity> GetStory(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/{story.StoryId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
    }

    [Action("Get story content", Description = "Get content of a specific story")]
    public async Task<FileResponse> GetStoryContent(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/{story.StoryId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<StoryContentResponse>(request);
        var contentJson = response.Story.Content.ToString();

        return new()
        {
            File = new(Encoding.UTF8.GetBytes(contentJson))
            {
                Name = $"{story.StoryId}.json",
                ContentType = MediaTypeNames.Application.Json
            }
        };
    }

    [Action("Create story", Description = "Create a new story")]
    public async Task<StoryEntity> CreateStory(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] CreateStoryInput input)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/";
        var request = new StoryblokRequest(endpoint, Method.Post, Creds)
            .WithJsonBody(new CreateStoryRequest(input), JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
    }
    
    [Action("Update story", Description = "Update a specific story")]
    public async Task<StoryEntity> UpdateStory(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] StoryRequest story,
        [ActionParameter] UpdateStoryInput input)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/{story.StoryId}";
        var request = new StoryblokRequest(endpoint, Method.Put, Creds)
            .WithJsonBody(new UpdateStoryRequest(input), JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
    }

    [Action("Delete story", Description = "Delete specific story")]
    public Task DeleteStory(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/{story.StoryId}";
        var request = new StoryblokRequest(endpoint, Method.Delete, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }

    [Action("Publish story", Description = "Publish specific story")]
    public Task PublishStory(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/{story.StoryId}/publish";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }

    [Action("Unpublish story", Description = "Unpublish specific story")]
    public Task UnpublishStory(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/{story.StoryId}/unpublish";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }
}