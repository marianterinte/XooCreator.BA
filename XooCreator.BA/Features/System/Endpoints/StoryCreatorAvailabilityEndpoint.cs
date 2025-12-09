using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.System.Endpoints;

[Endpoint]
public class StoryCreatorAvailabilityEndpoint
{
    private const string SettingKey = "story-creator-disabled";
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<StoryCreatorAvailabilityEndpoint> _logger;

    public StoryCreatorAvailabilityEndpoint(
        XooDbContext context,
        IAuth0UserService auth0,
        ILogger<StoryCreatorAvailabilityEndpoint> logger)
    {
        _context = context;
        _auth0 = auth0;
        _logger = logger;
    }

    // Allow both /api/story-creator/status and /api/{locale}/story-creator/status (locale optional)
    [Route("/api/story-creator/status")]
    [Route("/api/{locale}/story-creator/status")]
    [Authorize]
    public static async Task<Results<Ok<StoryCreatorStatusResponse>, UnauthorizedHttpResult>> HandleGet(
        [FromServices] StoryCreatorAvailabilityEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var status = await ep.GetStatusAsync(ct);
        return TypedResults.Ok(status);
    }

    // Allow both /api/admin/story-creator/status and /api/{locale}/admin/story-creator/status (locale optional)
    [Route("/api/admin/story-creator/status")]
    [Route("/api/{locale}/admin/story-creator/status")]
    [Authorize]
    public static async Task<Results<Ok<StoryCreatorStatusResponse>, UnauthorizedHttpResult, ForbidHttpResult, BadRequest<string>>> HandlePut(
        [FromBody] UpdateStoryCreatorStatusRequest request,
        [FromServices] StoryCreatorAvailabilityEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Admin))
            return TypedResults.Forbid();

        if (request == null)
            return TypedResults.BadRequest("Payload is required.");

        var setting = await ep._context.PlatformSettings.FirstOrDefaultAsync(x => x.Key == SettingKey, ct);
        if (setting == null)
        {
            setting = new PlatformSetting
            {
                Key = SettingKey
            };
            ep._context.PlatformSettings.Add(setting);
        }

        setting.BoolValue = request.IsDisabled;
        setting.UpdatedAt = DateTime.UtcNow;
        setting.UpdatedBy = user.Id.ToString();

        await ep._context.SaveChangesAsync(ct);

        ep._logger.LogInformation("Story Creator availability updated: disabled={Disabled}, adminId={AdminId}", request.IsDisabled, user.Id);

        return TypedResults.Ok(new StoryCreatorStatusResponse
        {
            IsDisabled = setting.BoolValue,
            UpdatedAt = setting.UpdatedAt,
            UpdatedBy = setting.UpdatedBy
        });
    }

    private async Task<StoryCreatorStatusResponse> GetStatusAsync(CancellationToken ct)
    {
        var setting = await _context.PlatformSettings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Key == SettingKey, ct);

        if (setting == null)
        {
            return new StoryCreatorStatusResponse
            {
                IsDisabled = false
            };
        }

        return new StoryCreatorStatusResponse
        {
            IsDisabled = setting.BoolValue,
            UpdatedAt = setting.UpdatedAt,
            UpdatedBy = setting.UpdatedBy
        };
    }
}

public record StoryCreatorStatusResponse
{
    public bool IsDisabled { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public string? UpdatedBy { get; init; }
}

public record UpdateStoryCreatorStatusRequest
{
    public bool IsDisabled { get; init; }
}

