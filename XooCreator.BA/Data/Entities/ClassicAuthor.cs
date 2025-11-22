namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Represents a classic/well-known author for classic stories
/// </summary>
public class ClassicAuthor
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Unique identifier for the author (e.g., "ion-creanga", "lewis-carroll")
    /// </summary>
    [System.ComponentModel.DataAnnotations.MaxLength(100)]
    public required string AuthorId { get; set; }
    
    /// <summary>
    /// Author's full name
    /// </summary>
    [System.ComponentModel.DataAnnotations.MaxLength(200)]
    public required string Name { get; set; }
    
    /// <summary>
    /// Language code for this author (e.g., "ro-ro", "en-us", "hu-hu")
    /// </summary>
    [System.ComponentModel.DataAnnotations.MaxLength(10)]
    public required string LanguageCode { get; set; }
    
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public List<StoryDefinition> StoryDefinitions { get; set; } = new();
}

