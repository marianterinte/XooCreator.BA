using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

public record CreateStoryEpicRequest
{
    public string? EpicId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
}

public record CreateStoryEpicResponse
{
    public required string EpicId { get; init; }
}

[Endpoint]
public class CreateStoryEpicEndpoint
{
    private readonly IStoryEpicService _epicService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateStoryEpicEndpoint> _logger;

    public CreateStoryEpicEndpoint(
        IStoryEpicService epicService,
        IAuth0UserService auth0,
        ILogger<CreateStoryEpicEndpoint> logger)
    {
        _epicService = epicService;
        _auth0 = auth0;
        _logger = logger;
    }

    // Route without locale - middleware UseLocaleInApiPath() strips locale from path before routing
    [Route("/api/story-editor/epics")]
    [Authorize]
    public static async Task<Results<Ok<CreateStoryEpicResponse>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] CreateStoryEpicEndpoint ep,
        [FromBody] CreateStoryEpicRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Creator-only guard
        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateStoryEpic forbidden: userId={UserId} roles={Roles}", 
                user?.Id, string.Join(",", user?.Roles ?? new List<UserRole> { user?.Role ?? UserRole.Reader }));
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(req.Name))
        {
            return TypedResults.BadRequest("Epic name is required.");
        }

        // Generate epicId if not provided
        string epicId = (req.EpicId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(epicId))
        {
            // Generate a simple ID based on name and timestamp
            var sanitizedName = Regex.Replace(req.Name, @"[^a-zA-Z0-9-]", "-").ToLowerInvariant();
            epicId = $"{sanitizedName}-{DateTime.UtcNow:yyyyMMddHHmmss}";
            ep._logger.LogInformation("Generated epicId: {EpicId} for userId={UserId}", epicId, user.Id);
        }

        try
        {
            await ep._epicService.EnsureEpicAsync(user.Id, epicId, req.Name, ct);
            ep._logger.LogInformation("CreateStoryEpic: userId={UserId} epicId={EpicId} name={Name}", 
                user.Id, epicId, req.Name);
            
            return TypedResults.Ok(new CreateStoryEpicResponse { EpicId = epicId });
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning("CreateStoryEpic failed: {Error}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
    }
}

