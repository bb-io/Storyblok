using Apps.Storyblok.Constants;
using Apps.Storyblok.Models.Response;
using Apps.Storyblok.Models.Response.Pagination.Base;
using Blackbird.Applications.Sdk.Utils.Extensions.String;
using Blackbird.Applications.Sdk.Utils.RestSharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
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

    protected override Exception ConfigureErrorException(RestResponse response)
    {
        var responseContent = response.Content!;

        var error = JsonConvert.DeserializeObject<ErrorResponse>(responseContent)!;
        return new(error.Error ?? responseContent);
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