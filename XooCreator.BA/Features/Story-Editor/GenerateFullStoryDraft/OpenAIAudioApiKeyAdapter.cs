using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Adapter that calls OpenAI TTS with a caller-provided API key.
/// Does not modify existing OpenAIAudioGeneratorService; used only by Generate Full Story Draft.
/// </summary>
public class OpenAIAudioApiKeyAdapter : IOpenAIAudioWithApiKey
{
    private readonly HttpClient _httpClient;
    private readonly string _organizationId;
    private readonly string _model;
    private readonly string _defaultVoice;
    private readonly string _baseUrl;
    private readonly ILogger<OpenAIAudioApiKeyAdapter> _logger;

    public OpenAIAudioApiKeyAdapter(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAIAudioApiKeyAdapter> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _organizationId = configuration["OpenAI:OrganizationId"] ?? string.Empty;
        _model = configuration["OpenAI:Audio:Model"] ?? "gpt-4o-mini-tts";
        _defaultVoice = configuration["OpenAI:Audio:Voice"] ?? "alloy";
        _baseUrl = configuration["OpenAI:Audio:BaseUrl"] ?? "https://api.openai.com/v1";
        _logger = logger;
    }

    public async Task<(byte[] AudioData, string Format)> GenerateAudioAsync(
        string text,
        string languageCode,
        string? voiceName = null,
        string? apiKeyOverride = null,
        string? modelOverride = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));
        if (text.Length > 4096)
            throw new ArgumentException("Text exceeds maximum length of 4096 characters", nameof(text));
        if (string.IsNullOrWhiteSpace(apiKeyOverride))
            throw new ArgumentException("API key is required for Generate Full Story Draft.", nameof(apiKeyOverride));

        voiceName ??= GetDefaultVoiceForLanguage(languageCode);
        var model = modelOverride ?? _model;

        var requestBody = new
        {
            model,
            input = text,
            voice = voiceName,
            response_format = "mp3"
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/audio/speech") { Content = content };

        request.Headers.Add("Authorization", $"Bearer {apiKeyOverride}");
        if (!string.IsNullOrWhiteSpace(_organizationId))
            request.Headers.Add("OpenAI-Organization", _organizationId);

        _logger.LogInformation(
            "OpenAI TTS (user key). Language: {LanguageCode}, Voice: {Voice}, Model: {Model}",
            languageCode, voiceName, model);

        using var response = await _httpClient.SendAsync(request, ct);

        if (!response.IsSuccessStatusCode)
        {
            var responseContent = await response.Content.ReadAsStringAsync(ct);
            _logger.LogError("OpenAI TTS API returned {StatusCode}: {Body}", (int)response.StatusCode, responseContent);
            response.EnsureSuccessStatusCode();
        }

        var audioBytes = await response.Content.ReadAsByteArrayAsync(ct);
        return (audioBytes, "mp3");
    }

    private string GetDefaultVoiceForLanguage(string languageCode)
    {
        return languageCode.ToLowerInvariant() switch
        {
            "ro-ro" => "nova",
            "hu-hu" => "echo",
            "en-us" => "alloy",
            _ => _defaultVoice
        };
    }
}
