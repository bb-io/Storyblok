namespace Apps.Storyblok.Models.Request.Component;

public class ComponentPayload
{
    public string Name { get; set; }

    public string? DisplayName { get; set; }

    public string? Image { get; set; }

    public string? Preview { get; set; }

    public bool? IsRoot { get; set; }

    public bool? IsNestable { get; set; }

    public string? ComponentGroupUuid { get; set; }
}