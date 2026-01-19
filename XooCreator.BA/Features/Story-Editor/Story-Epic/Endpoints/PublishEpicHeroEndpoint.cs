using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

public record PublishEpicHeroResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "Published";
    public DateTime? PublishedAtUtc { get; init; }
}

[Endpoint]
public class PublishEpicHeroEndpoint
{
    private readonly IEpicHeroService _heroService;
    private readonly IEpicHeroRepository _repository;
    private readonly IAuth0UserService _auth0;
    private readonly XooDbContext _db;
    private readonly ILogger<PublishEpicHeroEndpoint> _logger;

    public PublishEpicHeroEndpoint(
        IEpicHeroService heroService,
        IEpicHeroRepository repository,
        IAuth0UserService auth0,
        XooDbContext db,
        ILogger<PublishEpicHeroEndpoint> logger)
    {
        _heroService = heroService;
        _repository = repository;
        _auth0 = auth0;
        _db = db;
        _logger = logger;
    }

    [Route("/api/story-editor/heroes/{heroId}/publish")]
    [Authorize]
    public static async Task<Results<Ok<PublishEpicHeroResponse>, NotFound, Conflict<string>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromRoute] string heroId,
        [FromServices] PublishEpicHeroEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (!ep._auth0.HasRole(user, UserRole.Creator) && !ep._auth0.HasRole(user, UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);

        // Load craft to get owner info
        var craft = await ep._repository.GetCraftAsync(heroId, ct);
        if (craft == null)
        {
            return TypedResults.NotFound();
        }

        // Validate ownership (unless admin)
        if (!isAdmin && craft.OwnerUserId != user.Id)
        {
            ep._logger.LogWarning("PublishEpicHero unauthorized: userId={UserId} heroId={HeroId} ownerId={OwnerId}", 
                user.Id, heroId, craft.OwnerUserId);
            return TypedResults.Forbid();
        }

        // Get owner email (preserve original author)
        var ownerEmail = craft.Owner?.Email;
        if (string.IsNullOrWhiteSpace(ownerEmail))
        {
            // Fallback: lookup owner email from db
            ownerEmail = await ep._db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Id == craft.OwnerUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct) ?? string.Empty;
        }

        if (string.IsNullOrWhiteSpace(ownerEmail))
        {
            return TypedResults.BadRequest("Owner email is required.");
        }

        try
        {
            // Use craft's ownerUserId to preserve original author
            await ep._heroService.PublishAsync(craft.OwnerUserId, heroId, ownerEmail.Trim(), isAdmin, ct);
            var hero = await ep._heroService.GetHeroAsync(heroId, ct);
            return TypedResults.Ok(new PublishEpicHeroResponse 
            { 
                PublishedAtUtc = hero?.PublishedAtUtc 
            });
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.Conflict(ex.Message);
        }
        catch (UnauthorizedAccessException)
        {
            return TypedResults.Forbid();
        }
    }
}

