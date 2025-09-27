namespace XooCreator.BA.Data;

/// <summary>
/// Represents a tile (page or quiz) within a story
/// </summary>
public class StoryTile
{
    public Guid Id { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public string TileId { get; set; } = string.Empty; // e.g., "p1", "q1"
    public string Type { get; set; } = string.Empty; // "page" or "quiz"
    public int SortOrder { get; set; }
    
    // Page-specific fields
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    
    // Quiz-specific fields
    public string? Question { get; set; }
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public StoryDefinition StoryDefinition { get; set; } = null!;
    public List<StoryAnswer> Answers { get; set; } = new();
    public List<StoryTileTranslation> Translations { get; set; } = new();
}

public class StoryTileTranslation
{
    public Guid Id { get; set; }
    public Guid StoryTileId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro";
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? Question { get; set; }

    public StoryTile StoryTile { get; set; } = null!;
}
