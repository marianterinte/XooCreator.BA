using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

/// <summary>
/// Search users by name (first/last) or email for co-author picker.
/// </summary>
[Endpoint]
public class SearchUsersEndpoint
{
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SearchUsersEndpoint>? _logger;

    public SearchUsersEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<SearchUsersEndpoint>? logger = null)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/users/search")]
    [Authorize]
    public static async Task<Results<Ok<SearchUsersResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromQuery] string? q,
        [FromQuery] int limit,
        [FromServices] SearchUsersEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var searchTerm = (q ?? "").Trim();
        var take = limit <= 0 ? 20 : Math.Min(limit, 50);

        if (string.IsNullOrWhiteSpace(searchTerm))
        {
            return TypedResults.Ok(new SearchUsersResponse { Users = new List<SearchUserItem>() });
        }

        var term = $"%{searchTerm}%";
        var users = await ep._context.AlchimaliaUsers
            .AsNoTracking()
            .Where(u =>
                EF.Functions.ILike(u.Name, term) ||
                EF.Functions.ILike(u.Email, term) ||
                EF.Functions.ILike(u.FirstName, term) ||
                EF.Functions.ILike(u.LastName, term) ||
                EF.Functions.ILike(u.FirstName + " " + u.LastName, term) ||
                EF.Functions.ILike(u.LastName + " " + u.FirstName, term))
            .OrderBy(u => u.Name)
            .Take(take)
            .Select(u => new SearchUserItem
            {
                UserId = u.Id,
                Name = u.Name,
                Email = u.Email
            })
            .ToListAsync(ct);

        return TypedResults.Ok(new SearchUsersResponse { Users = users });
    }
}

public record SearchUserItem
{
    public Guid UserId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Email { get; init; } = string.Empty;
}

public record SearchUsersResponse
{
    public List<SearchUserItem> Users { get; init; } = new();
}
