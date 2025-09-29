namespace XooCreator.BA.Data;

public class TreeUnlockRule
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty; // "story", "all", "any"
    public string FromId { get; set; } = string.Empty; // region ID sau story ID (pentru type=story)
    public string ToRegionId { get; set; } = string.Empty; // target region ID
    public string? RequiredStoriesCsv { get; set; } // pentru type=all/any: "trunk-s1,trunk-s2,trunk-s3"
    public int? MinCount { get; set; } // pentru type=any
    public string? StoryId { get; set; } // pentru type=story
    public int SortOrder { get; set; }
    public string TreeConfigurationId { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public TreeConfiguration TreeConfiguration { get; set; } = null!;

    // Note: No FK relationships to avoid mixed ID type issues
    // Validation will be done in application code
}
