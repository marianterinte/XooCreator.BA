

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
    public bool IsCorrect { get; set; } = false; // True if this is the correct answer for the quiz
    public string? TokensJson { get; set; } // Deprecated: will be replaced by relation
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public StoryTile StoryTile { get; set; } = null!;
    public List<StoryAnswerToken> Tokens { get; set; } = new();
    public List<StoryAnswerTranslation> Translations { get; set; } = new();
}
