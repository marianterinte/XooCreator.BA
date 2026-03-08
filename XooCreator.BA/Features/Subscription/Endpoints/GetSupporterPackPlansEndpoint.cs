using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.Subscription.Services;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.Subscription.Endpoints;

[Endpoint]
public class GetSupporterPackPlansEndpoint
{
    private readonly ISupporterPackPlanConfigService _planConfig;

    public GetSupporterPackPlansEndpoint(ISupporterPackPlanConfigService planConfig)
    {
        _planConfig = planConfig;
    }

    public record PlanDto(
        string PlanId,
        decimal PriceRon,
        int GenerativeCredits,
        int PrintQuota,
        int ExclusiveStoryAccessCount,
        int ExclusiveEpicAccessCount,
        int FullStoryGenerationCount);

    /// <summary>Public endpoint: list active plans for /supporter-packs page.</summary>
    [Route("/api/supporter-packs/plans")]
    public static async Task<Results<Ok<IReadOnlyList<PlanDto>>, NotFound>> HandleGet(
        [FromServices] GetSupporterPackPlansEndpoint ep,
        CancellationToken ct)
    {
        var plans = await ep._planConfig.GetAllPlansAsync(ct);
        if (plans.Count == 0)
            return TypedResults.NotFound();

        var dtos = plans.Select(p => new PlanDto(
            p.PlanId,
            p.PriceRon,
            p.GenerativeCredits,
            p.PrintQuota,
            p.ExclusiveStoryAccessCount,
            p.ExclusiveEpicAccessCount,
            p.FullStoryGenerationCount)).ToList();

        return TypedResults.Ok<IReadOnlyList<PlanDto>>(dtos);
    }
}
