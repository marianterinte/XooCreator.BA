namespace XooCreator.BA.Features.CreatureBuilder.Services;

/// <summary>
/// LOI-specific image generation: builds kid-safe creature prompts and delegates to Google image service.
/// Business logic for Imagination Laboratory generative mode lives here.
/// </summary>
public interface ILOIImageGenerationService
{
    /// <summary>
    /// Generates a single kid-safe illustration of a hybrid creature from the given body-part to animal label map.
    /// Safe for children: no scary, violent, or mature content.
    /// </summary>
    Task<(byte[] ImageData, string MimeType)> GenerateCreatureImageAsync(
        IReadOnlyDictionary<string, string> partToAnimalLabel,
        string languageCode,
        CancellationToken ct = default);
}
