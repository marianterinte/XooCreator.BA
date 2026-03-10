using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Subscription.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class GetPrintQuotaEndpoint
{
    private readonly IAuth0UserService _auth0;
    private readonly IPrintQuotaService _quota;

    public GetPrintQuotaEndpoint(IAuth0UserService auth0, IPrintQuotaService quota)
    {
        _auth0 = auth0;
        _quota = quota;
    }

    public record PrintQuotaResponse(int UsedCount, int Limit, int Remaining, bool CanPrint, bool IsSupporter);

    /// <summary>
    /// Returns the user's print quota. Limit = sum of quotas from UserPackGrants (or FreePrintLimit if no grants).
    /// UsedCount = total exports/prints (each job completed + each StoryPrintRecord), not distinct stories.
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
        var q = await ep._quota.GetAsync(user.Id, isAdmin, ct);

        return TypedResults.Ok(new PrintQuotaResponse(
            UsedCount: q.UsedCount,
            Limit: q.Limit,
            Remaining: q.Remaining,
            CanPrint: q.CanPrint,
            IsSupporter: q.IsSupporter));
    }
}
