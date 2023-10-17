using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace Apps.Storyblok.Constants;

public static class JsonConfig
{
    public static JsonSerializerSettings Settings => new()
    {
        DefaultValueHandling = DefaultValueHandling.Ignore,
        ContractResolver = new DefaultContractResolver()
        {
            NamingStrategy = new SnakeCaseNamingStrategy()
        }
    };
}