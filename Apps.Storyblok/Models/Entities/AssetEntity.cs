namespace Apps.Storyblok.Models.Entities;
public class AssetEntity
{
    public long? Id { get; set; }
    public string Alt { get; set; }
    public string Name { get; set; }
    public string Focus { get; set; }
    public string Title { get; set; }
    public string Source { get; set; }
    public string Filename { get; set; }
    public string Copyright { get; set; }
    public string Fieldtype { get; set; }
    public bool IsExternalUrl { get; set; }
}

