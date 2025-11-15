namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Represents a story topic/tag that can be assigned to stories
/// Topics are organized by dimensions (educational, fun, emotional_depth, etc.)
/// </summary>
public class StoryTopic
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Unique identifier for the topic (e.g., "edu_math", "fun_adventure")
    /// </summary>
    [System.ComponentModel.DataAnnotations.MaxLength(100)]
    public required string TopicId { get; set; }
    
    /// <summary>
    /// Dimension/category this topic belongs to (e.g., "educational", "fun")
    /// </summary>
    [System.ComponentModel.DataAnnotations.MaxLength(50)]
    public required string DimensionId { get; set; }
    
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public List<StoryTopicTranslation> Translations { get; set; } = new();
    public List<StoryCraftTopic> StoryCrafts { get; set; } = new();
    public List<StoryDefinitionTopic> StoryDefinitions { get; set; } = new();
}

/// <summary>
/// Translation for StoryTopic (label per language)
/// </summary>
public class StoryTopicTranslation
{
    public Guid Id { get; set; }
    public Guid StoryTopicId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case
    public string Label { get; set; } = string.Empty;
    
    public StoryTopic StoryTopic { get; set; } = null!;
}

/// <summary>
/// Many-to-many relationship between StoryCraft and StoryTopic
/// </summary>
public class StoryCraftTopic
{
    public Guid StoryCraftId { get; set; }
    public Guid StoryTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryCraft StoryCraft { get; set; } = null!;
    public StoryTopic StoryTopic { get; set; } = null!;
}

/// <summary>
/// Many-to-many relationship between StoryDefinition and StoryTopic
/// </summary>
public class StoryDefinitionTopic
{
    public Guid StoryDefinitionId { get; set; }
    public Guid StoryTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryDefinition StoryDefinition { get; set; } = null!;
    public StoryTopic StoryTopic { get; set; } = null!;
}

