using Blackbird.Applications.Sdk.Common;

namespace Apps.Storyblok.Models.Entities;

public class ComponentEntity
{
    [Display("Component ID")] public string Id { get; set; }

    public string Name { get; set; }

    [Display("Display name")] public string DisplayName { get; set; }

    [Display("Created at")] public DateTime CreatedAt { get; set; }

    public string? Image { get; set; }

    [Display("Preview field")] public string? PreviewField { get; set; }

    [Display("Is root")] public bool IsRoot { get; set; }

    [Display("Is nestable")] public bool IsNestable { get; set; }

    [Display("Component group UUID")] public string? ComponentGroupUuid { get; set; }

    [DefinitionIgnore] public object Schema { get; set; }
}