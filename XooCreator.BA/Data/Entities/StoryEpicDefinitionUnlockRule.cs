namespace XooCreator.BA.Data;

public class StoryEpicDefinitionUnlockRule
{
    public int Id { get; set; }
    public string EpicId { get; set; } = string.Empty; // FK to StoryEpicDefinition
    public string Type { get; set; } = string.Empty; // "story", "all", "any"
    public string FromId { get; set; } = string.Empty; // region ID sau story ID
    public string ToRegionId { get; set; } = string.Empty; // target region ID
    public string? ToStoryId { get; set; } // target story ID (optional; when set, ToRegionId should be empty)
    public string? RequiredStoriesCsv { get; set; } // "story1,story2,story3"
    public int? MinCount { get; set; } // pentru type=any
    public string? StoryId { get; set; } // pentru type=story
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicDefinition Epic { get; set; } = null!;

    // Note: No FK relationships to avoid mixed ID type issues
    // Validation will be done in application code
}

