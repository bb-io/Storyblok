namespace Apps.Storyblok.Models.Request.Story;

public class UpdateStoryRequest
{
    public UpdateStoryPayload Story { get; set; }

    public UpdateStoryRequest(LanguageRequest input)
    {
        Story = new()
        {
            Lang = input.Lang
        };
    }
}