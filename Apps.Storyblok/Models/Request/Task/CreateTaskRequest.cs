namespace Apps.Storyblok.Models.Request.Task;

public class CreateTaskRequest
{
    public TaskPayload Task { get; set; }

    public CreateTaskRequest(CreateTaskInput input)
    {
        Task = new()
        {
            Name = input.Name,
            Description = input.Description,
            WebhookUrl = input.WebhookUrl,
        };
    }
}