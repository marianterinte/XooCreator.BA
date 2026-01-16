using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story hero (eroi din povești) in Alchimalia Universe
/// </summary>
public class StoryHero
{
    public Guid Id { get; set; }

    [MaxLength(100)]
    public string HeroId { get; set; } = string.Empty; // e.g., "puf-puf", "linkaro", "grubot"

    [MaxLength(500)]
    public string ImageUrl { get; set; } = string.Empty;

    // Unlock conditions (stored as JSON)
    [MaxLength(2000)]
    public string UnlockConditionsJson { get; set; } = string.Empty; // JSON: { "type": "story_completion", "requiredStories": [...], "minProgress": 100 }

    // Workflow fields
    [MaxLength(50)]
    public string Status { get; set; } = AlchimaliaUniverseStatus.Draft.ToDb();
    public Guid? CreatedByUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    [MaxLength(2000)]
    public string? ReviewNotes { get; set; }
    public int Version { get; set; } = 1;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Navigation
    public List<StoryHeroTranslation> Translations { get; set; } = new();
}

/// <summary>
/// Version snapshot for StoryHero
/// </summary>
public class StoryHeroVersion
{
    public Guid Id { get; set; }
    public Guid StoryHeroId { get; set; }
    public int Version { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? SnapshotJson { get; set; } // JSON snapshot al versiunii

    public StoryHero StoryHero { get; set; } = null!;
}
