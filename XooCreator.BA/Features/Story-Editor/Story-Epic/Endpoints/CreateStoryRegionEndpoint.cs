using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

public record CreateStoryRegionRequest
{
    public string? RegionId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public required string LanguageCode { get; init; }
}

public record CreateStoryRegionResponse
{
    public required string RegionId { get; init; }
}

[Endpoint]
public class CreateStoryRegionEndpoint
{
    private readonly IStoryRegionService _regionService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateStoryRegionEndpoint> _logger;

    public CreateStoryRegionEndpoint(
        IStoryRegionService regionService,
        IAuth0UserService auth0,
        ILogger<CreateStoryRegionEndpoint> logger)
    {
        _regionService = regionService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/regions")]
    [Authorize]
    public static async Task<Results<Ok<CreateStoryRegionResponse>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] CreateStoryRegionEndpoint ep,
        [FromBody] CreateStoryRegionRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateStoryRegion forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(req.Name))
        {
            return TypedResults.BadRequest("Region name is required.");
        }

        // Generate regionId if not provided (format: {slug}-region-{YYYYMMDD})
        string regionId = (req.RegionId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(regionId))
        {
            var sanitizedName = Regex.Replace(req.Name, @"[^a-zA-Z0-9-]", "-").ToLowerInvariant();
            regionId = $"{sanitizedName}-region-{DateTime.UtcNow:yyyyMMdd}";
            ep._logger.LogInformation("Generated regionId: {RegionId} for userId={UserId}", regionId, user.Id);
        }

        try
        {
            var region = await ep._regionService.CreateRegionAsync(
                user.Id, 
                regionId, 
                req.Name, 
                req.Description, 
                req.LanguageCode, 
                ct);
            ep._logger.LogInformation("CreateStoryRegion: userId={UserId} regionId={RegionId} name={Name} languageCode={LanguageCode}", 
                user.Id, regionId, req.Name, req.LanguageCode);
            
            return TypedResults.Ok(new CreateStoryRegionResponse { RegionId = region.Id });
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning("CreateStoryRegion failed: {Error}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
    }
}

