namespace Apps.Storyblok.Models.Request.Component;

public class CreateComponentRequest
{
    public ComponentPayload Component { get; set; }

    public CreateComponentRequest(CreateComponentInput input)
    {
        Component = new()
        {
            Name = input.Name,
            DisplayName = input.DisplayName,
            Image = input.Image,
            Preview = input.Preview,
            IsRoot = input.IsRoot,
            IsNestable = input.IsNestable,
            ComponentGroupUuid = input.ComponentGroupUuid,
        };
    }
}