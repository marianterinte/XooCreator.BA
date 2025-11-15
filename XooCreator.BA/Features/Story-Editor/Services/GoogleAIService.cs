using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for interacting with Google AI Studio (aistudiogoogle.com) APIs.
/// Handles Text-to-Speech and future Text Generation functionality.
/// </summary>
public interface IGoogleAIService
{
    /// <summary>
    /// Generates audio from text using Google Text-to-Speech API.
    /// </summary>
    /// <param name="text">Text to convert to speech</param>
    /// <param name="languageCode">Language code (e.g., "ro-RO", "en-US", "hu-HU")</param>
    /// <param name="voiceName">Optional voice name. If not provided, defaults based on language</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Base64-encoded audio data and format</returns>
    Task<(byte[] AudioData, string Format)> GenerateAudioAsync(
        string text,
        string languageCode,
        string? voiceName = null,
        CancellationToken ct = default);
}

public class GoogleAIService : IGoogleAIService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly ILogger<GoogleAIService> _logger;

    public GoogleAIService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<GoogleAIService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["GoogleAI:ApiKey"] 
            ?? throw new InvalidOperationException("GoogleAI:ApiKey is not configured in appsettings.json");
        _logger = logger;
    }

    public async Task<(byte[] AudioData, string Format)> GenerateAudioAsync(
        string text,
        string languageCode,
        string? voiceName = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            throw new ArgumentException("Text cannot be empty", nameof(text));
        }

        if (text.Length > 5000)
        {
            throw new ArgumentException("Text exceeds maximum length of 5000 characters", nameof(text));
        }

        // Select voice based on language if not provided
        voiceName ??= GetDefaultVoiceForLanguage(languageCode);

        var requestBody = new
        {
            input = new { text = text },
            voice = new
            {
                languageCode = languageCode,
                name = voiceName,
                ssmlGender = "NEUTRAL"
            },
            audioConfig = new
            {
                audioEncoding = "LINEAR16", // WAV format
                sampleRateHertz = 24000
            }
        };

        var json = JsonSerializer.Serialize(requestBody);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        // Google Cloud Text-to-Speech API requires API key as query parameter
        // NOTE: This must be a Google Cloud API key (from console.cloud.google.com), 
        // NOT a Google AI Studio API key (from aistudio.google.com)
        var url = $"https://texttospeech.googleapis.com/v1/text:synthesize?key={Uri.EscapeDataString(_apiKey)}";
        
        var request = new HttpRequestMessage(HttpMethod.Post, url)
        {
            Content = content
        };
        
        // Set content type explicitly
        request.Headers.Add("Content-Type", "application/json");

        try
        {
            _logger.LogInformation("Calling Google Text-to-Speech API with language: {LanguageCode}, voice: {VoiceName}", languageCode, voiceName);
            
            var response = await _httpClient.SendAsync(request, ct);
            
            if (!response.IsSuccessStatusCode)
            {
                var errorContent = await response.Content.ReadAsStringAsync(ct);
                _logger.LogError("Google Text-to-Speech API returned {StatusCode}: {ErrorContent}", response.StatusCode, errorContent);
            }
            
            response.EnsureSuccessStatusCode();

            var responseContent = await response.Content.ReadAsStringAsync(ct);
            var responseJson = JsonDocument.Parse(responseContent);

            if (!responseJson.RootElement.TryGetProperty("audioContent", out var audioContentElement))
            {
                throw new InvalidOperationException("Response does not contain audioContent");
            }

            var base64Audio = audioContentElement.GetString();
            if (string.IsNullOrWhiteSpace(base64Audio))
            {
                throw new InvalidOperationException("Audio content is empty");
            }

            var audioBytes = Convert.FromBase64String(base64Audio);
            return (audioBytes, "wav");
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError(ex, "Failed to call Google Text-to-Speech API. Status: {StatusCode}, Message: {Message}", 
                ex.Data.Contains("StatusCode") ? ex.Data["StatusCode"] : "Unknown", ex.Message);
            
            // Include more details in the error message
            var errorMessage = ex.Message;
            if (ex.Data.Contains("StatusCode"))
            {
                errorMessage += $" Status Code: {ex.Data["StatusCode"]}";
            }
            
            throw new InvalidOperationException($"Failed to generate audio: {errorMessage}", ex);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing audio generation response");
            throw;
        }
    }

    private static string GetDefaultVoiceForLanguage(string languageCode)
    {
        return languageCode.ToLowerInvariant() switch
        {
            "ro-ro" => "ro-RO-Standard-A",
            "en-us" => "en-US-Standard-C",
            "hu-hu" => "hu-HU-Standard-A",
            _ => "en-US-Standard-C" // Default fallback
        };
    }
}

