using Apps.Storyblok.Invocable;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Storyblok.Actions;

[ActionList]
public class ComponentActions : StoryblokInvocable
{
    public ComponentActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}