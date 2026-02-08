using System.Linq;
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
    /// <param name="apiKeyOverride">Optional API key (e.g. from job). When set, used instead of config key.</param>
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
        CancellationToken ct = default,
        string? apiKeyOverride = null);
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
        CancellationToken ct = default,
        string? apiKeyOverride = null)
    {
        if (string.IsNullOrWhiteSpace(storyJson))
            throw new ArgumentException("Story JSON cannot be empty", nameof(storyJson));

        if (string.IsNullOrWhiteSpace(tileText))
            throw new ArgumentException("Tile text cannot be empty", nameof(tileText));

        // Optimizare: extragem doar ultimele 3 pagini + summary pentru a reduce tokenii
        var optimizedJson = ExtractOptimizedStoryContext(storyJson);

        var prompt = BuildImagePrompt(optimizedJson, tileText, languageCode, extraInstructions);

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
        var apiKey = !string.IsNullOrWhiteSpace(apiKeyOverride) ? apiKeyOverride : _apiKey;
        request.Headers.Add("x-goog-api-key", apiKey);

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
    /// Extracts only the last 3 pages + summary from the full story JSON to reduce token costs.
    /// </summary>
    private static string ExtractOptimizedStoryContext(string fullStoryJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(fullStoryJson);
            var root = doc.RootElement;

            // Extragem metadata esențială
            var storyId = root.TryGetProperty("storyId", out var sid) ? sid.GetString() : null;
            var title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
            var summary = root.TryGetProperty("summary", out var s) ? s.GetString() : null;

            // Extragem tiles și filtrăm doar paginile (nu quiz-urile)
            var allTiles = new List<JsonElement>();
            if (root.TryGetProperty("tiles", out var tiles) && tiles.ValueKind == JsonValueKind.Array)
            {
                foreach (var tile in tiles.EnumerateArray())
                {
                    // Luăm doar tile-urile de tip "page"
                    if (tile.TryGetProperty("type", out var type) && 
                        type.GetString()?.Equals("page", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        allTiles.Add(tile);
                    }
                }
            }

            // Luăm ultimele 2-3 pagini (optimizare costuri - 2-3 pagini sunt suficiente pentru context)
            var lastPages = allTiles
                .OrderBy(t => t.TryGetProperty("sortOrder", out var so) ? so.GetInt32() : int.MaxValue)
                .TakeLast(3)
                .ToList();

            // Construim JSON-ul optimizat
            var optimized = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(storyId))
                optimized["storyId"] = storyId;
            if (!string.IsNullOrEmpty(title))
                optimized["title"] = title;
            if (!string.IsNullOrEmpty(summary))
                optimized["summary"] = summary;

            // Convertim ultimele pagini în obiecte simple (doar câmpurile esențiale)
            var optimizedTiles = new List<object>();
            foreach (var tile in lastPages)
            {
                var tileObj = new Dictionary<string, object?>();
                if (tile.TryGetProperty("tileId", out var tid))
                    tileObj["tileId"] = tid.GetString();
                if (tile.TryGetProperty("type", out var ttype))
                    tileObj["type"] = ttype.GetString();
                if (tile.TryGetProperty("sortOrder", out var tso))
                    tileObj["sortOrder"] = tso.GetInt32();
                if (tile.TryGetProperty("caption", out var cap))
                    tileObj["caption"] = cap.GetString();
                if (tile.TryGetProperty("text", out var txt))
                    tileObj["text"] = txt.GetString();
                // Nu includem imageUrl, audioUrl etc. pentru a reduce tokenii

                optimizedTiles.Add(tileObj);
            }

            optimized["tiles"] = optimizedTiles;

            return JsonSerializer.Serialize(optimized, new JsonSerializerOptions
            {
                WriteIndented = false // Compact pentru a reduce tokenii
            });
        }
        catch (Exception)
        {
            // Dacă parsing-ul eșuează, returnăm JSON-ul original (fallback)
            return fullStoryJson;
        }
    }

    /// <summary>
    /// Prompt engineering pentru ilustrația de pagină de poveste - optimizat pentru tokeni.
    /// </summary>
    private static string BuildImagePrompt(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Children's book illustration. Colorful, friendly. No text. Landscape. Consistent style.");
        sb.AppendLine($"Context: {storyJson}");
        sb.AppendLine($"Scene: {tileText}");

        if (!string.IsNullOrWhiteSpace(extraInstructions))
        {
            sb.AppendLine($"Style: {extraInstructions}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Extrage prima imagine din răspunsul Gemini (inlineData data + mimeType).
    /// </summary>
    private (byte[] ImageData, string MimeType) ExtractImageFromResponse(string responseJson)
    {
        using var doc = JsonDocument.Parse(responseJson);
        var root = doc.RootElement;

        // Check for prompt feedback (safety issues)
        if (root.TryGetProperty("promptFeedback", out var promptFeedback))
        {
            if (promptFeedback.TryGetProperty("blockReason", out var blockReason))
            {
                var reason = blockReason.GetString();
                _logger.LogWarning("Gemini blocked the image generation prompt. Reason: {BlockReason}", reason);
                throw new InvalidOperationException($"Image generation was blocked by Gemini safety filters. Reason: {reason}");
            }
        }

        if (!root.TryGetProperty("candidates", out var candidates) ||
            candidates.GetArrayLength() == 0)
        {
            _logger.LogError("Gemini image response has no candidates. Full response: {Response}", responseJson);
            throw new InvalidOperationException("No candidates returned from Gemini image API. The response may have been blocked or failed.");
        }

        var candidate = candidates[0];

        // Check finish reason (why generation stopped)
        // Valid reasons: STOP (normal completion), MAX_TOKENS (reached token limit but content is valid)
        // Invalid reasons: SAFETY, RECITATION (blocked by filters)
        if (candidate.TryGetProperty("finishReason", out var finishReason))
        {
            var reason = finishReason.GetString();
            
            // MAX_TOKENS is a valid completion - model reached output limit but generated valid content
            if (reason == "MAX_TOKENS")
            {
                _logger.LogInformation("Gemini image generation reached max output tokens limit. Generated content will be extracted.");
                // Continue to extract content - this is not an error
            }
            else if (reason != "STOP" && reason != null)
            {
                _logger.LogWarning("Gemini image generation finished with reason: {FinishReason}", reason);
                
                // If blocked by safety, provide more details
                if (reason == "SAFETY" || reason == "RECITATION")
                {
                    var safetyDetails = new StringBuilder();
                    if (candidate.TryGetProperty("safetyRatings", out var safetyRatings))
                    {
                        foreach (var rating in safetyRatings.EnumerateArray())
                        {
                            if (rating.TryGetProperty("category", out var cat) &&
                                rating.TryGetProperty("probability", out var prob))
                            {
                                var category = cat.GetString();
                                var probability = prob.GetString();
                                safetyDetails.AppendLine($"{category}: {probability}");
                            }
                        }
                    }
                    
                    var errorMsg = $"Image generation was blocked by safety filters. Finish reason: {reason}";
                    if (safetyDetails.Length > 0)
                        errorMsg += $"\nSafety ratings: {safetyDetails}";
                    
                    throw new InvalidOperationException(errorMsg);
                }
                
                throw new InvalidOperationException($"Image generation finished unexpectedly. Reason: {reason}");
            }
        }

        // Check if content exists
        if (!candidate.TryGetProperty("content", out var contentElement))
        {
            _logger.LogError("Gemini image candidate has no content property. Full response: {Response}", responseJson);
            throw new InvalidOperationException("Gemini image response candidate does not contain a content property.");
        }

        // Check if parts exist
        if (!contentElement.TryGetProperty("parts", out var parts) ||
            parts.GetArrayLength() == 0)
        {
            _logger.LogError("Gemini image response has no content parts. Candidate: {Candidate}", candidate.GetRawText());
            throw new InvalidOperationException("Gemini image response does not contain content parts. The content may have been blocked or the response format is unexpected.");
        }

        // Extract image from parts
        foreach (var part in parts.EnumerateArray())
        {
            // Acceptă atât inlineData cât și inline_data (just in case)
            if (part.TryGetProperty("inlineData", out var inlineData) ||
                part.TryGetProperty("inline_data", out inlineData))
            {
                if (!inlineData.TryGetProperty("data", out var dataElement))
                    continue;

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

        _logger.LogError("No inline image data found in Gemini response. Parts count: {PartsCount}", parts.GetArrayLength());
        throw new InvalidOperationException("No inline image data found in Gemini response. The model may not have generated an image.");
    }
}
