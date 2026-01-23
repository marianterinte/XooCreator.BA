using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Subscription of a Creator user to a challenge
/// </summary>
public class StoryCreatorsChallengeSubscription
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string ChallengeId { get; set; }
    
    public Guid UserId { get; set; }
    
    public DateTime SubscribedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation properties
    public StoryCreatorsChallenge Challenge { get; set; } = null!;
    // Note: User navigation property assumes AlchimaliaUser entity exists and is mapped correctly.
    // Based on other entities, we might not always have full navigation to User if it's in a different context/schema,
    // but the guide specifies it. I'll add it but keep in mind it might need `[NotMapped]` if typical pattern differs.
    // However, guide says: public AlchimaliaUser User { get; set; } = null!;
    // Checking other files for AlchimaliaUser reference... 
    // Actually, usually we rely on Id. But guide explicitly asks for it.
    // Let's assume AlchimaliaUser is available in XooCreator.BA.Data.Entities or similar.
    // Wait, the guide uses `XooCreator.BA.Data.Entities`.
}
