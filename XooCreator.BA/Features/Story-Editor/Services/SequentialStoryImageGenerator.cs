namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Generates story page images sequentially with reference chaining (each page uses the previous generated image as reference).
/// Same logic as Generate Full Story Draft when "Use consistent image style" is enabled. Used by both
/// Generate Full Story Draft (Google path) and Story Image Export worker.
/// </summary>
public interface ISequentialStoryImageGenerator
{
    /// <summary>
    /// Generates one image per tile text, sequentially. Page 1 uses <paramref name="referenceImageBytes"/> (if any);
    /// each following page uses the previously generated image as reference for character/style consistency.
    /// </summary>
    /// <param name="storyJson">Story context JSON (title, summary, tiles).</param>
    /// <param name="tileTexts">Ordered list of page/tile texts to illustrate.</param>
    /// <param name="locale">Language code (e.g. ro-ro, en-us).</param>
    /// <param name="extraInstructions">Optional style instructions.</param>
    /// <param name="referenceImageBytes">Optional reference image for the first page (user upload).</param>
    /// <param name="referenceImageMime">MIME type of reference image.</param>
    /// <param name="apiKey">Google API key.</param>
    /// <param name="imageModel">Optional model override.</param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>List of (image bytes, mime type) in the same order as <paramref name="tileTexts"/>.</returns>
    Task<List<(byte[] ImageData, string MimeType)>> GenerateSequentialWithChainingAsync(
        string storyJson,
        IReadOnlyList<string> tileTexts,
        string locale,
        string? extraInstructions,
        byte[]? referenceImageBytes,
        string? referenceImageMime,
        string apiKey,
        string? imageModel,
        CancellationToken ct = default);
}

/// <summary>
/// Implementation using <see cref="IGoogleImageService"/>. Single place for sequential + reference chaining logic.
/// </summary>
public sealed class SequentialStoryImageGenerator : ISequentialStoryImageGenerator
{
    private readonly IGoogleImageService _googleImage;

    public SequentialStoryImageGenerator(IGoogleImageService googleImage)
    {
        _googleImage = googleImage;
    }

    /// <inheritdoc />
    public async Task<List<(byte[] ImageData, string MimeType)>> GenerateSequentialWithChainingAsync(
        string storyJson,
        IReadOnlyList<string> tileTexts,
        string locale,
        string? extraInstructions,
        byte[]? referenceImageBytes,
        string? referenceImageMime,
        string apiKey,
        string? imageModel,
        CancellationToken ct = default)
    {
        var results = new List<(byte[] ImageData, string MimeType)>();
        byte[]? currentRefBytes = referenceImageBytes;
        string? currentRefMime = referenceImageMime;

        for (var i = 0; i < tileTexts.Count; i++)
        {
            var tileText = tileTexts[i];

            var (imageData, mimeType) = await _googleImage.GenerateStoryImageAsync(
                storyJson,
                tileText,
                locale,
                extraInstructions,
                currentRefBytes,
                currentRefMime,
                ct,
                apiKey,
                imageModel).ConfigureAwait(false);

            results.Add((imageData, mimeType ?? "image/png"));
            currentRefBytes = imageData;
            currentRefMime = mimeType;
        }

        return results;
    }
}
