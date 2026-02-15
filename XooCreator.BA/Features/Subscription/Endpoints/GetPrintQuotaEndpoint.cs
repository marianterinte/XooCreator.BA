using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class GetPrintQuotaEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;

    public GetPrintQuotaEndpoint(XooDbContext db, IAuth0UserService auth0, IConfiguration config)
    {
        _db = db;
        _auth0 = auth0;
        _config = config;
    }

    public record PrintQuotaResponse(int UsedCount, int Limit, int Remaining, bool CanPrint, bool IsSupporter);

    /// <summary>
    /// Returns the user's print quota. UsedCount = distinct stories printed/exported.
    /// Limit is configurable via Subscription:FreePrintLimit (default 1).
    /// </summary>
    [Route("/api/{locale}/user/print-quota")]
    [Authorize]
    public static async Task<Results<Ok<PrintQuotaResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromQuery] string? storyId,
        [FromServices] GetPrintQuotaEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (isAdmin)
            return TypedResults.Ok(new PrintQuotaResponse(UsedCount: 0, Limit: int.MaxValue, Remaining: int.MaxValue, CanPrint: true, IsSupporter: true));

        var limit = ep._config.GetValue("Subscription:FreePrintLimit", 1);
        if (limit < 0) limit = 1;

        // Distinct stories from client prints
        var printedStoryIds = await ep._db.StoryPrintRecords
            .AsNoTracking()
            .Where(r => r.UserId == user.Id)
            .Select(r => r.StoryId)
            .Distinct()
            .ToListAsync(ct);

        // Distinct stories from server exports (completed)
        var exportedStoryIds = await ep._db.StoryDocumentExportJobs
            .AsNoTracking()
            .Where(j => j.RequestedByUserId == user.Id && j.Status == StoryDocumentExportJobStatus.Completed)
            .Select(j => j.StoryId)
            .Distinct()
            .ToListAsync(ct);

        var allPrinted = printedStoryIds.Union(exportedStoryIds).Distinct().ToList();
        var usedCount = allPrinted.Count;

        var nowUtc = DateTime.UtcNow;
        var isSupporter = await ep._db.UserSubscriptions
            .AsNoTracking()
            .AnyAsync(u => u.UserId == user.Id && (u.ExpiresAtUtc == null || u.ExpiresAtUtc > nowUtc), ct);

        var remaining = Math.Max(0, limit - usedCount);
        var canPrint = isSupporter || usedCount < limit || (!string.IsNullOrWhiteSpace(storyId) && allPrinted.Contains(storyId.Trim()));

        return TypedResults.Ok(new PrintQuotaResponse(
            UsedCount: usedCount,
            Limit: limit,
            Remaining: remaining,
            CanPrint: canPrint,
            IsSupporter: isSupporter));
    }
}
