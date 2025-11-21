using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for generating story text (next page) using OpenAI.
/// </summary>
public interface IOpenAITextService
{
    /// <summary>
    /// Given the full story JSON, generates the text for the next page
    /// in the story, in the specified language.
    /// Used for incremental generation (Refresh AI Text, Generate Next Page).
    /// </summary>
    Task<string> GenerateNextPageAsync(
        string storyJson,
        string languageCode,
        string? extraInstructions = null,
        int? currentPageNumber = null,
        int? totalPages = null,
        CancellationToken ct = default);

    /// <summary>
    /// Generates a complete story with the specified number of pages in a single request.
    /// Used for full story generation from scratch.
    /// Returns the complete story text that needs to be parsed into pages.
    /// </summary>
    Task<string> GenerateFullStoryTextAsync(
        string title,
        string summary,
        string languageCode,
        int numberOfPages,
        List<string>? ageGroupIds = null,
        List<string>? topicIds = null,
        string? storyInstructions = null,
        CancellationToken ct = default);
}

/// <summary>
/// Service for generating story text (next page) using OpenAI GPT models.
/// </summary>
public class OpenAITextService : IOpenAITextService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _organizationId;
    private readonly string _model;
    private readonly string _baseUrl;
    private readonly ILogger<OpenAITextService> _logger;

    public OpenAITextService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAITextService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured in appsettings.json");
        _organizationId = configuration["OpenAI:OrganizationId"] ?? string.Empty;
        _model = configuration["OpenAI:Text:Model"] ?? "gpt-4o-mini";
        _baseUrl = configuration["OpenAI:Text:BaseUrl"] ?? "https://api.openai.com/v1";
        _logger = logger;
    }

    public async Task<string> GenerateNextPageAsync(
        string storyJson,
        string languageCode,
        string? extraInstructions = null,
        int? currentPageNumber = null,
        int? totalPages = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(storyJson))
            throw new ArgumentException("Story JSON cannot be empty", nameof(storyJson));

        // Optimizare: extragem doar ultimele 3 pagini + summary pentru a reduce tokenii
        var optimizedJson = ExtractOptimizedStoryContext(storyJson);

        // Construim system message
        var systemMessage = BuildSystemInstruction(languageCode, extraInstructions, currentPageNumber, totalPages);

        // Construim user message
        var userMessage = BuildUserMessage(optimizedJson, currentPageNumber, totalPages);

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage }
            },
            temperature = 0.9,
            max_tokens = 1024
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions")
        {
            Content = content
        };
        
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        if (!string.IsNullOrWhiteSpace(_organizationId))
        {
            request.Headers.Add("OpenAI-Organization", _organizationId);
        }

        try
        {
            _logger.LogInformation(
                "Calling OpenAI Text API for next page generation. Language: {LanguageCode}, Model: {Model}",
                languageCode, _model);

            using var response = await _httpClient.SendAsync(request, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "OpenAI Text API returned {StatusCode}: {Body}",
                    (int)response.StatusCode,
                    responseContent);
                response.EnsureSuccessStatusCode();
            }

            return ExtractTextFromResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating next story page with OpenAI.");
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

            var storyId = root.TryGetProperty("storyId", out var sid) ? sid.GetString() : null;
            var title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
            var summary = root.TryGetProperty("summary", out var s) ? s.GetString() : null;

            var allTiles = new List<JsonElement>();
            if (root.TryGetProperty("tiles", out var tiles) && tiles.ValueKind == JsonValueKind.Array)
            {
                foreach (var tile in tiles.EnumerateArray())
                {
                    if (tile.TryGetProperty("type", out var type) &&
                        type.GetString()?.Equals("page", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        allTiles.Add(tile);
                    }
                }
            }

            var lastPages = allTiles
                .OrderBy(t => t.TryGetProperty("sortOrder", out var so) ? so.GetInt32() : int.MaxValue)
                .TakeLast(3)
                .ToList();

            var optimized = new Dictionary<string, object?>();
            if (!string.IsNullOrEmpty(storyId))
                optimized["storyId"] = storyId;
            if (!string.IsNullOrEmpty(title))
                optimized["title"] = title;
            if (!string.IsNullOrEmpty(summary))
                optimized["summary"] = summary;

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

                optimizedTiles.Add(tileObj);
            }

            optimized["tiles"] = optimizedTiles;

            return JsonSerializer.Serialize(optimized, new JsonSerializerOptions
            {
                WriteIndented = false
            });
        }
        catch (Exception)
        {
            return fullStoryJson;
        }
    }

    /// <summary>
    /// Builds the system instruction - optimized for token efficiency.
    /// </summary>
    private static string BuildSystemInstruction(string languageCode, string? extraInstructions, int? currentPageNumber, int? totalPages)
    {
        var sb = new StringBuilder();
        
        if (currentPageNumber.HasValue && totalPages.HasValue)
        {
            sb.AppendLine($"Children's story. Generate ONLY page {currentPageNumber.Value} of {totalPages.Value} total pages.");
            sb.AppendLine($"This is page {currentPageNumber.Value} out of {totalPages.Value}. Generate approximately 150-200 words for THIS PAGE ONLY.");
            sb.AppendLine("Do NOT generate the entire story. Do NOT generate other pages. Generate ONLY the text for this specific page.");
        }
        else
        {
            sb.AppendLine("Children's story. Continue with NEXT PAGE text only. Keep consistent. Plain text.");
        }
        
        sb.AppendLine($"Language: {languageCode}.");

        if (!string.IsNullOrWhiteSpace(extraInstructions))
        {
            sb.AppendLine($"Instructions: {extraInstructions}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Builds the user message with explicit page information.
    /// </summary>
    private static string BuildUserMessage(string optimizedJson, int? currentPageNumber, int? totalPages)
    {
        if (currentPageNumber.HasValue && totalPages.HasValue)
        {
            return $"Story: {optimizedJson}\n\nGenerate ONLY page {currentPageNumber.Value} of {totalPages.Value}. This is page {currentPageNumber.Value} out of {totalPages.Value} total pages. Generate approximately 150-200 words for this page only. Do not generate the entire story.";
        }
        
        return $"Story: {optimizedJson}\n\nGenerate next page text only.";
    }

    /// <summary>
    /// Generates a complete story with the specified number of pages in a single request.
    /// </summary>
    public async Task<string> GenerateFullStoryTextAsync(
        string title,
        string summary,
        string languageCode,
        int numberOfPages,
        List<string>? ageGroupIds = null,
        List<string>? topicIds = null,
        string? storyInstructions = null,
        CancellationToken ct = default)
    {
        if (numberOfPages < 1 || numberOfPages > 10)
            throw new ArgumentException("Number of pages must be between 1 and 10", nameof(numberOfPages));

        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));

        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Summary cannot be empty", nameof(summary));

        var systemMessage = BuildFullStorySystemInstruction(languageCode, numberOfPages, ageGroupIds, topicIds, storyInstructions);
        var userMessage = BuildFullStoryUserMessage(title, summary, numberOfPages);

        var requestBody = new
        {
            model = _model,
            messages = new[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage }
            },
            temperature = 0.9,
            max_tokens = 4096 // Increased for full story
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions")
        {
            Content = content
        };
        
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        if (!string.IsNullOrWhiteSpace(_organizationId))
        {
            request.Headers.Add("OpenAI-Organization", _organizationId);
        }

        try
        {
            _logger.LogInformation(
                "Calling OpenAI Text API for full story generation. Language: {LanguageCode}, Pages: {Pages}, Model: {Model}",
                languageCode, numberOfPages, _model);

            using var response = await _httpClient.SendAsync(request, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError(
                    "OpenAI Text API returned {StatusCode}: {Body}",
                    (int)response.StatusCode,
                    responseContent);
                response.EnsureSuccessStatusCode();
            }

            return ExtractTextFromResponse(responseContent);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating full story text with OpenAI.");
            throw;
        }
    }

    /// <summary>
    /// Builds the system instruction for full story generation.
    /// </summary>
    private static string BuildFullStorySystemInstruction(
        string languageCode,
        int numberOfPages,
        List<string>? ageGroupIds,
        List<string>? topicIds,
        string? storyInstructions)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Children's story. Generate a complete story with exactly {numberOfPages} pages.");
        sb.AppendLine($"Format your response as: Page 1: [text for page 1]");
        sb.AppendLine($"Page 2: [text for page 2]");
        sb.AppendLine($"... and so on for all {numberOfPages} pages.");
        sb.AppendLine($"Each page should be approximately 150-200 words. Use clear page delimiters: 'Page X:' at the start of each page.");
        sb.AppendLine($"Language: {languageCode}.");

        if (ageGroupIds != null && ageGroupIds.Count > 0)
        {
            sb.AppendLine($"Target age groups: {string.Join(", ", ageGroupIds)}");
        }

        if (topicIds != null && topicIds.Count > 0)
        {
            sb.AppendLine($"Topics: {string.Join(", ", topicIds)}");
        }

        if (!string.IsNullOrWhiteSpace(storyInstructions))
        {
            sb.AppendLine($"Additional instructions: {storyInstructions}");
        }

        return sb.ToString();
    }

    /// <summary>
    /// Builds the user message for full story generation.
    /// </summary>
    private static string BuildFullStoryUserMessage(string title, string summary, int numberOfPages)
    {
        return $"Title: {title}\n\nSummary: {summary}\n\nGenerate a complete children's story with exactly {numberOfPages} pages. " +
               $"Format each page as 'Page X: [text]' where X is the page number (1 to {numberOfPages}). " +
               $"Each page should be approximately 150-200 words. Make sure the story is coherent from beginning to end.";
    }

    /// <summary>
    /// Extracts the text from OpenAI's ChatCompletionResponse JSON.
    /// </summary>
    private string ExtractTextFromResponse(string responseJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseJson);
            var root = doc.RootElement;

            if (root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
            {
                var firstChoice = choices.EnumerateArray().FirstOrDefault();
                if (firstChoice.TryGetProperty("message", out var message))
                {
                    if (message.TryGetProperty("content", out var content))
                    {
                        var text = content.GetString() ?? string.Empty;
                        return text.Trim();
                    }
                }
            }

            _logger.LogWarning("Unexpected OpenAI response format: {Response}", responseJson);
            throw new InvalidOperationException("Failed to extract text from OpenAI response");
        }
        catch (JsonException ex)
        {
            _logger.LogError(ex, "Failed to parse OpenAI response JSON: {Response}", responseJson);
            throw;
        }
    }
}

