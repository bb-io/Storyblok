using Apps.Storyblok.Api;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Invocation;
using RestSharp;

namespace Apps.Storyblok.DataSourceHandlers
{
    public class TagsDataHandler : StoryblokInvocable, IAsyncDataSourceHandler
    {
        private StoryRequest StoryRequest { get; }
        public TagsDataHandler(InvocationContext invocationContext, [ActionParameter] StoryRequest storyRequest) : base(
         invocationContext)
        {
            StoryRequest = storyRequest;
        }

        public async Task<Dictionary<string, string>> GetDataAsync(DataSourceContext context, CancellationToken ct)
        {
            var search = (context.SearchString ?? string.Empty).Trim();
            var endpoint = $"/v1/spaces/{StoryRequest.SpaceId}/tags/";
            var request = new StoryblokRequest(endpoint, Method.Get, Creds);

            if (!string.IsNullOrWhiteSpace(search))
                request.AddQueryParameter("search", search);

            var resp = await Client.ExecuteWithErrorHandling<TagsListResponse>(request);

            var items = resp.Tags
                .Where(t => !string.IsNullOrWhiteSpace(t.Name))
                .GroupBy(t => t.Name, StringComparer.OrdinalIgnoreCase)
                .Select(g => new { Name = g.Key, Count = g.Max(x => x.TaggingsCount ?? 0) })
                .OrderByDescending(x => x.Count)
                .ThenBy(x => x.Name, StringComparer.OrdinalIgnoreCase)
                .Take(500)
                .ToDictionary(x => x.Name, x => x.Name, StringComparer.OrdinalIgnoreCase);

            return items;
        }
    }
}
