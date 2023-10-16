namespace Apps.Storyblok.Models.Response.Pagination.Base;

public class PaginationResponse<T>
{
    public virtual T[] Items { get; set; }
}