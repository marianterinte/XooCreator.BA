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

        // Try to get craft first (draft), fallback to definition (published)
        var heroCraft = await ep._heroRepository.GetCraftAsync(heroId, ct);
        var heroDefinition = heroCraft == null ? await ep._heroRepository.GetDefinitionAsync(heroId, ct) : null;
        
        if (heroCraft == null && heroDefinition == null)
        {
            return TypedResults.NotFound();
        }

        string? greetingText = null;
        string? audioUrl = null;

        // Get greeting text and audio in requested language from craft or definition
        if (heroCraft != null)
        {
            var translation = heroCraft.Translations.FirstOrDefault(t => 
                t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase)) 
                ?? heroCraft.Translations.FirstOrDefault();
            
            greetingText = translation?.GreetingText ?? $"Hello! I'm {translation?.Name ?? heroId}.";
            audioUrl = translation?.GreetingAudioUrl;
        }
        else if (heroDefinition != null)
        {
            var translation = heroDefinition.Translations.FirstOrDefault(t => 
                t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase)) 
                ?? heroDefinition.Translations.FirstOrDefault();
            
            greetingText = translation?.GreetingText ?? $"Hello! I'm {translation?.Name ?? heroId}.";
            audioUrl = translation?.GreetingAudioUrl;
        }

        var response = new HeroGreetingDto
        {
            HeroId = heroId,
            Message = greetingText,
            AudioUrl = audioUrl
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

