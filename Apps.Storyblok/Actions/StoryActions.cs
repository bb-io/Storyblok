using System.Net.Mime;
using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.ContentConverters;
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
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;

namespace Apps.Storyblok.Actions;

[ActionList]
public class StoryActions : StoryblokInvocable
{
    private readonly IFileManagementClient _fileManagementClient;

    public StoryActions(InvocationContext invocationContext, IFileManagementClient fileManagementClient) : base(
        invocationContext)
    {
        _fileManagementClient = fileManagementClient;
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
    public async Task<StoryEntity> GetStory([ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
    }

    [Action("Export story content",
        Description = "Exports the localizable content to JSON where all the values can be translated.")]
    public async Task<FileResponse> ExportStoryContent(
        [ActionParameter] StoryRequest story,
        [ActionParameter] OptionalLanguage language)
    {
        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}/export.json";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);
        request.AddQueryParameter("lang_code", language.Language ?? string.Empty);

        var response = await Client.ExecuteWithErrorHandling(request);
        var contentJson = response.Content!;

        var html = StoryblokToHtmlConverter.ToHtml(contentJson);
        using var stream = new MemoryStream(html);
        var file = await _fileManagementClient.UploadAsync(stream, MediaTypeNames.Text.Html, $"{story.StoryId}.html");
        return new() { File = file };
    }

    [Action("Import story content", Description = "Imports a translated story export.")]
    public async Task<StoryEntity> ImportStoryContent(
        [ActionParameter] StoryRequest story,
        [ActionParameter] ImportRequest import)
    {
        var fileStream = await _fileManagementClient.DownloadAsync(import.Content);
        var fileBytes = await fileStream.GetByteData();

        var json = StoryblokToJsonConverter.ToJson(fileBytes, story.StoryId);

        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}/import.json";
        var request = new StoryblokRequest(endpoint, Method.Put, Creds)
            .AddJsonBody(new { data = json });

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
    }

    [Action("Update story", Description = "Update a specific story")]
    public async Task<StoryEntity> UpdateStory(
        [ActionParameter] StoryRequest story,
        [ActionParameter] UpdateStoryInput input)
    {
        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}";
        var request = new StoryblokRequest(endpoint, Method.Put, Creds)
            .WithJsonBody(new UpdateStoryRequest(input), JsonConfig.Settings);

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
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

    [Action("Delete story", Description = "Delete specific story")]
    public Task DeleteStory([ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}";
        var request = new StoryblokRequest(endpoint, Method.Delete, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }

    [Action("Publish story", Description = "Publish specific story")]
    public Task PublishStory([ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}/publish";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }

    [Action("Unpublish story", Description = "Unpublish specific story")]
    public Task UnpublishStory([ActionParameter] StoryRequest story)
    {
        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}/unpublish";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        return Client.ExecuteWithErrorHandling(request);
    }
}