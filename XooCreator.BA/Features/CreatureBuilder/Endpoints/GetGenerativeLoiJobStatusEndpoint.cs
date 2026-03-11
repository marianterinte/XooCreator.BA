using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public sealed class GetGenerativeLoiJobStatusEndpoint
{
    private readonly XooDbContext _db;
    private readonly IUserContextService _userContext;

    public GetGenerativeLoiJobStatusEndpoint(XooDbContext db, IUserContextService userContext)
    {
        _db = db;
        _userContext = userContext;
    }

    public record GenerativeLoiJobStatusDto(
        Guid JobId,
        string Status,
        string? ProgressMessage,
        string? ErrorMessage,
        DateTime QueuedAtUtc,
        DateTime? StartedAtUtc,
        DateTime? CompletedAtUtc,
        Guid? BestiaryItemId,
        string? ResultName,
        string? ResultImageUrl,
        string? ResultStoryText
    );

    [Route("/api/{locale}/creature-builder/generative-loi-jobs/{jobId:guid}")]
    [Authorize]
    public static async Task<Results<Ok<GenerativeLoiJobStatusDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] Guid jobId,
        [FromServices] GetGenerativeLoiJobStatusEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null)
            return TypedResults.Unauthorized();

        var job = await ep._db.GenerativeLoiJobs
            .AsNoTracking()
            .FirstOrDefaultAsync(j => j.Id == jobId && j.UserId == userId.Value, ct);
        if (job == null)
            return TypedResults.NotFound();

        return TypedResults.Ok(new GenerativeLoiJobStatusDto(
            job.Id,
            job.Status,
            job.ProgressMessage,
            job.ErrorMessage,
            job.QueuedAtUtc,
            job.StartedAtUtc,
            job.CompletedAtUtc,
            job.BestiaryItemId,
            job.ResultName,
            job.ResultImageUrl,
            job.ResultStoryText
        ));
    }
}
