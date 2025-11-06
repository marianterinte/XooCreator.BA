using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Features.TreeOfLight.Repositories;
using XooCreator.BA.Features.Stories.DTOs;
using System.Text.Json;

namespace XooCreator.BA.Features.Stories.Endpoints;

[Endpoint]
public class GetStoryEditorHeroesEndpoint
{
    private readonly ITreeOfLightRepository _repository;
    private readonly IUserContextService _userContext;

    public GetStoryEditorHeroesEndpoint(ITreeOfLightRepository repository, IUserContextService userContext)
    {
        _repository = repository;
        _userContext = userContext;
    }

    [Route("/api/{locale}/story-editor/heroes")] // GET
    [Authorize]
    public static async Task<Results<Ok<List<StoryEditorHeroDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetStoryEditorHeroesEndpoint ep)
    {
        var userId = await ep._userContext.GetUserIdAsync();
        if (userId == null) return TypedResults.Unauthorized();

        var heroes = await ep.GetAllHeroesForEditorAsync(locale);
        return TypedResults.Ok(heroes);
    }

    private async Task<List<StoryEditorHeroDto>> GetAllHeroesForEditorAsync(string locale)
    {
        var storyHeroes = await _repository.GetStoryHeroesAsync();
        var heroDtos = new List<StoryEditorHeroDto>();

        foreach (var storyHero in storyHeroes)
        {
            var name = await GetHeroTranslationAsync(storyHero.HeroId, "name", locale);
            var description = await GetHeroTranslationAsync(storyHero.HeroId, "description", locale);
            var story = await GetHeroTranslationAsync(storyHero.HeroId, "story", locale);

            heroDtos.Add(new StoryEditorHeroDto
            {
                HeroId = storyHero.HeroId,
                Name = name,
                Description = description,
                Story = story,
                ImageUrl = storyHero.ImageUrl
            });
        }

        return heroDtos;
    }

    private async Task<string> GetHeroTranslationAsync(string heroId, string field, string locale)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            
            // Normalize locale to lowercase (e.g., "en-US" -> "en-us")
            var normalizedLocale = locale.ToLower();
            
            // Try the individual hero file in StoryHeroes/i18n (new location)
            var heroFilePath = Path.Combine(baseDir, "Data", "SeedData", "StoryHeroes", "i18n", normalizedLocale, $"{heroId}.json");
            
            if (File.Exists(heroFilePath))
            {
                var json = await File.ReadAllTextAsync(heroFilePath);
                var heroData = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });

                if (heroData?.ContainsKey(field) == true)
                {
                    return heroData[field];
                }
            }
            
            // Fallback to English if not found
            if (normalizedLocale != "en-us")
            {
                var fallbackFilePath = Path.Combine(baseDir, "Data", "SeedData", "StoryHeroes", "i18n", "en-us", $"{heroId}.json");
                if (File.Exists(fallbackFilePath))
                {
                    var json = await File.ReadAllTextAsync(fallbackFilePath);
                    var heroData = JsonSerializer.Deserialize<Dictionary<string, string>>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });

                    if (heroData?.ContainsKey(field) == true)
                    {
                        return heroData[field];
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error reading hero translation for {heroId}.{field} ({locale}): {ex.Message}");
        }

        // Fallback values
        return field switch
        {
            "name" => heroId,
            "description" => $"A hero from the Tree of Light",
            "story" => $"Story of {heroId}",
            _ => string.Empty
        };
    }
}

