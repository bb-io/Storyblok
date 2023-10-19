using Apps.Storyblok.Api;
using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Authentication;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Storyblok.Invocables;

public class StoryblokInvocable : BaseInvocable
{
    protected AuthenticationCredentialsProvider[] Creds =>
        InvocationContext.AuthenticationCredentialsProviders.ToArray();
    
    protected StoryblokClient Client { get; }
    
    public StoryblokInvocable(InvocationContext invocationContext) : base(invocationContext)
    {
        Client = new();
    }
}