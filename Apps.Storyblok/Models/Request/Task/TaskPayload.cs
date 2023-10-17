namespace Apps.Storyblok.Models.Request.Task;

public class TaskPayload
{
    public string Name { get; set; }

    public string? Description { get; set; }

    public string WebhookUrl { get; set; }
}