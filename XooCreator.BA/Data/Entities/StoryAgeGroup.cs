namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Represents an age group for stories (e.g., preschool, early school, etc.)
/// </summary>
public class StoryAgeGroup
{
    public Guid Id { get; set; }
    
    /// <summary>
    /// Unique identifier for the age group (e.g., "preschool_3_5", "early_school_6_8")
    /// </summary>
    [System.ComponentModel.DataAnnotations.MaxLength(100)]
    public required string AgeGroupId { get; set; }
    
    public int MinAge { get; set; }
    public int MaxAge { get; set; }
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public List<StoryAgeGroupTranslation> Translations { get; set; } = new();
    public List<StoryCraftAgeGroup> StoryCrafts { get; set; } = new();
    public List<StoryDefinitionAgeGroup> StoryDefinitions { get; set; } = new();
}

/// <summary>
/// Translation for StoryAgeGroup (label and description per language)
/// </summary>
public class StoryAgeGroupTranslation
{
    public Guid Id { get; set; }
    public Guid StoryAgeGroupId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case
    public string Label { get; set; } = string.Empty;
    public string? Description { get; set; }
    
    public StoryAgeGroup StoryAgeGroup { get; set; } = null!;
}

/// <summary>
/// Many-to-many relationship between StoryCraft and StoryAgeGroup
/// </summary>
public class StoryCraftAgeGroup
{
    public Guid StoryCraftId { get; set; }
    public Guid StoryAgeGroupId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryCraft StoryCraft { get; set; } = null!;
    public StoryAgeGroup StoryAgeGroup { get; set; } = null!;
}

/// <summary>
/// Many-to-many relationship between StoryDefinition and StoryAgeGroup
/// </summary>
public class StoryDefinitionAgeGroup
{
    public Guid StoryDefinitionId { get; set; }
    public Guid StoryAgeGroupId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryDefinition StoryDefinition { get; set; } = null!;
    public StoryAgeGroup StoryAgeGroup { get; set; } = null!;
}

