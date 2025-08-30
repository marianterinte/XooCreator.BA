namespace XooCreator.BA.Data;

public class Animal
{
    public Guid Id { get; set; }
    public string Label { get; set; } = string.Empty; // e.g., Bunny
    public string Src { get; set; } = string.Empty;   // image url

    public bool IsHybrid { get; set; } // false = root, true = hybrid

    // Region relation
    public Guid RegionId { get; set; }
    public Region Region { get; set; } = null!;

    public ICollection<AnimalPartSupport> SupportedParts { get; set; } = new List<AnimalPartSupport>();
}
