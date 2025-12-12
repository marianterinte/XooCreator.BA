namespace XooCreator.BA.Data;

public class DbStoryEpic
{
    public string Id { get; set; } = string.Empty; // e.g., "my-epic-v1"
    public string Name { get; set; } = string.Empty; // e.g., "My Epic"
    public string? Description { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Status { get; set; } = "draft"; // draft, sent_for_approval, in_review, approved, changes_requested, published, archived
    public string? CoverImageUrl { get; set; } // Background image for tree logic view
    public bool IsDefault { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; set; }
    
    // Versioning fields (similar to StoryDefinition)
    public int Version { get; set; } = 0; // Published version, increments on each publish
    public int BaseVersion { get; set; } = 0; // The published version from which this draft originated (for re-publish)
    public int LastDraftVersion { get; set; } = 0; // Draft version counter (for tracking changes)
    
    // Review workflow fields (similar to StoryCraft)
    public Guid? AssignedReviewerUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ReviewEndedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    
    // Navigation properties
    public AlchimaliaUser Owner { get; set; } = null!;
    public ICollection<StoryEpicRegion> Regions { get; set; } = new List<StoryEpicRegion>();
    public ICollection<StoryEpicStoryNode> StoryNodes { get; set; } = new List<StoryEpicStoryNode>();
    public ICollection<StoryEpicUnlockRule> UnlockRules { get; set; } = new List<StoryEpicUnlockRule>();
}

