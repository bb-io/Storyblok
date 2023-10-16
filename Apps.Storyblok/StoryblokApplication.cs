using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok;

public class StoryblokApplication : IApplication
{
    public string Name
    {
        get => "Storyblok";
        set { }
    }

    public T GetInstance<T>()
    {
        throw new NotImplementedException();
    }
}