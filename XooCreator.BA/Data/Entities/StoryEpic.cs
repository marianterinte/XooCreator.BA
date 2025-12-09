namespace XooCreator.BA.Data;

public class StoryEpic
{
    public string Id { get; set; } = string.Empty; // e.g., "my-epic-v1"
    public string Name { get; set; } = string.Empty; // e.g., "My Epic"
    public string? Description { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Status { get; set; } = "draft"; // draft, published
    public bool IsDefault { get; set; } = false;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public AlchimaliaUser Owner { get; set; } = null!;
    public ICollection<StoryEpicRegion> Regions { get; set; } = new List<StoryEpicRegion>();
    public ICollection<StoryEpicStoryNode> StoryNodes { get; set; } = new List<StoryEpicStoryNode>();
    public ICollection<StoryEpicUnlockRule> UnlockRules { get; set; } = new List<StoryEpicUnlockRule>();
}

