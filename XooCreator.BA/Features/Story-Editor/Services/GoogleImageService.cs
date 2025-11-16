using System.Net;
using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for generating story images using Gemini 2.5 Flash Image (Nano Banana).
/// </summary>
public interface IGoogleImageService
{
    /// <summary>
    /// Generates a single illustration image for the current story page/tile,
    /// using Gemini 2.5 Flash Image (aka Nano Banana).
    /// </summary>
    /// <param name="storyJson">
    /// Full story JSON (same as in the text service – used as context).
    /// The model should assume the currently edited page/tile is the one
    /// that needs illustration.
    /// </param>
    /// <param name="tileText">
    /// The text content of the current tile/page that needs illustration.
    /// This tells the model what specific scene to generate.
    /// </param>
    /// <param name="languageCode">
    /// Language of the story text (e.g. "ro-RO", "en-US").
    /// Only used for prompt wording.
    /// </param>
    /// <param name="extraInstructions">
    /// Optional comment/instructions that tell the model what to generate.
    /// This will be considered along with the reference image.
    /// </param>
    /// <param name="referenceImage">
    /// Optional reference image bytes (for style / character consistency).
    /// </param>
    /// <param name="referenceImageMimeType">
    /// MIME type of reference image (e.g. "image/png", "image/jpeg").
    /// If null and reference image is provided, defaults to "image/png".
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Generated image bytes and MIME type (usually "image/png").
    /// </returns>
    Task<(byte[] ImageData, string MimeType)> GenerateStoryImageAsync(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions = null,
        byte[]? referenceImage = null,
        string? referenceImageMimeType = null,
        CancellationToken ct = default);
}

/// <summary>
/// Generates story illustrations using Gemini 2.5 Flash Image (Nano Banana)
/// via the Google AI Studio API key.
/// </summary>
public class GoogleImageService : IGoogleImageService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _imageEndpoint;
    private readonly ILogger<GoogleImageService> _logger;

    public GoogleImageService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<GoogleImageService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["GoogleAI:ApiKey"]
            ?? throw new InvalidOperationException("GoogleAI:ApiKey is not configured in appsettings.json");
        _imageEndpoint = configuration["GoogleAI:Image:Endpoint"]
            ?? throw new InvalidOperationException("GoogleAI:Image:Endpoint is not configured in appsettings.json");
        _logger = logger;
    }

    public async Task<(byte[] ImageData, string MimeType)> GenerateStoryImageAsync(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions = null,
        byte[]? referenceImage = null,
        string? referenceImageMimeType = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(storyJson))
            throw new ArgumentException("Story JSON cannot be empty", nameof(storyJson));

        if (string.IsNullOrWhiteSpace(tileText))
            throw new ArgumentException("Tile text cannot be empty", nameof(tileText));

        var prompt = BuildImagePrompt(storyJson, tileText, languageCode, extraInstructions);

        // Construim parts: text + (opțional) imagine de referință
        var parts = new List<object>
        {
            new { text = prompt }
        };

        if (referenceImage != null && referenceImage.Length > 0)
        {
            var mimeType = string.IsNullOrWhiteSpace(referenceImageMimeType)
                ? "image/png"
                : referenceImageMimeType;

            var base64 = Convert.ToBase64String(referenceImage);

            parts.Add(new
            {
                inlineData = new
                {
                    mimeType,
                    data = base64
                }
            });
        }

        var requestBody = new
        {
            contents = new[]
            {
                new
                {
                    parts = parts.ToArray()
                }
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, _imageEndpoint)
        {
            Content = content
        };
        request.Headers.Add("x-goog-api-key", _apiKey);

        try
        {
            _logger.LogInformation(
                "Calling Gemini 2.5 Flash Image for story illustration. Language: {LanguageCode}, HasReferenceImage: {HasRef}",
                languageCode,
                referenceImage != null);

            using var response = await _httpClient.SendAsync(request, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Gemini Image API returned {StatusCode}: {Body}",
                    (int)response.StatusCode,
                    responseContent);

                // Check for quota/availability errors
                if (response.StatusCode == HttpStatusCode.TooManyRequests)
                {
                    var errorMessage = "Image generation model is not available in your current plan. " +
                                     "The model may require a paid plan or may not be available in your region. " +
                                     "Please check your Google AI Studio plan and quotas.";
                    throw new InvalidOperationException(errorMessage);
                }

                response.EnsureSuccessStatusCode();
            }

            return ExtractImageFromResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating story image with Gemini.");
            throw;
        }
    }

    /// <summary>
    /// Prompt engineering pentru ilustrația de pagină de poveste.
    /// Modelul primește JSON-ul complet, textul tile-ului curent și trebuie să ilustreze scena descrisă.
    /// </summary>
    private static string BuildImagePrompt(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are an illustrator for a children's picture book app called XooCreator.");
        sb.AppendLine("The user sends you a JSON document that defines a story with pages/tiles.");
        sb.AppendLine("Your job is to generate ONE illustration image for the CURRENT page/tile.");
        sb.AppendLine("Assume the current page is the one the user is editing now,");
        sb.AppendLine("and the story JSON you receive is the full story context.");
        sb.AppendLine();
        sb.AppendLine("Rules:");
        sb.AppendLine("- Keep characters, locations and mood consistent with the previous pages.");
        sb.AppendLine("- Use a colorful, friendly style suitable for young children.");
        sb.AppendLine("- DO NOT draw any text inside the image (no captions, no speech bubbles).");
        sb.AppendLine("- Imagine this as a storybook page illustration (landscape aspect ratio).");
        sb.AppendLine($"- The story language is: {languageCode}. The visual style should fit this culture if relevant.");
        sb.AppendLine("- If a reference image is provided with this prompt, use it as reference for style and characters.");
        sb.AppendLine();
        sb.AppendLine("Here is the full story JSON (context):");
        sb.AppendLine(storyJson);
        sb.AppendLine();
        sb.AppendLine("--- CURRENT TILE TEXT ---");
        sb.AppendLine("Generate an image that illustrates this specific scene:");
        sb.AppendLine(tileText);
        sb.AppendLine("--- END OF TILE TEXT ---");
        sb.AppendLine();
        sb.AppendLine("Now, based on the story context and the tile text above, create a single, coherent illustration for this scene.");

        if (!string.IsNullOrWhiteSpace(extraInstructions))
        {
            sb.AppendLine();
            sb.AppendLine("Additional instructions from the user (consider these along with the reference image if provided):");
            sb.AppendLine(extraInstructions);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Extrage prima imagine din răspunsul Gemini (inlineData data + mimeType).
    /// </summary>
    private static (byte[] ImageData, string MimeType) ExtractImageFromResponse(string responseJson)
    {
        using var doc = JsonDocument.Parse(responseJson);
        var root = doc.RootElement;

        if (!root.TryGetProperty("candidates", out var candidates) ||
            candidates.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("No candidates returned from Gemini image API.");
        }

        var candidate = candidates[0];

        if (!candidate.TryGetProperty("content", out var contentElement) ||
            !contentElement.TryGetProperty("parts", out var parts) ||
            parts.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("Gemini image response does not contain content parts.");
        }

        foreach (var part in parts.EnumerateArray())
        {
            // Acceptă atât inlineData cât și inline_data (just in case)
            if (part.TryGetProperty("inlineData", out var inlineData) ||
                part.TryGetProperty("inline_data", out inlineData))
            {
                var dataElement = inlineData.GetProperty("data");
                var base64 = dataElement.GetString();

                if (string.IsNullOrWhiteSpace(base64))
                    continue;

                string mimeType = "image/png";
                if (inlineData.TryGetProperty("mimeType", out var mimeElem))
                    mimeType = mimeElem.GetString() ?? mimeType;
                else if (inlineData.TryGetProperty("mime_type", out var mimeElem2))
                    mimeType = mimeElem2.GetString() ?? mimeType;

                var bytes = Convert.FromBase64String(base64);
                return (bytes, mimeType);
            }
        }

        throw new InvalidOperationException("No inline image data found in Gemini response.");
    }
}
