using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents an epic hero craft (draft/working copy) in the editor
/// Similar to StoryEpicCraft but for heroes
/// </summary>
public class EpicHeroCraft
{
    [MaxLength(100)]
    public required string Id { get; set; } = string.Empty; // e.g., "hero-arthur-20250115"
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Label/Display name
    
    public string? ImageUrl { get; set; } // Poza uploadată (Azure Blob path)
    
    public Guid OwnerUserId { get; set; }
    
    [MaxLength(20)]
    public required string Status { get; set; } = "draft"; // draft, sent_for_approval, in_review, approved, changes_requested, archived
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Versioning reference: the published version from which this draft originated
    public int BaseVersion { get; set; } = 0;
    
    // Incremental counter for changes performed in the editor
    public int LastDraftVersion { get; set; } = 0;
    
    // Review workflow fields (similar to StoryEpicCraft)
    public Guid? AssignedReviewerUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ReviewEndedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    
    // Navigation properties
    public AlchimaliaUser Owner { get; set; } = null!;
    public ICollection<EpicHeroCraftTranslation> Translations { get; set; } = new List<EpicHeroCraftTranslation>();
    public ICollection<EpicHeroCraftTopic> Topics { get; set; } = new List<EpicHeroCraftTopic>();
    public ICollection<EpicHeroCraftRegion> Regions { get; set; } = new List<EpicHeroCraftRegion>();
}

