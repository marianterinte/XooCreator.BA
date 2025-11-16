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

            // Conținutul efectiv: îi dai doar ultimele 3 pagini + summary (optimizat)
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new
                        {
                            text =
                                "Here is the recent story context (last 3 pages + summary) as JSON:\n\n" +
                                optimizedJson +
                                "\n\n--- END OF STORY CONTEXT ---\n\n" +
                                "Read the story so far and imagine the next page. " +
                                "Now generate ONLY the text for the next page."
                        }
                    }
                }
            },

            // Config de generare – poți regla ce vrei aici
            generationConfig = new
            {
                temperature = 0.9,      // mai creativ un pic
                topP = 0.9,
                topK = 40,
                maxOutputTokens = 512,  // arhisuficient pentru o pagină
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

            // Luăm ultimele 3 pagini
            var last3Pages = allTiles
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

            // Convertim ultimele 3 pagini în obiecte simple (doar câmpurile esențiale)
            var optimizedTiles = new List<object>();
            foreach (var tile in last3Pages)
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
    /// Builds the long system instruction explaining exactly what we want.
    /// </summary>
    private static string BuildSystemInstruction(string languageCode, string? extraInstructions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a children's story writing assistant for an app called XooCreator.");
        sb.AppendLine("The user sends you a JSON document that describes a picture book / story.");
        sb.AppendLine("The JSON can contain arrays like \"tiles\" or \"pages\", with text, images and audio references.");
        sb.AppendLine("Your job is to continue the story by inventing the NEXT PAGE ONLY.");
        sb.AppendLine("Read all existing pages carefully and keep consistency for characters, tone and setting.");
        sb.AppendLine("Output ONLY the text for the next page, in the same style and tone.");
        sb.AppendLine("Do NOT repeat text from previous pages.");
        sb.AppendLine("Do NOT output JSON, keys, quotes, explanations, titles, or anything else.");
        sb.AppendLine("Just return the plain story text for the next page.");
        sb.AppendLine($"Write the answer in language: {languageCode}.");

        if (!string.IsNullOrWhiteSpace(extraInstructions))
        {
            sb.AppendLine();
            sb.AppendLine("Additional style instructions from the user:");
            sb.AppendLine(extraInstructions);
        }

        return sb.ToString();
    }

    /// <summary>
    /// Extracts the concatenated text from Gemini's GenerateContentResponse JSON.
    /// </summary>
    private static string ExtractTextFromResponse(string responseJson)
    {
        using var doc = JsonDocument.Parse(responseJson);
        var root = doc.RootElement;

        if (!root.TryGetProperty("candidates", out var candidates) ||
            candidates.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("No candidates returned from Gemini.");
        }

        var candidate = candidates[0];

        if (!candidate.TryGetProperty("content", out var contentElement) ||
            !contentElement.TryGetProperty("parts", out var parts) ||
            parts.GetArrayLength() == 0)
        {
            throw new InvalidOperationException("Gemini response does not contain content parts.");
        }

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
            throw new InvalidOperationException("Generated text is empty.");

        return result;
    }
}
