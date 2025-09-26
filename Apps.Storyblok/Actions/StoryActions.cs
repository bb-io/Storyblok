using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.ContentConverters;
using Apps.Storyblok.DataSourceHandlers;
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
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.Sdk.Utils.Extensions.Files;
using Blackbird.Applications.Sdk.Utils.Extensions.Http;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using Blackbird.Filters.Transformations;
using Blackbird.Filters.Xliff.Xliff2;
using RestSharp;
using System.Net.Mime;
using System.Text;

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

    [Action("Search stories", Description = "Search all stories in your space")]
    public async Task<ListStoriesResponse> ListStories(
        [ActionParameter] SpaceRequest space,
        [ActionParameter] ListStoriesRequest query,
        [ActionParameter] ListStoriesTagsInput tagsInput)
    {
        var endpoint = $"/v1/spaces/{space.SpaceId}/stories".WithQuery(query);
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var tags = tagsInput?.Tags?
       .Where(t => !string.IsNullOrWhiteSpace(t))
       .Select(t => t.Trim())
       .Distinct(StringComparer.OrdinalIgnoreCase)
       .ToArray();

        if (tags is { Length: > 0 })
            request.AddQueryParameter("with_tag", string.Join(",", tags));

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
        var html = Encoding.UTF8.GetString(await fileStream.GetByteData());

        if (Xliff2Serializer.IsXliff2(html))
        {
            html = Transformation.Parse(html, import.Content.Name).Target().Serialize();
            if (html == null) throw new PluginMisconfigurationException("XLIFF did not contain any files");
        }

        var json = StoryblokToJsonConverter.ToJson(html, story.StoryId);

        var endpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.StoryId}/import.json";
        var request = new StoryblokRequest(endpoint, Method.Put, Creds)
            .AddJsonBody(new { data = json });

        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story;
    }

    [Action("Update story", Description = "Update a specific story")]
    public async Task<StoryEntity> UpdateStory(
        [ActionParameter] StoryRequest story,
        [ActionParameter] LanguageRequest input)
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

    [Action("Add tags to story", Description = "Add one or more tags to a specific story")]
    public async Task<StoryEntity> AddTagsToStory(
      [ActionParameter] StoryRequest story,
      [ActionParameter] AddTagsToStoryRequest input)
    {
        var newTags = input.Tags?
           .Where(t => !string.IsNullOrWhiteSpace(t))
           .Select(t => t.Trim())
           .Distinct(StringComparer.OrdinalIgnoreCase)
           .ToArray();

        if (newTags == null || newTags.Length == 0)
            throw new PluginMisconfigurationException("Provide at least one non-empty tag.");

        if (!long.TryParse(story.StoryId, out var storyId))
            throw new PluginMisconfigurationException("Story ID must be a numeric.");


        var current = await GetStory(story);
        var currentTags = (current.TagList ?? Enumerable.Empty<string>())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        var toAdd = newTags
           .Except(currentTags, StringComparer.OrdinalIgnoreCase)
           .ToArray();

        if (toAdd.Length == 0)
        {
            return current;
        }

        var mergedTags = currentTags
        .Union(newTags, StringComparer.OrdinalIgnoreCase)
        .ToArray();

        var endpoint = $"/v1/spaces/{story.SpaceId}/tags/bulk_association";
        var payload = new
        {
            tags = new
            {
                stories = new[]
                {
                    new { story_id = storyId, tag_list = mergedTags  }
                }
            }
        };

        var request = new StoryblokRequest(endpoint, Method.Post, Creds).AddJsonBody(payload);
        await Client.ExecuteWithErrorHandling(request);
        Task.Delay(1000);

        var refreshed = await GetStory(story);
        return refreshed;
    }

    [Action("Remove tag from story", Description = "Remove a single tag from a specific story")]
    public async Task<StoryEntity> RemoveTagFromStory(
    [ActionParameter] StoryRequest story,
    [ActionParameter, Display("Tag")][DataSource(typeof(TagsDataHandler))] string tag)
    {
        var tagToRemove = (tag ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(tagToRemove))
            throw new PluginMisconfigurationException("Tag must be provided.");

        if (!long.TryParse(story.StoryId, out var storyId))
            throw new PluginMisconfigurationException("Story ID must be numeric.");

        var current = await GetStory(story);
        var currentTags = (current.TagList ?? Enumerable.Empty<string>())
            .Where(t => !string.IsNullOrWhiteSpace(t))
            .Select(t => t.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToArray();

        if (!currentTags.Contains(tagToRemove, StringComparer.OrdinalIgnoreCase))
            return current;

        var remainingTags = currentTags
            .Where(t => !string.Equals(t, tagToRemove, StringComparison.OrdinalIgnoreCase))
            .ToArray();

        var endpoint = $"/v1/spaces/{story.SpaceId}/tags/bulk_association";
        var payload = new
        {
            tags = new
            {
                stories = new[]
                {
                new { story_id = storyId, tag_list = remainingTags }
            }
            }
        };

        var request = new StoryblokRequest(endpoint, Method.Post, Creds).AddJsonBody(payload);
        await Client.ExecuteWithErrorHandling(request);

        await Task.Delay(1000);

        return await GetStory(story);
    }
}
