using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class SetStartupRegionEndpoint
{
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<SetStartupRegionEndpoint> _logger;

    public SetStartupRegionEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<SetStartupRegionEndpoint> logger)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics/{epicId}/regions/{regionId}/set-startup")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromRoute] string regionId,
        [FromServices] SetStartupRegionEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Creator-only guard
        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("SetStartupRegion forbidden: userId={UserId} roles={Roles}",
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        // Verify epic exists and user owns it
        var epic = await ep._context.StoryEpics
            .FirstOrDefaultAsync(e => e.Id == epicId, ct);
        
        if (epic == null)
        {
            ep._logger.LogWarning("SetStartupRegion: Epic not found epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.NotFound();
        }

        if (epic.OwnerUserId != user.Id)
        {
            ep._logger.LogWarning("SetStartupRegion: User does not own epic epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.Forbid();
        }

        // Verify region exists in this epic
        var region = await ep._context.StoryEpicRegions
            .FirstOrDefaultAsync(r => r.EpicId == epicId && r.RegionId == regionId, ct);
        
        if (region == null)
        {
            ep._logger.LogWarning("SetStartupRegion: Region not found epicId={EpicId} regionId={RegionId} userId={UserId}", 
                epicId, regionId, user.Id);
            return TypedResults.BadRequest("Region not found in this epic");
        }

        // Set all regions in this epic to IsStartupRegion = false
        var allRegions = await ep._context.StoryEpicRegions
            .Where(r => r.EpicId == epicId)
            .ToListAsync(ct);
        
        foreach (var reg in allRegions)
        {
            reg.IsStartupRegion = false;
            reg.UpdatedAt = DateTime.UtcNow;
        }

        // Set the selected region to IsStartupRegion = true
        region.IsStartupRegion = true;
        region.UpdatedAt = DateTime.UtcNow;

        await ep._context.SaveChangesAsync(ct);

        ep._logger.LogInformation("SetStartupRegion: Region marked as startup epicId={EpicId} regionId={RegionId} userId={UserId}", 
            epicId, regionId, user.Id);

        return TypedResults.Ok();
    }
}

