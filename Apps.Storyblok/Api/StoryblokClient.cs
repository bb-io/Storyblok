using Apps.Storyblok.Constants;
using Apps.Storyblok.Models.Response;
using Apps.Storyblok.Models.Response.Pagination.Base;
using Blackbird.Applications.Sdk.Common.Exceptions;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using RestSharp;

namespace Apps.Storyblok.Api;

public class StoryblokClient : BlackBirdRestClient
{
    protected override JsonSerializerSettings? JsonSettings => JsonConfig.Settings;

    public StoryblokClient() : base(new()
    {
        BaseUrl = Urls.Api.ToUri()
    })
    {
    }

    public override async Task<T> ExecuteWithErrorHandling<T>(RestRequest request)
    {
        string content = (await ExecuteWithErrorHandling(request)).Content;
        T val = JsonConvert.DeserializeObject<T>(content, JsonSettings);
        if (val == null)
        {
            throw new Exception($"Could not parse {content} to {typeof(T)}");
        }

        return val;
    }

    public override async Task<RestResponse> ExecuteWithErrorHandling(RestRequest request)
    {
        RestResponse restResponse = await ExecuteAsync(request);
        if (!restResponse.IsSuccessStatusCode)
        {
            throw ConfigureErrorException(restResponse);
        }

        return restResponse;
    }

    protected override Exception ConfigureErrorException(RestResponse response)
    {
       
        if (response == null)
        {
            return new PluginApplicationException($"Error: {response.ErrorMessage}");
        }

        if (string.IsNullOrEmpty(response.Content))
        {
            return new PluginApplicationException($"Error: {response.ErrorMessage}");
        }

        var responseContent = response.Content!;
        try
        {
            var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(responseContent, JsonSettings);
            if (errorResponse?.Error != null)
            {
                return new PluginApplicationException(errorResponse.Error);
            }
        }
        catch (Exception ex)
        {
            return new PluginApplicationException($"Error: {ex.Message}. Raw content: {responseContent}", ex);
        }

        return new PluginApplicationException($"Error: {responseContent}");
    }

    public async Task<List<TV>> Paginate<T, TV>(RestRequest request) where T : PaginationResponse<TV>
    {
        var baseUrl = request.Resource;
        var page = 1;

        var result = new List<TV>();
        T? response;

        do
        {
            request.Resource = baseUrl.SetQueryParameter("page", (page++).ToString());

            response = await ExecuteWithErrorHandling<T>(request);
            result.AddRange(response.Items);
        } while (response.Items.Any());

        return result;
    }
}