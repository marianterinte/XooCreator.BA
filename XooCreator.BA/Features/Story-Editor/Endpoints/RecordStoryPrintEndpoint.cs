using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Features.Subscription.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class RecordStoryPrintEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IPrintQuotaService _printQuota;

    public RecordStoryPrintEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IStoryCraftsRepository crafts,
        IPrintQuotaService printQuota)
    {
        _db = db;
        _auth0 = auth0;
        _crafts = crafts;
        _printQuota = printQuota;
    }

    public record RecordStoryPrintRequest(string? LanguageCode, bool? IsDraft);

    [Route("/api/{locale}/stories/{storyId}/print-record")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, UnauthorizedHttpResult, ForbidHttpResult, ProblemHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] RecordStoryPrintRequest? body,
        [FromServices] RecordStoryPrintEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var isDraft = body?.IsDraft ?? false;
        var languageCode = (body?.LanguageCode ?? locale ?? "ro-ro").Trim().ToLowerInvariant();

        if (isDraft)
        {
            var craft = await ep._crafts.GetAsync(storyId, ct);
            if (craft == null) return TypedResults.NotFound();

            var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
            var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
            if (!isAdmin && !isCreator) return TypedResults.Forbid();
            if (!isAdmin && craft.OwnerUserId != user.Id) return TypedResults.Forbid();
        }
        else
        {
            var def = await ep._db.StoryDefinitions
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StoryId == storyId && s.IsActive && s.Status == StoryStatus.Published, ct);
            if (def == null) return TypedResults.NotFound();

            var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
            var isOwner = def.CreatedBy.HasValue && def.CreatedBy.Value == user.Id;
            var hasPurchase = await ep._db.StoryPurchases.AnyAsync(sp => sp.UserId == user.Id && sp.StoryId == storyId, ct);
            var hasOwned = await ep._db.UserOwnedStories.AnyAsync(uos => uos.UserId == user.Id && uos.StoryDefinitionId == def.Id, ct);
            var canReaderDownload = def.PriceInCredits == 0 || hasPurchase || hasOwned;

            if (!isAdmin && !isOwner && !canReaderDownload) return TypedResults.Forbid();
        }

        // Enforce print quota (client-side print also consumes 1).
        var isAdminUser = ep._auth0.HasRole(user, UserRole.Admin);
        var quota = await ep._printQuota.GetAsync(user.Id, isAdminUser, ct);
        if (!quota.CanPrint)
            return TypedResults.Problem(detail: "print_quota_api_limit_reached", statusCode: 402);

        ep._db.StoryPrintRecords.Add(new StoryPrintRecord
        {
            Id = Guid.NewGuid(),
            UserId = user.Id,
            StoryId = storyId,
            PrintedAtUtc = DateTime.UtcNow,
            LanguageCode = languageCode,
            IsDraft = isDraft
        });
        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok();
    }
}
