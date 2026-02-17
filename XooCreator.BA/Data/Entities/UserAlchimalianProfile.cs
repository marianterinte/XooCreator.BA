namespace XooCreator.BA.Data;

/// <summary>
/// Per-user profile for Alchimalian Hero page: selected hero (display + profile picture).
/// Stored separately from Users so profile data is not always loaded with user.
/// </summary>
public class UserAlchimalianProfile
{
    public Guid UserId { get; set; }
    /// <summary>Hero definition ID (e.g. hero_wise_owl). Null = no selection.</summary>
    public string? SelectedHeroId { get; set; }
    /// <summary>Hero IDs the user has "discovered" (tapped Descoperă / Vezi ce erou poți deveni). Stored as JSON array.</summary>
    public string DiscoveredHeroIdsJson { get; set; } = "[]";
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    public AlchimaliaUser User { get; set; } = null!;
}
