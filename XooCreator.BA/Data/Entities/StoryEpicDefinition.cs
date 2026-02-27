namespace XooCreator.BA.Data;

using XooCreator.BA.Data.Entities;

/// <summary>
/// Represents a story epic definition (published version) in the system
/// Similar to StoryDefinition but for epics
/// </summary>
public class StoryEpicDefinition
{
    public string Id { get; set; } = string.Empty; // e.g., "my-epic-v1"
    public string Name { get; set; } = string.Empty; // e.g., "My Epic"
    public string? Description { get; set; }
    public Guid OwnerUserId { get; set; }
    public string Status { get; set; } = "published"; // published, unpublished
    public string? CoverImageUrl { get; set; } // Background image for tree logic view
    public string? MarketplaceCoverImageUrl { get; set; } // Cover image shown in marketplace (separate from tree background)
    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true; // For soft delete / unpublish
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? PublishedAtUtc { get; set; }
    
    // Versioning fields (similar to StoryDefinition)
    public int Version { get; set; } = 1; // Global version, increments on publish
    public int BaseVersion { get; set; } = 0; // The published version from which this draft originated (for re-publish)
    public int LastPublishedVersion { get; set; } = 0; // Draft version that produced the current publish

    /// <summary>Language codes that have audio support for this epic (e.g. ["ro-ro", "en-us"]).</summary>
    public List<string> AudioLanguages { get; set; } = new();

    // Navigation properties
    public AlchimaliaUser Owner { get; set; } = null!;
    public ICollection<StoryEpicDefinitionRegion> Regions { get; set; } = new List<StoryEpicDefinitionRegion>();
    public ICollection<StoryEpicDefinitionStoryNode> StoryNodes { get; set; } = new List<StoryEpicDefinitionStoryNode>();
    public ICollection<StoryEpicDefinitionUnlockRule> UnlockRules { get; set; } = new List<StoryEpicDefinitionUnlockRule>();
    public ICollection<StoryEpicDefinitionTranslation> Translations { get; set; } = new List<StoryEpicDefinitionTranslation>();
    public ICollection<StoryEpicDefinitionTopic> Topics { get; set; } = new List<StoryEpicDefinitionTopic>();
    public ICollection<StoryEpicDefinitionAgeGroup> AgeGroups { get; set; } = new List<StoryEpicDefinitionAgeGroup>();
    public ICollection<StoryEpicDefinitionCoAuthor> CoAuthors { get; set; } = new List<StoryEpicDefinitionCoAuthor>();
}

