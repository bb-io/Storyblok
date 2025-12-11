using Apps.Storyblok.Api;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Request.Story;
using Apps.Storyblok.Models.Response.Story;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Storyblok.DataSourceHandlers;

public class DimensionDataHandler : StoryblokInvocable, IAsyncDataSourceItemHandler
{
    private ImportStoryRequest ImportStoryRequest { get; }

    public DimensionDataHandler(InvocationContext invocationContext, [ActionParameter] ImportStoryRequest importStoryRequest) 
        : base(invocationContext)
    {
        ImportStoryRequest = importStoryRequest;
    }

    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context,
        CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(ImportStoryRequest.SpaceId))
        {
            throw new("You have to input Space first");
        }
        
        if(string.IsNullOrEmpty(ImportStoryRequest.ContentId))
        {
            throw new("You have to input 'Story ID' first");
        }

        var endpoint = $"/v1/spaces/{ImportStoryRequest.SpaceId}/stories/{ImportStoryRequest.ContentId}";
        var request = new StoryblokRequest(endpoint, Method.Get, Creds);
        
        var response = await Client.ExecuteWithErrorHandling<StoryResponse>(request);
        return response.Story.Alternates?
            .Select(alternate => new DataSourceItem(alternate.Id, alternate.FullSlug)) 
            ?? Enumerable.Empty<DataSourceItem>();
    }
}