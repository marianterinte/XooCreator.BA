using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetStoryTopicsEndpoint
{
    private readonly XooDbContext _context;

    public GetStoryTopicsEndpoint(XooDbContext context)
    {
        _context = context;
    }

    public record TopicDto(
        string Id,
        string DimensionId,
        string Label,
        int SortOrder
    );

    public record DimensionDto(
        string Id,
        string Label,
        List<TopicDto> Topics
    );

    public record TopicsResponse(
        List<DimensionDto> Dimensions
    );

    [Route("/api/{locale}/story-editor/topics")]
    [Authorize]
    public static async Task<Results<Ok<TopicsResponse>, BadRequest<string>>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetStoryTopicsEndpoint ep,
        CancellationToken ct)
    {
        var lang = locale.ToLowerInvariant();

        try
        {
            // Load all topics with their translations
            var topics = await ep._context.StoryTopics
                .Include(t => t.Translations)
                .Where(t => t.IsActive)
                .OrderBy(t => t.SortOrder)
                .ToListAsync(ct);

            // Group by dimension
            var dimensions = topics
                .GroupBy(t => t.DimensionId)
                .Select(g => new DimensionDto(
                    Id: g.Key,
                    Label: GetDimensionLabel(g.Key, lang), // We can add dimension translations later if needed
                    Topics: g.Select(t => new TopicDto(
                        Id: t.TopicId,
                        DimensionId: t.DimensionId,
                        Label: t.Translations
                            .FirstOrDefault(tr => tr.LanguageCode == lang)?.Label
                            ?? t.Translations.FirstOrDefault()?.Label
                            ?? t.TopicId,
                        SortOrder: t.SortOrder
                    )).OrderBy(t => t.SortOrder).ToList()
                ))
                .OrderBy(d => d.Id == "alchimalia_universe" ? 0 : d.Id == "classic" ? 1 : 2)
                .ThenBy(d => d.Id)
                .ToList();

            return TypedResults.Ok(new TopicsResponse(Dimensions: dimensions));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Failed to load topics: {ex.Message}");
        }
    }

    private static string GetDimensionLabel(string dimensionId, string lang)
    {
        // Try to load from JSON files first
        var label = GetDimensionLabelFromJson(dimensionId, lang);
        if (!string.IsNullOrEmpty(label))
        {
            return label;
        }

        // Fallback to hardcoded values if JSON not found
        return dimensionId switch
        {
            "alchimalia_universe" => lang == "ro-ro" ? "Universul Alchimalia" : lang == "hu-hu" ? "Alchimalia Univerzum" : "Alchimalia Universe",
            "educational" => lang == "ro-ro" ? "Educațional" : lang == "hu-hu" ? "Oktatási" : "Educational",
            "fun" => lang == "ro-ro" ? "Distractiv" : lang == "hu-hu" ? "Szórakoztató" : "Fun",
            "emotional_depth" => lang == "ro-ro" ? "Profunzime emoțională" : lang == "hu-hu" ? "Érzelmi mélység" : "Emotional depth",
            "pace_and_action" => lang == "ro-ro" ? "Ritm și acțiune" : lang == "hu-hu" ? "Ütem és akció" : "Pace and action",
            "complexity" => lang == "ro-ro" ? "Complexitate narativă" : lang == "hu-hu" ? "Narratív komplexitás" : "Narrative complexity",
            "interactivity" => lang == "ro-ro" ? "Interactivitate" : lang == "hu-hu" ? "Interaktivitás" : "Interactivity",
            "values_and_morals" => lang == "ro-ro" ? "Valori și mesaj" : lang == "hu-hu" ? "Értékek és erkölcs" : "Values and morals",
            "classic" => lang == "ro-ro" ? "Poveste Populară (clasică)" : lang == "hu-hu" ? "Népszerű Történet (klasszikus)" : "Popular Story (classic)",
            _ => dimensionId
        };
    }

    private static string GetDimensionLabelFromJson(string dimensionId, string lang)
    {
        try
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "Story-Editor", "Topic", "i18n");
            var jsonPath = Path.Combine(basePath, lang, "topics.json");
            
            if (!File.Exists(jsonPath))
            {
                return string.Empty;
            }

            var json = File.ReadAllText(jsonPath);
            var data = JsonSerializer.Deserialize<TopicsSeedData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            var dimension = data?.StoryDimensions?.FirstOrDefault(d => d.Id == dimensionId);
            return dimension?.Label ?? string.Empty;
        }
        catch
        {
            // If anything fails, return empty to use fallback
            return string.Empty;
        }
    }

    // DTOs for JSON deserialization
    private class TopicsSeedData
    {
        [JsonPropertyName("story_dimensions")]
        public List<StoryDimensionSeedData>? StoryDimensions { get; set; }
    }

    private class StoryDimensionSeedData
    {
        public string Id { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }
}

