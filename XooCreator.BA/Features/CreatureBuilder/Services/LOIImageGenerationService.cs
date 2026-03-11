using System.Linq;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.CreatureBuilder.Services;

/// <summary>
/// LOI (Laboratory of Imagination) image generation: business-specific prompts for hybrid creatures,
/// kid-safe. Uses Google image service for the actual API call.
/// </summary>
public class LOIImageGenerationService : ILOIImageGenerationService
{
    private readonly IGoogleImageService _googleImageService;
    private readonly ILogger<LOIImageGenerationService> _logger;

    public LOIImageGenerationService(
        IGoogleImageService googleImageService,
        ILogger<LOIImageGenerationService> logger)
    {
        _googleImageService = googleImageService;
        _logger = logger;
    }

    public async Task<(byte[] ImageData, string MimeType)> GenerateCreatureImageAsync(
        IReadOnlyDictionary<string, string> partToAnimalLabel,
        string languageCode,
        CancellationToken ct = default)
    {
        var prompt = BuildCreatureImagePrompt(partToAnimalLabel);
        _logger.LogInformation("LOI generating creature image. Parts={Count}", partToAnimalLabel?.Count ?? 0);
        return await _googleImageService.GenerateImageFromPromptAsync(prompt, ct);
    }

    /// <summary>
    /// Builds kid-safe prompt for a hybrid creature from body part -> animal label map.
    /// </summary>
    private static string BuildCreatureImagePrompt(IReadOnlyDictionary<string, string>? partToAnimalLabel)
    {
        if (partToAnimalLabel == null || partToAnimalLabel.Count == 0)
            return "Children's book illustration. Kid-friendly, safe for children. Draw one cute hybrid fantasy creature. Style: colorful cartoon, soft shapes, magical forest. No scary elements. No text. Portrait, vertical 9:16 (9 wide, 16 tall).";
        var parts = new List<string>();
        foreach (var kv in partToAnimalLabel.OrderBy(x => x.Key))
        {
            var label = (kv.Value ?? "").Trim();
            if (string.IsNullOrEmpty(label) || label == "—") continue;
            var partName = kv.Key.ToLowerInvariant() switch
            {
                "head" => "head",
                "body" => "body",
                "arms" => "limbs",
                "legs" => "legs",
                "tail" => "tail",
                "wings" => "wings",
                "horn" => "horn",
                "horns" => "horns",
                _ => kv.Key.ToLowerInvariant()
            };
            parts.Add($"{partName} of {label}");
        }
        var creatureDesc = parts.Count > 0 ? string.Join(", ", parts) : "friendly animal features";
        return "Children's book illustration. Kid-friendly, safe for children. " +
               "Draw one cute hybrid fantasy creature: " + creatureDesc + ". " +
               "Style: colorful cartoon, soft shapes, magical forest or gentle background. " +
               "No scary, violent, or frightening elements. No text in the image. Portrait, vertical 9:16 (9 wide, 16 tall).";
    }
}
