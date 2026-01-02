using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.UserAdministration.DTOs;
using XooCreator.BA.Features.UserAdministration.Repositories;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.UserAdministration.Services;

public interface IUserAdministrationService
{
    Task<GetAllUsersResponse> GetAllUsersAsync(CancellationToken ct = default);
    Task<UpdateUserRoleResponse> UpdateUserRoleAsync(Guid userId, UserRole role, CancellationToken ct = default);
    Task<UpdateUserRoleResponse> UpdateUserRolesAsync(Guid userId, List<UserRole> roles, CancellationToken ct = default);
    Task<CurrentUserResponse?> GetCurrentUserAsync(CancellationToken ct = default);
}

public class UserAdministrationService : IUserAdministrationService
{
    private readonly IUserAdministrationRepository _repository;
    private readonly IAuth0UserService _auth0UserService;

    public UserAdministrationService(IUserAdministrationRepository repository, IAuth0UserService auth0UserService)
    {
        _repository = repository;
        _auth0UserService = auth0UserService;
    }

    public async Task<GetAllUsersResponse> GetAllUsersAsync(CancellationToken ct = default)
    {
        try
        {
            var users = await _repository.GetAllUsersAsync(ct);
            var userDtos = users.Select(u => new UserDto
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email,
                Role = u.Role,  // Backward compatibility
                Roles = u.Roles ?? (u.Role != 0 ? new List<UserRole> { u.Role } : new List<UserRole> { UserRole.Reader })
            }).ToList();

            return new GetAllUsersResponse
            {
                Success = true,
                Users = userDtos
            };
        }
        catch (Exception ex)
        {
            return new GetAllUsersResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<UpdateUserRoleResponse> UpdateUserRoleAsync(Guid userId, UserRole role, CancellationToken ct = default)
    {
        // For backward compatibility, convert single role to list
        return await UpdateUserRolesAsync(userId, new List<UserRole> { role }, ct);
    }

    public async Task<UpdateUserRoleResponse> UpdateUserRolesAsync(Guid userId, List<UserRole> roles, CancellationToken ct = default)
    {
        try
        {
            var user = await _repository.GetUserByIdAsync(userId, ct);
            if (user == null)
            {
                return new UpdateUserRoleResponse
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            var success = await _repository.UpdateUserRolesAsync(userId, roles, ct);
            
            // Invalidate cache for this user to ensure fresh data on next request
            if (success && !string.IsNullOrEmpty(user.Auth0Id))
            {
                _auth0UserService.InvalidateUserCache(user.Auth0Id);
            }
            
            return new UpdateUserRoleResponse
            {
                Success = success,
                ErrorMessage = success ? null : "Failed to update user roles"
            };
        }
        catch (Exception ex)
        {
            return new UpdateUserRoleResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<CurrentUserResponse?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        try
        {
            var user = await _auth0UserService.GetCurrentUserAsync(ct);
            if (user == null)
            {
                return null;
            }

            return new CurrentUserResponse
            {
                Id = user.Id,
                Email = user.Email,
                Role = user.Role,  // Backward compatibility
                Roles = user.Roles ?? (user.Role != 0 ? new List<UserRole> { user.Role } : new List<UserRole> { UserRole.Reader })
            };
        }
        catch (Exception)
        {
            return null;
        }
    }
}

