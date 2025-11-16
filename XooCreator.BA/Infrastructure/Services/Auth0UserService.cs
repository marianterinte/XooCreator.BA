using System.Linq;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data.Repositories;

namespace XooCreator.BA.Infrastructure.Services;

public interface IAuth0UserService
{
    Task<AlchimaliaUser?> GetCurrentUserAsync(CancellationToken ct = default);
    Task<Guid?> GetCurrentUserIdAsync(CancellationToken ct = default);
    bool HasRole(AlchimaliaUser user, UserRole role);
    bool HasAnyRole(AlchimaliaUser user, params UserRole[] roles);
}

public class Auth0UserService : IAuth0UserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IUserRepository _userRepository;
    private AlchimaliaUser? _cachedUser;

    public Auth0UserService(IHttpContextAccessor httpContextAccessor, IUserRepository userRepository)
    {
        _httpContextAccessor = httpContextAccessor;
        _userRepository = userRepository;
    }

    public async Task<AlchimaliaUser?> GetCurrentUserAsync(CancellationToken ct = default)
    {
        if (_cachedUser != null)
            return _cachedUser;

        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return null;

        var auth0Id = GetAuth0IdFromClaims(httpContext.User);
        if (string.IsNullOrEmpty(auth0Id))
            return null;

        var email = GetClaimValue(httpContext.User, "email");
        var name = GetClaimValue(httpContext.User, "name") ?? GetClaimValue(httpContext.User, "nickname");
        var picture = GetClaimValue(httpContext.User, "picture");

        // Access tokens often don't include profile claims. Fall back to sensible defaults.
        if (string.IsNullOrWhiteSpace(name)) name = "Unknown";
        if (string.IsNullOrWhiteSpace(email)) email = $"{auth0Id.Replace("|", "+") }@unknown.local";

        // Ensure user exists and sync profile data
        _cachedUser = await _userRepository.EnsureAsync(auth0Id, name, email, picture, ct);
        return _cachedUser;
    }

    public async Task<Guid?> GetCurrentUserIdAsync(CancellationToken ct = default)
    {
        var user = await GetCurrentUserAsync(ct);
        return user?.Id;
    }

    private static string? GetAuth0IdFromClaims(ClaimsPrincipal user)
    {
        // Auth0 sub claim format: "auth0|1234567890" or "google-oauth2|1234567890"
        return GetClaimValue(user, "sub");
    }

    private static string? GetClaimValue(ClaimsPrincipal user, string claimType)
    {
        return user.FindFirst(claimType)?.Value;
    }

    /// <summary>
    /// Checks if a user has a specific role, considering multiple roles support
    /// </summary>
    public bool HasRole(AlchimaliaUser user, UserRole role)
    {
        // Use roles array if available, otherwise fall back to single role
        var roles = user.Roles != null && user.Roles.Count > 0
            ? user.Roles
            : (user.Role != UserRole.Reader ? new List<UserRole> { user.Role } : new List<UserRole> { UserRole.Reader });

        // Check if user has the role in their roles array
        if (roles.Contains(role)) return true;

        // Admin has all permissions
        if (roles.Contains(UserRole.Admin)) return true;

        // PremiumCreator has all Creator permissions plus premium features
        if (roles.Contains(UserRole.PremiumCreator) && (role == UserRole.Creator || role == UserRole.PremiumCreator || role == UserRole.Reader)) return true;

        // Creator has Creator and Reader permissions
        if (roles.Contains(UserRole.Creator) && role <= UserRole.Creator) return true;

        return false;
    }

    /// <summary>
    /// Checks if a user has any of the specified roles
    /// </summary>
    public bool HasAnyRole(AlchimaliaUser user, params UserRole[] roles)
    {
        return roles.Any(role => HasRole(user, role));
    }
}
