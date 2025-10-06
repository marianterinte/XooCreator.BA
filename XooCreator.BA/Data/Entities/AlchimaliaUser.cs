namespace XooCreator.BA.Data;

public class AlchimaliaUser
{
    public Guid Id { get; set; }
    public string Auth0Id { get; set; } = string.Empty;  // Auth0 sub claim (e.g., "auth0|1234567890")
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Picture { get; set; }  // From Auth0 profile (optional)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool HasVisitedImaginationLaboratory { get; set; }
}
