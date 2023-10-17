using Apps.Storyblok.Models.Entities;
using Apps.Storyblok.Models.Response.Pagination.Base;
using Newtonsoft.Json;

namespace Apps.Storyblok.Models.Response.Pagination;

public class TasksPaginationResponse : PaginationResponse<TaskEntity>
{
    [JsonProperty("tasks")] public override TaskEntity[] Items { get; set; }
}