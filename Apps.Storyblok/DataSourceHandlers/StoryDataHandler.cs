using Apps.Storyblok.Api;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Request.Story;
using Apps.Storyblok.Models.Response.Pagination;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Storyblok.DataSourceHandlers;

public class StoryDataHandler(InvocationContext invocationContext, [ActionParameter] StoryRequest storyRequest) 
    : StoryblokInvocable(invocationContext), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(storyRequest.SpaceId))
            throw new("You have to input Space first");

        var endpoint = $"/v1/spaces/{storyRequest.SpaceId}/stories";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);

        if (!string.IsNullOrEmpty(context.SearchString))
            request.AddQueryParameter("search", context.SearchString);

        var items = await Client.ExecuteWithErrorHandling<StoriesPaginationResponse>(request);
        return items.Items
            .OrderByDescending(x => x.CreatedAt)
            .Select(x => new DataSourceItem(x.ContentId, x.Name))
            .ToList();
    }
}