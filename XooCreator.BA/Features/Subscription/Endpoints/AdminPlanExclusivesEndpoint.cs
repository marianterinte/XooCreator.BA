using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class AdminPlanExclusivesEndpoint
{
    private static readonly HashSet<string> ValidPlanIds = new(StringComparer.OrdinalIgnoreCase)
        { "Bronze", "Silver", "Gold", "Platinum" };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public AdminPlanExclusivesEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
    }

    public record PlanExclusiveDto(string PlanId, IReadOnlyList<string> StoryIds, IReadOnlyList<string> EpicIds);

    public record UpdatePlanExclusivesRequest(IReadOnlyList<string>? StoryIds, IReadOnlyList<string>? EpicIds);

    [Route("/api/admin/supporter-packs/exclusives")]
    [Authorize]
    public static async Task<Results<Ok<IReadOnlyList<PlanExclusiveDto>>, ForbidHttpResult>> HandleGet(
        [FromServices] AdminPlanExclusivesEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var bundles = await ep._db.SupporterPackPlanExclusives
            .AsNoTracking()
            .OrderBy(b => b.PlanId)
            .ToListAsync(ct);

        var dtos = bundles.Select(b => new PlanExclusiveDto(
            b.PlanId,
            ParseJsonIds(b.ExclusiveStoryIdsJson),
            ParseJsonIds(b.ExclusiveEpicIdsJson)
        )).ToList();

        return TypedResults.Ok<IReadOnlyList<PlanExclusiveDto>>(dtos);
    }

    [Route("/api/admin/supporter-packs/exclusives/{planId}")]
    [Authorize]
    public static async Task<Results<Ok<PlanExclusiveDto>, NotFound, ForbidHttpResult, BadRequest<string>>> HandlePut(
        string planId,
        [FromBody] UpdatePlanExclusivesRequest? request,
        [FromServices] AdminPlanExclusivesEndpoint ep,
        CancellationToken ct)
    {
        var admin = await ep._auth0.GetCurrentUserAsync(ct);
        if (admin == null || !admin.Roles.Contains(UserRole.Admin))
            return TypedResults.Forbid();

        var plan = (planId ?? "").Trim();
        if (string.IsNullOrEmpty(plan) || !ValidPlanIds.Contains(plan))
            return TypedResults.BadRequest("PlanId must be one of: Bronze, Silver, Gold, Platinum.");

        var entity = await ep._db.SupporterPackPlanExclusives
            .FirstOrDefaultAsync(b => b.PlanId == plan, ct);
        if (entity == null)
            return TypedResults.NotFound();

        var storyIds = request?.StoryIds?.Select(s => (s ?? "").Trim()).Where(s => s.Length > 0).ToList() ?? new List<string>();
        var epicIds = request?.EpicIds?.Select(s => (s ?? "").Trim()).Where(s => s.Length > 0).ToList() ?? new List<string>();

        entity.ExclusiveStoryIdsJson = JsonSerializer.Serialize(storyIds);
        entity.ExclusiveEpicIdsJson = JsonSerializer.Serialize(epicIds);
        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok(new PlanExclusiveDto(entity.PlanId, storyIds, epicIds));
    }

    private static IReadOnlyList<string> ParseJsonIds(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return Array.Empty<string>();
        try
        {
            var list = JsonSerializer.Deserialize<List<string>>(json);
            return list?.Select(s => (s ?? "").Trim()).Where(s => s.Length > 0).ToList() ?? new List<string>();
        }
        catch
        {
            return new List<string>();
        }
    }
}
