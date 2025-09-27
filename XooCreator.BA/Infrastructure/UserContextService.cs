namespace XooCreator.BA.Infrastructure;

public interface IUserContextService
{
    Task<Guid?> GetUserIdAsync();
    Task<string?> GetUserSubAsync();
    string GetRequestLocaleOrDefault(string fallback = "ro-ro");
}

// Temporary implementation - will be replaced with Auth0 integration
public class UserContextService : IUserContextService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public UserContextService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Task<Guid?> GetUserIdAsync()
    {
        // For now, return a hardcoded test user ID
        // This will be replaced with actual Auth0 user extraction
        var testUserId = Guid.Parse("11111111-1111-1111-1111-111111111111");
        return Task.FromResult<Guid?>(testUserId);
    }

    public Task<string?> GetUserSubAsync()
    {
        // For now, return a hardcoded test sub
        // This will be replaced with actual Auth0 sub extraction
        return Task.FromResult<string?>("test-user-sub");
    }

    public string GetRequestLocaleOrDefault(string fallback = "ro-ro")
    {
        var ctx = _httpContextAccessor.HttpContext;
        var val = ctx?.GetRouteValue("locale") as string;
        if (!string.IsNullOrWhiteSpace(val)) return val.ToLowerInvariant();
        return fallback;
    }
}
