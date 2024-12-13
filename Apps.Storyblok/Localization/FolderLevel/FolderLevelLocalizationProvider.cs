using Apps.Storyblok.Api;
using Apps.Storyblok.ContentConverters;
using Apps.Storyblok.Models.Request;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;
using RestSharp;
using System.Net.Mime;

namespace Apps.Storyblok.Localization.FolderLevel;
public class FolderLevelLocalizationProvider : ILocalizationProvider
{
    private readonly InvocationContext _invocationContext;

    private readonly AuthenticationCredentialsProvider[] _creds;

    private readonly StoryblokClient _client;

    public FolderLevelLocalizationProvider(InvocationContext invocationContext, StoryblokClient client)
    {
        _invocationContext = invocationContext;
        _creds = _invocationContext.AuthenticationCredentialsProviders.ToArray();
        _client = client;
    }

    public async Task<byte[]> ExportStoryContent(StoryRequest storyRequest, OptionalLanguage language)
    {
        var endpoint = $"/v1/spaces/{storyRequest.SpaceId}/stories/{storyRequest.StoryId}/export.json";
        var request = new StoryblokRequest(endpoint, Method.Get, _creds);
        request.AddQueryParameter("lang_code", language.Language ?? string.Empty);

        var response = await _client.ExecuteWithErrorHandling(request);
        var contentJson = response.Content!;

        var html = StoryblokToHtmlConverter.ToHtml(contentJson);   
        return html;
    }
}

