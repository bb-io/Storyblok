using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Response.Pagination.Base;
using Newtonsoft.Json;

namespace Apps.Storyblok.Models.Response.Pagination;

public class StoriesPaginationResponse : PaginationResponse<StoryEntity>
{
    [JsonProperty("stories")] public override StoryEntity[] Items { get; set; }
}