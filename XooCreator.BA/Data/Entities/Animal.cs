using XooCreator.BA.Data.Enums;

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

    // Workflow fields
    public string Status { get; set; } = AlchimaliaUniverseStatus.Draft.ToDb();
    public Guid? CreatedByUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public int Version { get; set; } = 1;
    public Guid? ParentVersionId { get; set; } // FK către Animals (pentru versioning)
    
    public DateTime? CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }

    public ICollection<AnimalPartSupport> SupportedParts { get; set; } = new List<AnimalPartSupport>();
    public List<AnimalTranslation> Translations { get; set; } = new();
}

public class AnimalTranslation
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro";
    public string Label { get; set; } = string.Empty;
    public Animal Animal { get; set; } = null!;
}
