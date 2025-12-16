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

    public record SetStartupRegionRequest
    {
        public required bool IsStartup { get; init; }
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics/{epicId}/regions/{regionId}/set-startup")]
    [Authorize]
    public static async Task<Results<Ok, NotFound, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string epicId,
        [FromRoute] string regionId,
        [FromBody] SetStartupRegionRequest req,
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

        // Verify epic exists and user owns it (check both craft and definition)
        var craft = await ep._context.StoryEpicCrafts
            .FirstOrDefaultAsync(e => e.Id == epicId, ct);
        
        if (craft == null)
        {
            ep._logger.LogWarning("SetStartupRegion: Epic not found epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.NotFound();
        }

        if (craft.OwnerUserId != user.Id)
        {
            ep._logger.LogWarning("SetStartupRegion: User does not own epic epicId={EpicId} userId={UserId}", epicId, user.Id);
            return TypedResults.Forbid();
        }

        // Verify region exists in this epic (use StoryEpicCraftRegions)
        var region = await ep._context.StoryEpicCraftRegions
            .FirstOrDefaultAsync(r => r.EpicId == epicId && r.RegionId == regionId, ct);
        
        if (region == null)
        {
            ep._logger.LogWarning("SetStartupRegion: Region not found epicId={EpicId} regionId={RegionId} userId={UserId}", 
                epicId, regionId, user.Id);
            return TypedResults.BadRequest("Region not found in this epic");
        }

        if (req.IsStartup)
        {
            // Step 1: Set ALL existing startup regions to false first (to avoid constraint violation)
            // We need to do this in a separate save to ensure no two regions have IsStartupRegion = true at the same time
            var existingStartupRegions = await ep._context.StoryEpicCraftRegions
                .Where(r => r.EpicId == epicId && r.IsStartupRegion)
                .ToListAsync(ct);
            
            foreach (var reg in existingStartupRegions)
            {
                reg.IsStartupRegion = false;
                reg.UpdatedAt = DateTime.UtcNow;
            }
            
            // Save changes to unset all existing startup regions
            if (existingStartupRegions.Count > 0)
            {
                await ep._context.SaveChangesAsync(ct);
            }

            // Step 2: Now set the selected region to true (safe now, no other region is startup)
            // Detach the region from context and reload it to ensure we have fresh data
            ep._context.Entry(region).State = Microsoft.EntityFrameworkCore.EntityState.Detached;
            var freshRegion = await ep._context.StoryEpicCraftRegions
                .FirstOrDefaultAsync(r => r.EpicId == epicId && r.RegionId == regionId, ct);
            
            if (freshRegion == null)
            {
                return TypedResults.BadRequest("Region not found after reload");
            }

            freshRegion.IsStartupRegion = true;
            freshRegion.UpdatedAt = DateTime.UtcNow;

            await ep._context.SaveChangesAsync(ct);

            ep._logger.LogInformation("SetStartupRegion: Region marked as startup epicId={EpicId} regionId={RegionId} userId={UserId}", 
                epicId, regionId, user.Id);
        }
        else
        {
            // Just unset this region
            region.IsStartupRegion = false;
            region.UpdatedAt = DateTime.UtcNow;

            await ep._context.SaveChangesAsync(ct);

            ep._logger.LogInformation("SetStartupRegion: Region unmarked as startup epicId={EpicId} regionId={RegionId} userId={UserId}", 
                epicId, regionId, user.Id);
        }

        return TypedResults.Ok();
    }
}

