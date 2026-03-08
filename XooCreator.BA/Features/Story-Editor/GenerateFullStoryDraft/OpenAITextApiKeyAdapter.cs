using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Adapter that performs OpenAI full-story text generation using a caller-provided API key.
/// Does not modify existing OpenAITextService; used only by Generate Full Story Draft.
/// </summary>
public class OpenAITextApiKeyAdapter : IOpenAITextWithApiKey
{
    private readonly HttpClient _httpClient;
    private readonly string _organizationId;
    private readonly string _model;
    private readonly string _baseUrl;
    private readonly ILogger<OpenAITextApiKeyAdapter> _logger;

    public OpenAITextApiKeyAdapter(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAITextApiKeyAdapter> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _organizationId = configuration["OpenAI:OrganizationId"] ?? string.Empty;
        _model = configuration["OpenAI:Text:Model"] ?? "gpt-4o-mini";
        _baseUrl = configuration["OpenAI:Text:BaseUrl"] ?? "https://api.openai.com/v1";
        _logger = logger;
    }

    public async Task<string> GenerateFullStoryTextAsync(
        string title,
        string summary,
        string languageCode,
        int numberOfPages,
        List<string>? ageGroupIds = null,
        List<string>? topicIds = null,
        string? storyInstructions = null,
        string? apiKeyOverride = null,
        string? modelOverride = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(apiKeyOverride))
            throw new ArgumentException("API key is required for Generate Full Story Draft.", nameof(apiKeyOverride));
        if (numberOfPages < 1 || numberOfPages > 10)
            throw new ArgumentException("Number of pages must be between 1 and 10", nameof(numberOfPages));
        if (string.IsNullOrWhiteSpace(title))
            throw new ArgumentException("Title cannot be empty", nameof(title));
        if (string.IsNullOrWhiteSpace(summary))
            throw new ArgumentException("Summary cannot be empty", nameof(summary));

        var systemMessage = BuildFullStorySystemInstruction(languageCode, numberOfPages, ageGroupIds, topicIds, storyInstructions);
        var userMessage = BuildFullStoryUserMessage(title, summary, numberOfPages);
        var model = modelOverride ?? _model;

        var requestBody = new
        {
            model,
            messages = new[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage }
            },
            temperature = 0.9,
            max_tokens = 8192
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/chat/completions") { Content = content };

        request.Headers.Add("Authorization", $"Bearer {apiKeyOverride}");
        if (!string.IsNullOrWhiteSpace(_organizationId))
            request.Headers.Add("OpenAI-Organization", _organizationId);

        _logger.LogInformation(
            "OpenAI full story text (user key). Language: {LanguageCode}, Pages: {Pages}, Model: {Model}",
            languageCode, numberOfPages, model);

        using var response = await _httpClient.SendAsync(request, ct);
        var responseContent = await response.Content.ReadAsStringAsync(ct);

        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError("OpenAI Text API returned {StatusCode}: {Body}", (int)response.StatusCode, responseContent);
            response.EnsureSuccessStatusCode();
        }

        return ExtractTextFromResponse(responseContent);
    }

    private static string BuildFullStorySystemInstruction(
        string languageCode,
        int numberOfPages,
        List<string>? ageGroupIds,
        List<string>? topicIds,
        string? storyInstructions)
    {
        var sb = new StringBuilder();
        sb.AppendLine("You are a children's story writer. Reply with plain text only, using exactly these section markers.");
        sb.AppendLine("Format:");
        sb.AppendLine("###T");
        sb.AppendLine("[Story title on one line]");
        sb.AppendLine("###S");
        sb.AppendLine("[Story summary in one short paragraph]");
        for (var i = 1; i <= numberOfPages; i++)
        {
            sb.AppendLine($"###P{i}");
            sb.AppendLine($"[Page {i} text; multiple lines allowed]");
        }
        sb.AppendLine($"Language: {languageCode}. Generate exactly {numberOfPages} pages.");
        if (ageGroupIds != null && ageGroupIds.Count > 0)
            sb.AppendLine($"Target age groups: {string.Join(", ", ageGroupIds)}");
        if (topicIds != null && topicIds.Count > 0)
            sb.AppendLine($"Topics: {string.Join(", ", topicIds)}");
        if (!string.IsNullOrWhiteSpace(storyInstructions))
            sb.AppendLine($"Additional instructions: {storyInstructions}");
        return sb.ToString();
    }

    private static string BuildFullStoryUserMessage(string title, string summary, int numberOfPages)
    {
        return $"Seed/summary for the story:\n{summary}\n\nSuggested title: {title}\n\n" +
               $"Generate the complete story in the requested format with exactly {numberOfPages} pages. " +
               "Use ###T for title, ###S for summary, ###P1, ###P2, etc. for each page. Each page can be multiple lines.";
    }

    private static string ExtractTextFromResponse(string responseJson)
    {
        using var doc = JsonDocument.Parse(responseJson);
        var root = doc.RootElement;
        if (root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
        {
            var first = choices.EnumerateArray().FirstOrDefault();
            if (first.TryGetProperty("message", out var message) && message.TryGetProperty("content", out var content))
                return (content.GetString() ?? string.Empty).Trim();
        }
        throw new InvalidOperationException("Failed to extract text from OpenAI response");
    }
}
