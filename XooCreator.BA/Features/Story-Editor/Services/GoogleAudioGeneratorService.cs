using System.Net;
using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for interacting with Google Gemini TTS (via Google AI Studio).
/// Uses Gemini TTS to generate single-speaker audio.
/// </summary>
public interface IGoogleAudioGeneratorService
{
    /// <summary>
    /// Generates audio from text using Gemini TTS.
    /// </summary>
    /// <param name="text">Text to convert to speech.</param>
    /// <param name="languageCode">
    /// Language code (e.g., "ro-RO", "en-US", "hu-HU").
    /// Currently only used for your own voice-mapping logic.
    /// </param>
    /// <param name="voiceName">
    /// Optional Gemini TTS voice name (e.g., "Sulafat", "Zephyr").
    /// If not provided, defaults based on language, with "Sulafat" as fallback.
    /// </param>
    /// <param name="styleInstructions">
    /// Optional style instructions for tone (e.g., "Read aloud in a warm and friendly tone, like telling stories to children").
    /// If not provided, uses default from configuration.
    /// </param>
    /// <param name="apiKeyOverride">
    /// Optional API key override. If provided, uses this instead of the configured API key.
    /// </param>
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Audio bytes and format ("wav").</returns>
    Task<(byte[] AudioData, string Format)> GenerateAudioAsync(
        string text,
        string languageCode,
        string? voiceName = null,
        string? styleInstructions = null,
        string? apiKeyOverride = null,
        CancellationToken ct = default);
}

public class GoogleAudioGeneratorService : IGoogleAudioGeneratorService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _ttsEndpoint;
    private readonly string _defaultVoice;
    private readonly string? _defaultStyleInstructions;
    private readonly ILogger<GoogleAudioGeneratorService> _logger;

    public GoogleAudioGeneratorService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<GoogleAudioGeneratorService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        // ApiKey is no longer read from config; audio export requires the user to provide it in the Generate Audio modal.
        _apiKey = configuration["GoogleAI:ApiKey"] ?? string.Empty;
        _ttsEndpoint = configuration["GoogleAI:Tts:Endpoint"]
            ?? throw new InvalidOperationException("GoogleAI:Tts:Endpoint is not configured in appsettings.json");
        _defaultVoice = configuration["GoogleAI:Tts:DefaultVoice"] ?? "Sulafat";
        _defaultStyleInstructions = configuration["GoogleAI:Tts:StyleInstructions"];
        _logger = logger;
    }

    public async Task<(byte[] AudioData, string Format)> GenerateAudioAsync(
        string text,
        string languageCode,
        string? voiceName = null,
        string? styleInstructions = null,
        string? apiKeyOverride = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(text))
            throw new ArgumentException("Text cannot be empty", nameof(text));

        // Limiță de siguranță – poți ajusta după cum ai nevoie
        if (text.Length > 5000)
            throw new ArgumentException("Text exceeds maximum length of 5000 characters", nameof(text));

        // Default: Sulafat, sau altceva în funcție de limbă dacă vrei
        voiceName ??= GetDefaultVoiceForLanguage(languageCode);
        
        // Folosește style instructions din parametru sau din configurație
        var finalStyleInstructions = styleInstructions ?? _defaultStyleInstructions;

        // Construiește textul final: dacă avem style instructions, le adăugăm înainte de text
        var finalText = string.IsNullOrWhiteSpace(finalStyleInstructions)
            ? text
            : $"{finalStyleInstructions}\n\n{text}";

        var requestBody = new
        {
            // Textul de sintetizat (cu style instructions incluse dacă există)
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = finalText }
                    }
                }
            },

            // Configurare pentru TTS single-speaker
            generationConfig = new
            {
                responseModalities = new[] { "AUDIO" },
                speechConfig = new
                {
                    voiceConfig = new
                    {
                        prebuiltVoiceConfig = new
                        {
                            voiceName = voiceName // ex: "Sulafat"
                        }
                    }
                }
            },

            // Modelul TTS – folosim Flash pentru costuri mai mici
            // Notă: modelul este determinat de endpoint-ul din appsettings.json
            // Nu specificăm model aici pentru a evita conflicte cu endpoint-ul
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, _ttsEndpoint)
        {
            Content = content
        };

        // API key: must be provided via apiKeyOverride (from Generate Audio modal). No longer uses config.
        var apiKeyToUse = !string.IsNullOrWhiteSpace(apiKeyOverride) ? apiKeyOverride : _apiKey;
        if (string.IsNullOrWhiteSpace(apiKeyToUse))
        {
            throw new InvalidOperationException(
                "Google API key is required for audio generation. Please provide it in the Generate Audio modal (required for full audio export).");
        }
        request.Headers.Add("x-goog-api-key", apiKeyToUse);

        try
        {
            _logger.LogInformation(
                "Calling Gemini TTS with language: {LanguageCode}, voice: {VoiceName}, hasStyleInstructions: {HasStyle}",
                languageCode, voiceName, !string.IsNullOrWhiteSpace(finalStyleInstructions));

            using var response = await _httpClient.SendAsync(request, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini TTS returned {StatusCode}: {ErrorContent}",
                    response.StatusCode, responseContent);

                // Check for quota/rate limit errors
                if (response.StatusCode == HttpStatusCode.TooManyRequests || 
                    response.StatusCode == HttpStatusCode.ServiceUnavailable)
                {
                    var quotaError = ExtractQuotaErrorMessage(responseContent);
                    throw new InvalidOperationException(quotaError);
                }

                // Check response body for quota-related errors
                if (!string.IsNullOrWhiteSpace(responseContent))
                {
                    try
                    {
                        using var errorDoc = JsonDocument.Parse(responseContent);
                        if (errorDoc.RootElement.TryGetProperty("error", out var errorObj))
                        {
                            if (errorObj.TryGetProperty("status", out var status))
                            {
                                var statusStr = status.GetString() ?? "";
                                if (statusStr.Contains("RESOURCE_EXHAUSTED") || 
                                    statusStr.Contains("RATE_LIMIT_EXCEEDED") ||
                                    statusStr.Contains("QUOTA_EXCEEDED"))
                                {
                                    var quotaError = ExtractQuotaErrorMessage(responseContent);
                                    throw new InvalidOperationException(quotaError);
                                }
                            }
                            
                            if (errorObj.TryGetProperty("message", out var message))
                            {
                                var messageStr = message.GetString() ?? "";
                                if (messageStr.Contains("quota", StringComparison.OrdinalIgnoreCase) ||
                                    messageStr.Contains("rate limit", StringComparison.OrdinalIgnoreCase) ||
                                    messageStr.Contains("resource exhausted", StringComparison.OrdinalIgnoreCase))
                                {
                                    var quotaError = ExtractQuotaErrorMessage(responseContent);
                                    throw new InvalidOperationException(quotaError);
                                }
                            }
                        }
                    }
                    catch (JsonException)
                    {
                        // If JSON parsing fails, continue with normal error handling
                    }
                }

                response.EnsureSuccessStatusCode();
            }

            using var doc = JsonDocument.Parse(responseContent);
            var root = doc.RootElement;

            // audioContent = candidates[0].content.parts[0].inlineData.data (base64 PCM)
            if (!root.TryGetProperty("candidates", out var candidates) ||
                candidates.GetArrayLength() == 0)
            {
                throw new InvalidOperationException("No candidates returned from Gemini TTS.");
            }

            var candidate = candidates[0];

            if (!candidate.TryGetProperty("content", out var contentElement))
                throw new InvalidOperationException("Response does not contain content.");

            if (!contentElement.TryGetProperty("parts", out var parts) ||
                parts.GetArrayLength() == 0)
            {
                throw new InvalidOperationException("Response does not contain parts.");
            }

            var firstPart = parts[0];

            if (!firstPart.TryGetProperty("inlineData", out var inlineData) ||
                !inlineData.TryGetProperty("data", out var dataElement))
            {
                throw new InvalidOperationException("Response does not contain inlineData.data.");
            }

            var base64Audio = dataElement.GetString();
            if (string.IsNullOrWhiteSpace(base64Audio))
                throw new InvalidOperationException("Audio content is empty.");

            // Gemini TTS dă PCM 16-bit, 24kHz, mono; îl ambalăm într-un WAV.
            var pcmBytes = Convert.FromBase64String(base64Audio);
            var wavBytes = WrapPcmAsWav(pcmBytes, sampleRate: 24000, bitsPerSample: 16, channels: 1);

            return (wavBytes, "wav");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing Gemini TTS response");
            throw;
        }
    }

    /// <summary>
    /// Extracts a user-friendly quota error message from the API response.
    /// </summary>
    private static string ExtractQuotaErrorMessage(string responseContent)
    {
        try
        {
            using var doc = JsonDocument.Parse(responseContent);
            if (doc.RootElement.TryGetProperty("error", out var errorObj))
            {
                if (errorObj.TryGetProperty("message", out var message))
                {
                    var messageStr = message.GetString() ?? "";
                    if (!string.IsNullOrWhiteSpace(messageStr))
                    {
                        return $"Google API quota limit reached: {messageStr}. Please try again later or use a different API key.";
                    }
                }
            }
        }
        catch (JsonException)
        {
            // If JSON parsing fails, return generic message
        }

        return "Google API quota limit has been reached. Please try again later or use a different API key in the modal.";
    }

    /// <summary>
    /// Default voice mapping. Uses configured default voice, but can be customized per-language.
    /// </summary>
    private string GetDefaultVoiceForLanguage(string languageCode)
    {
        // Folosește voice-ul default din configurație
        // Poți personaliza pe viitor pe limbă, ex:
        // "ro-ro" => "Sulafat", "en-us" => "Zephyr", etc.
        _ = languageCode; // silence unused param warning if you don't change mapping yet
        return _defaultVoice;
    }

    /// <summary>
    /// Wrap raw PCM (16-bit, little-endian) into a standard WAV header.
    /// </summary>
    private static byte[] WrapPcmAsWav(byte[] pcmData, int sampleRate, short bitsPerSample, short channels)
    {
        using var ms = new MemoryStream();
        using var writer = new BinaryWriter(ms);

        var subChunk2Size = pcmData.Length;
        var chunkSize = 36 + subChunk2Size;
        short audioFormat = 1; // PCM
        var byteRate = sampleRate * channels * (bitsPerSample / 8);
        short blockAlign = (short)(channels * (bitsPerSample / 8));

        // RIFF header
        writer.Write(Encoding.ASCII.GetBytes("RIFF"));
        writer.Write(chunkSize);
        writer.Write(Encoding.ASCII.GetBytes("WAVE"));

        // fmt subchunk
        writer.Write(Encoding.ASCII.GetBytes("fmt "));
        writer.Write(16);                 // Subchunk1Size for PCM
        writer.Write(audioFormat);
        writer.Write(channels);
        writer.Write(sampleRate);
        writer.Write(byteRate);
        writer.Write(blockAlign);
        writer.Write(bitsPerSample);

        // data subchunk
        writer.Write(Encoding.ASCII.GetBytes("data"));
        writer.Write(subChunk2Size);
        writer.Write(pcmData);

        writer.Flush();
        return ms.ToArray();
    }
}
