using Newtonsoft.Json;

namespace Apps.Storyblok.Models.Request.Story
{
    public class TagEntity
    {
        [JsonProperty("name")]
        public string Name { get; set; } = default!;

        [JsonProperty("taggings_count")]
        public int? TaggingsCount { get; set; }
    }

    public class TagsListResponse
    {
        [JsonProperty("tags")]
        public IEnumerable<TagEntity> Tags { get; set; } = Enumerable.Empty<TagEntity>();
    }
}
