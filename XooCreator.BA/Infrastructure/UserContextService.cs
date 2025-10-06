using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Infrastructure;

public interface IUserContextService
{
    Task<Guid?> GetUserIdAsync();
    Task<string?> GetUserSubAsync();
    string GetRequestLocaleOrDefault(string fallback = "ro-ro");
}

public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IAuth0UserService _auth0UserService;

    public UserContextService(IHttpContextAccessor httpContextAccessor, IAuth0UserService auth0UserService)
    {
        _httpContextAccessor = httpContextAccessor;
        _auth0UserService = auth0UserService;
    }

    public async Task<Guid?> GetUserIdAsync()
    {
        return await _auth0UserService.GetCurrentUserIdAsync();
    }

    public Task<string?> GetUserSubAsync()
    {
        var httpContext = _httpContextAccessor.HttpContext;
        if (httpContext?.User?.Identity?.IsAuthenticated != true)
            return Task.FromResult<string?>(null);

        // Extract Auth0 sub claim
        var sub = httpContext.User.FindFirst("sub")?.Value;
        return Task.FromResult(sub);
    }

    public string GetRequestLocaleOrDefault(string fallback = "ro-ro")
    {
        var ctx = _httpContextAccessor.HttpContext;
        var val = ctx?.GetRouteValue("locale") as string;
        if (!string.IsNullOrWhiteSpace(val)) return val.ToLowerInvariant();
        return fallback;
    }
}
