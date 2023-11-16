using System.Net.Mime;
using System.Text;
using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request;
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

    // TODO: Turn richtext fields into HTML.

    // TODO: Even with lang_code default, sometimes some non-localizable fields are sent. F.e. "text_color" or "background-blur".
    // My hypothesis is that it's because Storyblok will filter all the default-non localizable fields but for custom fields it doesn't know
    // Whether it should be localized.
    // Two options: Either we filter-out or we filter-in. Both have pros and cons. Keep a list of things we should filter out will result in still having
    // Non localizable fields in the future, filter-in will be problematic for custom components as they will have localizable fields that are not on our list.
    // For now let's choose filter-out and add all keys that end with "text_color", "background-blur", etc. etc.

    // TODO: Make language code dynamic input.
    // TODO: Remove language, url, page, text_nodes from exported json
    [Action("Export story content", Description = "Exports the localizable content to JSON where all the values can be translated.")]
    public async Task<FileResponse> ExportStoryContent(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] StoryRequest story,
        [ActionParameter] OptionalLanguage language)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/{story.StoryId}/export.json";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);
        request.AddQueryParameter("lang_code", language.Language ?? "default");

        var response = await Client.ExecuteWithErrorHandling(request);
        var contentJson = response.Content;

        return new()
        {
            File = new(Encoding.UTF8.GetBytes(contentJson))
            {
                Name = $"{story.StoryId}.json",
                ContentType = MediaTypeNames.Application.Json
            }
        };
    }

    // TODO: Turn tagged richtext back into JSOn structure.
    // TODO: Verify if page, language, url and text_nodes are needed here. If so re-add them in this method.
    [Action("Import story content", Description = "Imports a translated story export.")]
    public async Task<StoryEntity> ImportStoryContent(
    [ActionParameter] SpaceRequest space,
    [ActionParameter] StoryRequest story,
    [ActionParameter] ImportRequest import)
    {
        var json = Encoding.UTF8.GetString(import.Content.Bytes);
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories/{story.StoryId}/import.json";
        var request = new StoryblokRequest(endpoint, Method.Put, Creds);
        request.AddJsonBody(new { data = json });

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
    }

    // TODO: Remove content realted inputs.
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

    // TODO: Remove content related inputs.
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