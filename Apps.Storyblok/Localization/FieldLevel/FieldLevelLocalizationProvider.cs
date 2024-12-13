using Apps.Storyblok.Api;
using Apps.Storyblok.Models.Request;
using Apps.Storyblok.Models.Request.Story;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Files;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Storyblok.Localization.FieldLevel;
public class FieldLevelLocalizationProvider : ILocalizationProvider
{
    private readonly InvocationContext _invocationContext;

    private readonly AuthenticationCredentialsProvider[] _creds;

    private readonly StoryblokClient _client;

    public FieldLevelLocalizationProvider(InvocationContext invocationContext, StoryblokClient client)
    {
        _invocationContext = invocationContext;
        _creds = invocationContext.AuthenticationCredentialsProviders.ToArray();
        _client = client;
    }


    public async Task<byte[]> ExportStoryContent(StoryRequest storyRequest, OptionalLanguage language)
    {
        throw new NotImplementedException();
    }
}

