using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class GetHeroGreetingEndpoint
{
    private readonly IEpicHeroRepository _heroRepository;
    private readonly IUserContextService _userContext;
    private readonly IBlobSasService _blobSas;

    public GetHeroGreetingEndpoint(
        IEpicHeroRepository heroRepository,
        IUserContextService userContext,
        IBlobSasService blobSas)
    {
        _heroRepository = heroRepository;
        _userContext = userContext;
        _blobSas = blobSas;
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
        string? audioPath = null;
        bool isFromDraft = false;

        // Get greeting text and audio in requested language from craft or definition
        if (heroCraft != null)
        {
            var translation = heroCraft.Translations.FirstOrDefault(t => 
                t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase)) 
                ?? heroCraft.Translations.FirstOrDefault();
            
            greetingText = translation?.GreetingText ?? $"Hello! I'm {translation?.Name ?? heroId}.";
            audioPath = translation?.GreetingAudioUrl;
            isFromDraft = true;
        }
        else if (heroDefinition != null)
        {
            var translation = heroDefinition.Translations.FirstOrDefault(t => 
                t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase)) 
                ?? heroDefinition.Translations.FirstOrDefault();
            
            greetingText = translation?.GreetingText ?? $"Hello! I'm {translation?.Name ?? heroId}.";
            audioPath = translation?.GreetingAudioUrl;
            isFromDraft = false;
        }

        // Generate SAS URL for audio if path exists
        string? audioUrl = null;
        if (!string.IsNullOrWhiteSpace(audioPath))
        {
            // Determine which container to use based on source (draft vs published)
            var container = isFromDraft ? ep._blobSas.DraftContainer : ep._blobSas.PublishedContainer;
            
            try
            {
                // Generate SAS URL for reading the audio file
                audioUrl = (await ep._blobSas.GetReadSasAsync(container, audioPath, TimeSpan.FromHours(1), ct)).ToString();
            }
            catch
            {
                // If SAS generation fails, try to return path for fallback
                audioUrl = audioPath;
            }
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

