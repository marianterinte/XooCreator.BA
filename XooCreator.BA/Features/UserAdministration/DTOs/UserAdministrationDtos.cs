using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.UserAdministration.DTOs;

public class UserDto
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }  // Kept for backward compatibility
    public List<UserRole> Roles { get; set; } = new();  // Multiple roles support
}

public class GetAllUsersResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
    public List<UserDto> Users { get; set; } = new();
}

public class UpdateUserRoleRequest
{
    public UserRole Role { get; set; }  // Kept for backward compatibility
    public List<UserRole>? Roles { get; set; }  // Multiple roles support (takes precedence if provided)
}

public class UpdateUserRoleResponse
{
    public bool Success { get; set; }
    public string? ErrorMessage { get; set; }
}

public class CurrentUserResponse
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public UserRole Role { get; set; }  // Kept for backward compatibility
    public List<UserRole> Roles { get; set; } = new();  // Multiple roles support
}

