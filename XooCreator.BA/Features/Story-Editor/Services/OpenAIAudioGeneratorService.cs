using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for interacting with OpenAI TTS.
/// </summary>
public interface IOpenAIAudioGeneratorService
{
    /// <summary>
    /// Generates audio from text using OpenAI TTS.
    /// </summary>
    Task<(byte[] AudioData, string Format)> GenerateAudioAsync(
        string text,
        string languageCode,
        string? voiceName = null,
        CancellationToken ct = default);
}

public class OpenAIAudioGeneratorService : IOpenAIAudioGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _organizationId;
    private readonly string _model;
    private readonly string _defaultVoice;
    private readonly string _baseUrl;
    private readonly ILogger<OpenAIAudioGeneratorService> _logger;

    public OpenAIAudioGeneratorService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAIAudioGeneratorService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured in appsettings.json");
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
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));

        if (text.Length > 4096)
            throw new ArgumentException("Text exceeds maximum length of 4096 characters", nameof(text));

        voiceName ??= GetDefaultVoiceForLanguage(languageCode);

        var requestBody = new
        {
            model = _model,
            input = text,
            voice = voiceName,
            response_format = "mp3"
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_baseUrl}/audio/speech")
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
                "Calling OpenAI TTS API. Language: {LanguageCode}, Voice: {Voice}, Model: {Model}",
                languageCode, voiceName, _model);

            using var response = await _httpClient.SendAsync(request, ct);

            if (!response.IsSuccessStatusCode)
            {
                var responseContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError(
                    "OpenAI TTS API returned {StatusCode}: {Body}",
                    (int)response.StatusCode,
                    responseContent);
                response.EnsureSuccessStatusCode();
            }

            var audioBytes = await response.Content.ReadAsByteArrayAsync(ct);
            return (audioBytes, "mp3");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating audio with OpenAI TTS.");
            throw;
        }
    }

    private string GetDefaultVoiceForLanguage(string languageCode)
    {
        // OpenAI TTS voices: alloy, echo, fable, onyx, nova, shimmer
        // Map language codes to appropriate voices
        return languageCode.ToLowerInvariant() switch
        {
            "ro-ro" => "nova",      // Romanian - softer, warmer voice
            "hu-hu" => "echo",      // Hungarian
            "en-us" => "alloy",     // English - default
            _ => _defaultVoice
        };
    }
}

