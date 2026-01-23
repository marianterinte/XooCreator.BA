using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Main entity for Story Creators Challenge
/// </summary>
public class StoryCreatorsChallenge
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string ChallengeId { get; set; } // e.g., "ccc-2025-01"
    
    [MaxLength(20)]
    public string Status { get; set; } = "active"; // 'active', 'draft', 'archived'
    
    public int SortOrder { get; set; } = 0;
    
    public DateTime? EndDate { get; set; } // End date of the challenge
    
    [MaxLength(500)]
    public string? CoverImageUrl { get; set; } // Challenge cover image URL (Azure Blob Storage)
    
    [MaxLength(500)]
    public string? CoverImageRelPath { get; set; } // Relative path in blob storage
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public Guid? CreatedByUserId { get; set; }
    public Guid? UpdatedByUserId { get; set; }
    
    // Navigation properties
    public ICollection<StoryCreatorsChallengeTranslation> Translations { get; set; } = new List<StoryCreatorsChallengeTranslation>();
    public ICollection<StoryCreatorsChallengeItem> Items { get; set; } = new List<StoryCreatorsChallengeItem>();
}
