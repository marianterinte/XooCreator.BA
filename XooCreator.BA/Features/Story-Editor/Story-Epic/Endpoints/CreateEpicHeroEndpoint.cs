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

public record CreateEpicHeroRequest
{
    public string? HeroId { get; init; }
    public required string Name { get; init; }
    public required string LanguageCode { get; init; } // Language code for initial translation
    public string? Description { get; init; } // Optional description for initial translation
}

public record CreateEpicHeroResponse
{
    public required string HeroId { get; init; }
}

[Endpoint]
public class CreateEpicHeroEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<CreateEpicHeroEndpoint> _logger;

    public CreateEpicHeroEndpoint(
        IEpicHeroService heroService,
        IAuth0UserService auth0,
        ILogger<CreateEpicHeroEndpoint> logger)
    {
        _heroService = heroService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/heroes")]
    [Authorize]
    public static async Task<Results<Ok<CreateEpicHeroResponse>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] CreateEpicHeroEndpoint ep,
        [FromBody] CreateEpicHeroRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator))
        {
            ep._logger.LogWarning("CreateEpicHero forbidden: userId={UserId}", user?.Id);
            return TypedResults.Forbid();
        }

        if (string.IsNullOrWhiteSpace(req.Name))
        {
            return TypedResults.BadRequest("Hero name is required.");
        }

        // Generate heroId if not provided (format: hero-{slug}-{YYYYMMDD})
        string heroId = (req.HeroId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(heroId))
        {
            var sanitizedName = Regex.Replace(req.Name, @"[^a-zA-Z0-9-]", "-").ToLowerInvariant();
            heroId = $"hero-{sanitizedName}-{DateTime.UtcNow:yyyyMMdd}";
            ep._logger.LogInformation("Generated heroId: {HeroId} for userId={UserId}", heroId, user.Id);
        }

        try
        {
            var hero = await ep._heroService.CreateHeroAsync(user.Id, heroId, req.Name, req.LanguageCode, req.Description, ct);
            ep._logger.LogInformation("CreateEpicHero: userId={UserId} heroId={HeroId} name={Name} languageCode={LanguageCode}", 
                user.Id, heroId, req.Name, req.LanguageCode);
            
            return TypedResults.Ok(new CreateEpicHeroResponse { HeroId = hero.Id });
        }
        catch (InvalidOperationException ex)
        {
            ep._logger.LogWarning("CreateEpicHero failed: {Error}", ex.Message);
            return TypedResults.BadRequest(ex.Message);
        }
    }
}

