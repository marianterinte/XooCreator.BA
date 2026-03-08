using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class GetUserSupporterBenefitsEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;

    public GetUserSupporterBenefitsEndpoint(XooDbContext db, IAuth0UserService auth0)
    {
        _db = db;
        _auth0 = auth0;
    }

    public record SupporterGrantDto(string PlanId, DateTime GrantedAtUtc);
    public record SupporterBenefitsResponse(List<SupporterGrantDto> Grants);

    [Route("/api/{locale}/user/supporter-benefits")]
    [Authorize]
    public static async Task<Results<Ok<SupporterBenefitsResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetUserSupporterBenefitsEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var grants = await ep._db.UserPackGrants
            .AsNoTracking()
            .Where(g => g.UserId == user.Id)
            .OrderBy(g => g.GrantedAtUtc)
            .Select(g => new SupporterGrantDto(g.PlanId, g.GrantedAtUtc))
            .ToListAsync(ct);

        return TypedResults.Ok(new SupporterBenefitsResponse(grants));
    }
}
