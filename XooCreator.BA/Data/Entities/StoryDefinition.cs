namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story definition in the system
/// </summary>
public class StoryDefinition
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty; // e.g., "root-s1", "intro-pufpuf"
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Category { get; set; } = string.Empty; // e.g., "main", "intro", "special"
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public List<StoryTile> Tiles { get; set; } = new();
    public List<StoryDefinitionTranslation> Translations { get; set; } = new();
}

public class StoryDefinitionTranslation
{
    public Guid Id { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case
    public string Title { get; set; } = string.Empty;

    public StoryDefinition StoryDefinition { get; set; } = null!;
}
