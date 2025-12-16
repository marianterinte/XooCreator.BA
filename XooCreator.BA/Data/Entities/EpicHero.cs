using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents an independent hero for epics that can be reused across multiple epics
/// Similar to StoryCraft lifecycle: draft → in_review → approved → published
/// Note: Named EpicHero to avoid conflict with existing StoryHero entity used for Tree of Heroes
/// </summary>
public class EpicHero
{
    [MaxLength(100)]
    public required string Id { get; set; } = string.Empty; // e.g., "hero-arthur-20250115", auto-generated
    
    [MaxLength(200)]
    public required string Name { get; set; } = string.Empty; // Label/Display name
    
    public string? ImageUrl { get; set; } // Poza uploadată (Azure Blob path)
    
    public string? GreetingText { get; set; } // Mesaj de salut text
    
    public string? GreetingAudioUrl { get; set; } // Mesaj audio URL (Azure Blob path)
    
    public Guid OwnerUserId { get; set; }
    
    [MaxLength(20)]
    public required string Status { get; set; } = "draft"; // draft, in_review, approved, published, archived
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; set; }
    
    // Review workflow fields (similar to StoryCraft)
    public Guid? AssignedReviewerUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ReviewEndedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    
    // Navigation properties
    public AlchimaliaUser Owner { get; set; } = null!;
    public ICollection<StoryEpicHeroReference> EpicReferences { get; set; } = new List<StoryEpicHeroReference>();
    public ICollection<EpicHeroTranslation> Translations { get; set; } = new List<EpicHeroTranslation>();
}

