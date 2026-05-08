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
    
    protected override Exception ConfigureErrorException(RestResponse response)
    {
        if (string.IsNullOrEmpty(response.Content))
        {
            if (string.IsNullOrEmpty(response.ErrorMessage))
            {
                return new PluginApplicationException($"Request failed with status code {response.StatusCode} and no content or error message.");
            }
            
            return new PluginApplicationException($"Request failed with status code {response.StatusCode} and error message: {response.ErrorMessage}");
        }
        
        if(response.ContentType == "text/html")
        {
            return new PluginApplicationException($"Request failed with status code {response.StatusCode} and HTML content: {response.Content}");
        }
        
        var errorMessage = GetErrorMessage(response.Content!);
        return new PluginApplicationException(errorMessage);
    }
    
    private static string GetErrorMessage(string content)
    {
        try
        {
            if (content.StartsWith("["))
            {
                var errorResponses = JsonConvert.DeserializeObject<List<string>>(content, JsonConfig.Settings);
                if (errorResponses != null && errorResponses.Any())
                {
                    return string.Join(", ", errorResponses);
                }
            }
            else
            {
                var errorResponse = JsonConvert.DeserializeObject<ErrorResponse>(content, JsonConfig.Settings);
                if (errorResponse?.Error != null)
                {
                    return errorResponse.Error;
                }
            }
        }
        catch (Exception ex)
        {
            return $"Error parsing content: {ex.Message}. Raw content: {content}";
        }

        return $"Unknown error: {content}";
    }
}