using Blackbird.Applications.Sdk.Common;
using Newtonsoft.Json;

namespace Apps.Storyblok.Models.Request.Story;

public class ListStoriesRequest
{
    [Display("Components")]
    [JsonProperty("contain_component")]
    public string? ContainComponent { get; set; }

    [Display("Text search")]
    [JsonProperty("text_search")]
    public string? TextSearch { get; set; }

    [Display("Sort by")]
    [JsonProperty("sort_by")]
    public string? SortBy { get; set; }

    [Display("Is pinned")]
    [JsonProperty("pinned")]
    public bool? Pinned { get; set; }

    [Display("Excluding story IDs")]
    [JsonProperty("excluding_ids")]
    public string? ExcludingIds { get; set; }

    [Display("IDs")]
    [JsonProperty("by_ids")]
    public string? ByIds { get; set; }

    [Display("UUIDs")]
    [JsonProperty("by_uuids")]
    public string? ByUuids { get; set; }

    [Display("Tags", Description = "One or more tags. Matches stories that have ANY of these tags (OR).")]
    [JsonIgnore]
    public IEnumerable<string>? Tags { get; set; }

    [JsonProperty("with_tag")]
    [DefinitionIgnore]
    public string? WithTag =>
        Tags == null
            ? null
            : string.Join(",", 
                Tags
                    .Where(t => !string.IsNullOrWhiteSpace(t))
                    .Select(t => t.Trim())
                    .Distinct()
            );

    [Display("Folders only")]
    [JsonProperty("folder_only")]
    public bool? FolderOnly { get; set; }

    [Display("Stories only")]
    [JsonProperty("story_only")]
    public bool? StoryOnly { get; set; }

    [Display("Parent ID")]
    [JsonProperty("with_parent")]
    public string? WithParent { get; set; }

    [Display("Slug")]
    [JsonProperty("with_slug")]
    public string? WithSlug { get; set; }

    [Display("Starts with")]
    [JsonProperty("starts_with")]
    public string? StartsWith { get; set; }

    [Display("In trash folder")]
    [JsonProperty("in_trash")]
    public bool? InTrash { get; set; }

    [Display("Search term")]
    [JsonProperty("search")]
    public string? Search { get; set; }

    [Display("Filter query")]
    [JsonProperty("filter_query")]
    public string? FilterQuery { get; set; }

    [Display("Release ID")]
    [JsonProperty("in_release")]
    public string? InRelease { get; set; }

    [Display("Is published")]
    [JsonProperty("is_published")]
    public bool? IsPublished { get; set; }
}