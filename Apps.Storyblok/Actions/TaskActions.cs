using Apps.Storyblok.Invocable;
using Blackbird.Applications.Sdk.Common.Actions;
using Blackbird.Applications.Sdk.Common.Invocation;

namespace Apps.Storyblok.Actions;

[ActionList]
public class TaskActions : StoryblokInvocable
{
    public TaskActions(InvocationContext invocationContext) : base(invocationContext)
    {
    }
}