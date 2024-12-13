using Apps.Storyblok.Api;
using Apps.Storyblok.Constants;
using Apps.Storyblok.Localization;
using Apps.Storyblok.Localization.FieldLevel;
using Apps.Storyblok.Localization.FolderLevel;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;
using Blackbird.Applications.SDK.Extensions.FileManagement.Interfaces;

namespace Apps.Storyblok.Invocables;

public class StoryblokInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();
    
    protected StoryblokClient Client { get; }
    protected ILocalizationProvider LocalizationProvider { get; }
    
    public StoryblokInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new();

        var localizationLevel = invocationContext.AuthenticationCredentialsProviders.FirstOrDefault(key => key.KeyName == CredsNames.LocalizationLevel).Value;
        
        if (localizationLevel == CredsNames.FieldLocalizationLevel)
            LocalizationProvider = new FieldLevelLocalizationProvider(invocationContext, Client);
        else if(localizationLevel == CredsNames.FolderLocalizationLevel)
            LocalizationProvider = new FolderLevelLocalizationProvider(invocationContext, Client);
    }
}