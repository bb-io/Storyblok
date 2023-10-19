using Apps.Storyblok.Api;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Response.Space;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Storyblok.DataSourceHandlers;

public class SpaceDataHandler : StoryblokInvocable, IAsyncDataSourceHandler
{
    public SpaceDataHandler(InvocationContext invocationContext) : base(invocationContext)
    {
    }

    public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        var request = new StoryblokRequest("/v1/spaces", Method.Get, Creds);
        var response = await Client.ExecuteWithErrorHandling<ListSpacesResponse>(request);

        return response.Spaces
            .Where(x => context.SearchString is null ||
                        x.Name.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
            .OrderByDescending(x => x.CreatedAt)
            .Take(30)
            .ToDictionary(x => x.Id, x => x.Name);
    }
}