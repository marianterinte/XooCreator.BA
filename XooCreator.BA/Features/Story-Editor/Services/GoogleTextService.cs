using System.Linq;
using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for generating story text (next page) using Google Gemini.
/// </summary>
public interface IGoogleTextService
{
    /// <summary>
    /// Given the full story JSON, generates the text for the next page
    /// in the story, in the specified language.
    /// </summary>
    /// <param name="storyJson">
    /// The full story JSON (what your FE already has for the story).
    /// </param>
    /// <param name="languageCode">
    /// Language code: e.g. "ro-RO", "en-US", "hu-HU".
    /// </param>
    /// <param name="extraInstructions">
    /// Optional extra style instructions (e.g. "for 7 year old kids").
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>
    /// Plain text for the next page (no JSON, no quotes, no explanations).
    /// </returns>
    Task<string> GenerateNextPageAsync(
        string storyJson,
        string languageCode,
        string? extraInstructions = null,
        CancellationToken ct = default);
}
 
/// <summary>
/// Service for generating story text (next page) using Google Gemini 2.5 Flash.
/// Uses the same Google AI Studio API key as the TTS service.
/// </summary>
public class GoogleTextService : IGoogleTextService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _textEndpoint;
    private readonly ILogger<GoogleTextService> _logger;

    public GoogleTextService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<GoogleTextService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["GoogleAI:ApiKey"]
            ?? throw new InvalidOperationException("GoogleAI:ApiKey is not configured in appsettings.json");
        _textEndpoint = configuration["GoogleAI:Text:Endpoint"]
            ?? throw new InvalidOperationException("GoogleAI:Text:Endpoint is not configured in appsettings.json");
        _logger = logger;
    }

    public async Task<string> GenerateNextPageAsync(
        string storyJson,
        string languageCode,
        string? extraInstructions = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(storyJson))
            throw new ArgumentException("Story JSON cannot be empty", nameof(storyJson));

        // Optimizare: extragem doar ultimele 3 pagini + summary pentru a reduce tokenii
        var optimizedJson = ExtractOptimizedStoryContext(storyJson);

        // Prompt engineering – system instruction: ce vrei TU de la model
        var systemInstructionText = BuildSystemInstruction(languageCode, extraInstructions);

        var requestBody = new
        {
            // Instrucțiuni globale pentru model (cum să se comporte)
            system_instruction = new
            {
                parts = new[]
                {
                    new { text = systemInstructionText }
                }
            },

            // Conținutul efectiv: optimizat pentru tokeni
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text = $"Story: {optimizedJson}\n\nGenerate next page text only."
                        }
                    }
                }
            },

            // Config de generare – optimizat pentru costuri
            generationConfig = new
            {
                temperature = 0.9,      // mai creativ un pic
                topP = 0.9,
                topK = 40,
                maxOutputTokens = 1024,  // Optimizat: suficient pentru o pagină de poveste (200-500 tokeni reali)
                response_mime_type = "text/plain"
            },

            // Modelul text
            model = "gemini-2.5-flash"
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, _textEndpoint)
        {
            Content = content
        };
        request.Headers.Add("x-goog-api-key", _apiKey);

        try
        {
            _logger.LogInformation(
                "Calling Gemini Text API for next page generation. Language: {LanguageCode}",
                languageCode);

            using var response = await _httpClient.SendAsync(request, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "Gemini Text API returned {StatusCode}: {Body}",
                    (int)response.StatusCode,
                    responseContent);

                response.EnsureSuccessStatusCode();
            }

            return ExtractTextFromResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating next story page.");
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
    /// Builds the system instruction - optimized for token efficiency.
    /// </summary>
    private static string BuildSystemInstruction(string languageCode, string? extraInstructions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("Children's story. Continue with NEXT PAGE text only. Keep consistent. Plain text.");
        sb.AppendLine($"Language: {languageCode}.");

        if (!string.IsNullOrWhiteSpace(extraInstructions))
        {
            sb.AppendLine($"Instructions: {extraInstructions}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Extracts the concatenated text from Gemini's GenerateContentResponse JSON.
    /// </summary>
    private string ExtractTextFromResponse(string responseJson)
    {
        using var doc = JsonDocument.Parse(responseJson);
        var root = doc.RootElement;

        // Check for prompt feedback (safety issues)
        if (root.TryGetProperty("promptFeedback", out var promptFeedback))
        {
            if (promptFeedback.TryGetProperty("blockReason", out var blockReason))
            {
                var reason = blockReason.GetString();
                _logger.LogWarning("Gemini blocked the prompt. Reason: {BlockReason}", reason);
                throw new InvalidOperationException($"Content generation was blocked by Gemini safety filters. Reason: {reason}");
            }
        }

        if (!root.TryGetProperty("candidates", out var candidates) ||
            candidates.GetArrayLength() == 0)
        {
            _logger.LogError("Gemini response has no candidates. Full response: {Response}", responseJson);
            throw new InvalidOperationException("No candidates returned from Gemini. The response may have been blocked or failed.");
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
                _logger.LogInformation("Gemini reached max output tokens limit. Generated content will be extracted.");
                // Continue to extract content - this is not an error
            }
            else if (reason != "STOP" && reason != null)
            {
                _logger.LogWarning("Gemini finished with reason: {FinishReason}", reason);
                
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
                    
                    var errorMsg = $"Content generation was blocked by safety filters. Finish reason: {reason}";
                    if (safetyDetails.Length > 0)
                        errorMsg += $"\nSafety ratings: {safetyDetails}";
                    
                    throw new InvalidOperationException(errorMsg);
                }
                
                throw new InvalidOperationException($"Content generation finished unexpectedly. Reason: {reason}");
            }
        }

        // Check if content exists
        if (!candidate.TryGetProperty("content", out var contentElement))
        {
            _logger.LogError("Gemini candidate has no content property. Full response: {Response}", responseJson);
            throw new InvalidOperationException("Gemini response candidate does not contain a content property.");
        }

        // Check if parts exist
        if (!contentElement.TryGetProperty("parts", out var parts) ||
            parts.GetArrayLength() == 0)
        {
            _logger.LogError("Gemini response has no content parts. Candidate: {Candidate}", candidate.GetRawText());
            throw new InvalidOperationException("Gemini response does not contain content parts. The content may have been blocked or the response format is unexpected.");
        }

        // Extract text from parts
        var sb = new StringBuilder();
        foreach (var part in parts.EnumerateArray())
        {
            if (part.TryGetProperty("text", out var textElement))
            {
                var chunk = textElement.GetString();
                if (!string.IsNullOrEmpty(chunk))
                    sb.Append(chunk);
            }
        }

        var result = sb.ToString().Trim();
        if (string.IsNullOrEmpty(result))
        {
            _logger.LogWarning("Gemini returned empty text. Parts count: {PartsCount}", parts.GetArrayLength());
            throw new InvalidOperationException("Generated text is empty. The model may have returned no text content.");
        }

        return result;
    }
}
