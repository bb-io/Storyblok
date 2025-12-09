using Apps.Storyblok.Api;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.Story;
using Apps.Storyblok.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Storyblok.DataSourceHandlers;

public class StoryDataHandler : StoryblokInvocable, IAsyncDataSourceHandler
{
    private StoryRequest StoryRequest { get; }

    public StoryDataHandler(InvocationContext invocationContext, [ActionParameter] StoryRequest storyRequest) : base(
        invocationContext)
    {
        StoryRequest = storyRequest;
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(StoryRequest.SpaceId))
            throw new("You have to input Space first");

        var endpoint = $"/v1/spaces/{StoryRequest.SpaceId}/stories";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        var items = await Client.Paginate<StoriesPaginationResponse, StoryEntity>(request);
        return items
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.CreatedAt)
            .ToDictionary(x => x.ContentId, x => x.Name);
    }
}