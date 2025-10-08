using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.ReleaseReq;
using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Models.Response.ReleaseResp;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Storyblok.Actions;

[ActionList]
public class ReleaseActions(InvocationContext invocationContext) : StoryblokInvocable(invocationContext)
{
    [Action("Search releases", Description = "Search all releases in your space")]
    public Task<ListReleasesResponse> ListReleases(
        [ActionParameter] SpaceRequest space)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/releases";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<ListReleasesResponse>(request);
    }

    [Action("Get release", Description = "Get details of a specific release")]
    public async Task<ReleaseEntity> GetRelease(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] ReleaseRequest release)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/releases/{release.ReleaseId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<ReleaseResponse>(request);
        return response.Release;
    }

    [Action("Create release", Description = "Create a new release")]
    public async Task<ReleaseEntity> CreateRelease(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] CreateReleaseInput input)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/releases/";
        var request = new StoryblokRequest(endpoint, Method.Post, Creds)
            .WithJsonBody(new CreateReleaseRequest(input), JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling<ReleaseResponse>(request);
        return response.Release;
    }

    [Action("Delete release", Description = "Delete specific release")]
    public Task DeleteRelease(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] ReleaseRequest release)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/releases/{release.ReleaseId}";
        var request = new StoryblokRequest(endpoint, Method.Delete, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }
}