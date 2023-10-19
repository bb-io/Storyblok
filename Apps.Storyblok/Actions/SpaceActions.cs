using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Models.Response.Space;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Storyblok.Actions;

[ActionList]
public class SpaceActions : StoryblokInvocable
{
    public SpaceActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    [Action("List spaces", Description = "List all spaces in your space")]
    public Task<ListSpacesResponse> ListSpaces()
    {
        var request = new StoryblokRequest("/v1/spaces", Method.Get, Creds);
        return Client.ExecuteWithErrorHandling<ListSpacesResponse>(request);
    }

    [Action("Get space", Description = "Get details of a specific space")]
    public async Task<SpaceEntity> GetSpace([ActionParameter] SpaceRequest space)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<SpaceResponse>(request);
        return response.Space;
    }

    [Action("Create space", Description = "Create a new space")]
    public async Task<SpaceEntity> CreateSpace([ActionParameter] CreateSpaceInput input)
    {
        var endpoint = "/v1/spaces";
        var request = new StoryblokRequest(endpoint, Method.Post, Creds)
            .WithJsonBody(new CreateSpaceRequest(input), JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling<SpaceResponse>(request);
        return response.Space;
    }

    [Action("Delete space", Description = "Delete specific space")]
    public Task DeleteSpace([ActionParameter] SpaceRequest space)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}";
        var request = new StoryblokRequest(endpoint, Method.Delete, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }

    [Action("Backup space", Description = "Backup specific space")]
    public async Task<SpaceEntity> BackupSpace([ActionParameter] SpaceRequest space)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/backups";
        var request = new StoryblokRequest(endpoint, Method.Post, Creds);

        var response = await Client.ExecuteWithErrorHandling<SpaceResponse>(request);
        return response.Space;
    }
}