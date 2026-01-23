using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Translation for StoryCreatorsChallenge (Topic per language)
/// </summary>
public class StoryCreatorsChallengeTranslation
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string ChallengeId { get; set; }
    
    [MaxLength(10)]
    public required string LanguageCode { get; set; } = "ro-ro";
    
    [MaxLength(500)]
    public required string Topic { get; set; } = string.Empty;
    
    public string? Description { get; set; } // General description of the challenge
    
    // Navigation property
    public StoryCreatorsChallenge Challenge { get; set; } = null!;
}
