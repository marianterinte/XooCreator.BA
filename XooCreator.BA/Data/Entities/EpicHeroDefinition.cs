using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents an epic hero definition (published version) in the system
/// Similar to StoryEpicDefinition but for heroes
/// </summary>
public class EpicHeroDefinition
{
    [MaxLength(100)]
    public required string Id { get; set; } = string.Empty; // e.g., "hero-arthur-20250115"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Label/Display name
    
    public string? ImageUrl { get; set; } // Poza uploadată (Azure Blob path)
    
    public Guid OwnerUserId { get; set; }
    
    [MaxLength(20)]
    public required string Status { get; set; } = "published"; // published, unpublished
    
    public bool IsActive { get; set; } = true; // For soft delete / unpublish
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; set; }
    
    // Versioning fields (similar to StoryEpicDefinition)
    public int Version { get; set; } = 1; // Global version, increments on publish
    public int BaseVersion { get; set; } = 0; // The published version from which this draft originated (for re-publish)
    public int LastPublishedVersion { get; set; } = 0; // Draft version that produced the current publish
    
    // Navigation properties
    public AlchimaliaUser Owner { get; set; } = null!;
    public ICollection<EpicHeroDefinitionTranslation> Translations { get; set; } = new List<EpicHeroDefinitionTranslation>();
    public ICollection<EpicHeroDefinitionTopic> Topics { get; set; } = new List<EpicHeroDefinitionTopic>();
}

