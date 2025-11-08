namespace XooCreator.BA.Data.Enums;

/// <summary>
/// Represents the role of a user in the system
/// </summary>
public enum UserRole
{
    /// <summary>
    /// Reader - basic user with read-only access
    /// </summary>
    Reader = 0,
    
    /// <summary>
    /// Creator - user with content creation permissions
    /// </summary>
    Creator = 1,
    
    /// <summary>
    /// Reviewer - user with review permissions (claim/approve/reject)
    /// </summary>
    Reviewer = 3,

    /// <summary>
    /// Admin - administrator with full system access
    /// </summary>
    Admin = 2
}

