using XooCreator.BA.Features.TreeOfLight;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents a quiz answer option
/// </summary>
public class StoryAnswer
{
    public Guid Id { get; set; }
    public Guid StoryTileId { get; set; }
    public string AnswerId { get; set; } = string.Empty; // e.g., "a", "b", "c"
    public string Text { get; set; } = string.Empty;
    public string? TokensJson { get; set; } // JSON serialized list of TokenReward objects
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public StoryTile StoryTile { get; set; } = null!;
}
