using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetHeroGreetingEndpoint
{
    private readonly IEpicHeroRepository _heroRepository;
    private readonly IUserContextService _userContext;

    public GetHeroGreetingEndpoint(
        IEpicHeroRepository heroRepository,
        IUserContextService userContext)
    {
        _heroRepository = heroRepository;
        _userContext = userContext;
    }

    [Route("/api/{locale}/story-epic/heroes/{heroId}/greeting")]
    [Authorize]
    public static async Task<Results<Ok<HeroGreetingDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string heroId,
        [FromServices] GetHeroGreetingEndpoint ep,
        CancellationToken ct)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var hero = await ep._heroRepository.GetAsync(heroId, ct);
        if (hero == null)
        {
            return TypedResults.NotFound();
        }

        // Get greeting text in requested language
        var translation = hero.Translations.FirstOrDefault(t => 
            t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));
        
        // Fallback to first available translation if requested language not found
        translation ??= hero.Translations.FirstOrDefault();

        var greetingText = translation?.GreetingText ?? $"Hello! I'm {translation?.Name ?? heroId}.";

        var response = new HeroGreetingDto
        {
            HeroId = heroId,
            Message = greetingText,
            AudioUrl = hero.GreetingAudioUrl
        };

        return TypedResults.Ok(response);
    }
}

public record HeroGreetingDto
{
    public required string HeroId { get; init; }
    public required string Message { get; init; }
    public string? AudioUrl { get; init; }
}

