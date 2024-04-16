using Blackbird.Applications.Sdk.Common;
using Blackbird.Applications.Sdk.Common.Metadata;

namespace Apps.Storyblok;

public class StoryblokApplication : IApplication, ICategoryProvider
{
    public IEnumerable<ApplicationCategory> Categories
    {
        get => [ApplicationCategory.Cms];
        set { }
    }
    
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