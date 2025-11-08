using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

public class AlchimaliaUser
{
    public Guid Id { get; set; }
    public string Auth0Id { get; set; } = string.Empty;  // Auth0 sub claim (e.g., "auth0|1234567890")
    public string Email { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;  // Kept for backward compatibility
    public string FirstName { get; set; } = string.Empty;  // Nume
    public string LastName { get; set; } = string.Empty;  // Prenume
    public UserRole Role { get; set; } = UserRole.Reader;  // Kept for backward compatibility, will be deprecated
    public List<UserRole> Roles { get; set; } = new() { UserRole.Reader };  // Multiple roles support
    public string? Picture { get; set; }  // From Auth0 profile (optional)
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastLoginAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public bool HasVisitedImaginationLaboratory { get; set; }
}
