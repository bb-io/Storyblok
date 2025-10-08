using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.Component;
using Apps.Storyblok.Models.Request.Space;
using Apps.Storyblok.Models.Response.Component;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using RestSharp;

namespace Apps.Storyblok.Actions;

[ActionList]
public class ComponentActions(InvocationContext invocationContext) : StoryblokInvocable(invocationContext)
{    
    [Action("Search components", Description = "List all components in your space")]
    public Task<ListComponentsResponse> ListComponents(
        [ActionParameter] SpaceRequest space)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/components";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling<ListComponentsResponse>(request);
    }

    [Action("Get component", Description = "Get details of a specific component")]
    public async Task<ComponentEntity> GetComponent(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] ComponentRequest component)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/components/{component.ComponentId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<ComponentResponse>(request);
        return response.Component;
    }
    
    [Action("Create component", Description = "Create a new component")]
    public async Task<ComponentEntity> CreateComponent(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] CreateComponentInput input)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/components/";
        var request = new StoryblokRequest(endpoint, Method.Post, Creds)
            .WithJsonBody(new CreateComponentRequest(input), JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling<ComponentResponse>(request);
        return response.Component;
    }

    [Action("Delete component", Description = "Delete specific component")]
    public Task DeleteComponent(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] ComponentRequest component)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/components/{component.ComponentId}";
        var request = new StoryblokRequest(endpoint, Method.Delete, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }
}