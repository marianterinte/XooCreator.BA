using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Infrastructure;

public interface IUserContextService
{
    Task<Guid?> GetUserIdAsync();
    Guid GetCurrentUserId();
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

    public Guid GetCurrentUserId()
    {
        var userId = GetUserIdAsync().GetAwaiter().GetResult();
        if (userId == null)
            throw new UnauthorizedAccessException("User not authenticated");
        return userId.Value;
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
        if (ctx == null) return fallback;
        
        // First check route value (for endpoints with locale in path like /api/{locale}/...)
        var routeVal = ctx.GetRouteValue("locale") as string;
        if (!string.IsNullOrWhiteSpace(routeVal)) return routeVal.ToLowerInvariant();
        
        // Then check query string (for language-agnostic endpoints like /api/story-editor/epics?locale=ro-ro)
        var queryVal = ctx.Request.Query["locale"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(queryVal)) return queryVal.ToLowerInvariant();
        
        return fallback;
    }
}
