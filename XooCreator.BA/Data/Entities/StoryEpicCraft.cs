namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story epic craft (draft/working copy) in the editor
/// Similar to StoryCraft but for epics
/// </summary>
public class StoryEpicCraft
{
    public string Id { get; set; } = string.Empty; // e.g., "my-epic-v1"
    public string Name { get; set; } = string.Empty; // e.g., "My Epic"
    public string? Description { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Status { get; set; } = "draft"; // draft, sent_for_approval, in_review, approved, changes_requested, archived
    public string? CoverImageUrl { get; set; } // Background image for tree logic view
    public bool IsDefault { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Versioning reference: the published version from which this draft originated
    public int BaseVersion { get; set; } = 0;
    
    // Incremental counter for changes performed in the editor
    public int LastDraftVersion { get; set; } = 0;
    
    // Review workflow fields (similar to StoryCraft)
    public Guid? AssignedReviewerUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ReviewEndedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    
    // Navigation properties
    public AlchimaliaUser Owner { get; set; } = null!;
    public ICollection<StoryEpicCraftRegion> Regions { get; set; } = new List<StoryEpicCraftRegion>();
    public ICollection<StoryEpicCraftStoryNode> StoryNodes { get; set; } = new List<StoryEpicCraftStoryNode>();
    public ICollection<StoryEpicCraftUnlockRule> UnlockRules { get; set; } = new List<StoryEpicCraftUnlockRule>();
    public ICollection<StoryEpicCraftTranslation> Translations { get; set; } = new List<StoryEpicCraftTranslation>();
    public ICollection<StoryEpicCraftHeroReference> HeroReferences { get; set; } = new List<StoryEpicCraftHeroReference>();
}

