using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for interacting with Google Gemini TTS (via Google AI Studio).
/// Uses Gemini TTS to generate single-speaker audio.
/// </summary>
public interface IGoogleTtsService
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
    /// <param name="ct">Cancellation token.</param>
    /// <returns>Audio bytes and format ("wav").</returns>
    Task<(byte[] AudioData, string Format)> GenerateAudioAsync(
        string text,
        string languageCode,
        string? voiceName = null,
        CancellationToken ct = default);
}

public class GoogleTtsService : IGoogleTtsService
{
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _ttsEndpoint;
    private readonly ILogger<GoogleTtsService> _logger;

    public GoogleTtsService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<GoogleTtsService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["GoogleAI:ApiKey"]
            ?? throw new InvalidOperationException("GoogleAI:ApiKey is not configured in appsettings.json");
        _ttsEndpoint = configuration["GoogleAI:Tts:Endpoint"]
            ?? throw new InvalidOperationException("GoogleAI:Tts:Endpoint is not configured in appsettings.json");
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

        // Limiță de siguranță – poți ajusta după cum ai nevoie
        if (text.Length > 5000)
            throw new ArgumentException("Text exceeds maximum length of 5000 characters", nameof(text));

        // Default: Sulafat, sau altceva în funcție de limbă dacă vrei
        voiceName ??= GetDefaultVoiceForLanguage(languageCode);

        var requestBody = new
        {
            // Textul de sintetizat
            contents = new[]
            {
                new
                {
                    parts = new[]
                    {
                        new { text = text }
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

            // Modelul TTS – în AI Studio vezi exact „Gemini 2.5 Pro Preview TTS”
            model = "gemini-2.5-pro-preview-tts"
        };

        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");

        using var request = new HttpRequestMessage(HttpMethod.Post, _ttsEndpoint)
        {
            Content = content
        };

        // API key de la aistudio.google.com
        request.Headers.Add("x-goog-api-key", _apiKey);

        try
        {
            _logger.LogInformation(
                "Calling Gemini TTS with language: {LanguageCode}, voice: {VoiceName}",
                languageCode, voiceName);

            using var response = await _httpClient.SendAsync(request, ct);
            var responseContent = await response.Content.ReadAsStringAsync(ct);

            if (!response.IsSuccessStatusCode)
            {
                _logger.LogError("Gemini TTS returned {StatusCode}: {ErrorContent}",
                    response.StatusCode, responseContent);

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
    /// Default voice mapping. Right now we just return "Sulafat" for all,
    /// but you can tweak per-language later (e.g., children's voice for ro-RO).
    /// </summary>
    private static string GetDefaultVoiceForLanguage(string languageCode)
    {
        // Deocamdată totul pe Sulafat, așa cum ai cerut.
        // Poți personaliza pe viitor, ex:
        // "ro-ro" => "Sulafat", "en-us" => "Zephyr", etc.
        _ = languageCode; // silence unused param warning if you don't change mapping yet
        return "Sulafat";
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
