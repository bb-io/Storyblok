using RestSharp;
using Apps.Storyblok.Api;
using Apps.Storyblok.Invocables;
using Apps.Storyblok.Models.Request.Story;
using Apps.Storyblok.Models.Response.Component;
using Apps.Storyblok.Models.Response.Story;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Dynamic;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Common.Invocation;
using Newtonsoft.Json.Linq;
using System.Globalization;

namespace Apps.Storyblok.DataSourceHandlers;

public class StoryFieldsDataHandler(InvocationContext context, [ActionParameter] StoryRequest story)
    : StoryblokInvocable(context), IAsyncDataSourceItemHandler
{
    public async Task<IEnumerable<DataSourceItem>> GetDataAsync(DataSourceContext context, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(story.ContentId))
            throw new PluginMisconfigurationException();

        var storyEndpoint = $"/v1/spaces/{story.SpaceId}/stories/{story.ContentId}";
        var storyRequest = new StoryblokRequest(storyEndpoint, Method.Get, Creds);
        var storyResponse = await Client.ExecuteWithErrorHandling<StoryResponse>(storyRequest);

        var content = JObject.Parse(storyResponse.Story.Content.ToString());
        var componentName = content["component"]?.ToString();
        if (string.IsNullOrEmpty(componentName))
            return [];

        var componentsEndpoint = $"/v1/spaces/{story.SpaceId}/components";
        var compRequest = new StoryblokRequest(componentsEndpoint, Method.Get, Creds);
        compRequest.AddQueryParameter("search", componentName);

        var compResponse = await Client.ExecuteWithErrorHandling<ListComponentsResponse>(compRequest);
        var component = compResponse.Components.FirstOrDefault(x => x.Name == componentName);
        if (component == null || component.Schema == null)
            return [];

        var schema = JObject.Parse(component.Schema.ToString());
        var fields = new List<DataSourceItem>();

        foreach (var property in schema.Properties())
        {
            var fieldKey = property.Name;

            if (context.SearchString != null && !fieldKey.Contains(context.SearchString, StringComparison.OrdinalIgnoreCase))
                continue;

            var displayLabel = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(fieldKey.Replace("_", " "));
            fields.Add(new DataSourceItem(fieldKey, displayLabel));
        }

        return fields;
    }
}
