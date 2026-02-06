using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GenerateAudioEndpoint
{
    private readonly IGoogleAudioGeneratorService _googleTts;
    private readonly IOpenAIAudioGeneratorService _openAITts;
    private readonly IConfiguration _configuration;
    private readonly IAuth0UserService _auth0;

    public GenerateAudioEndpoint(
        IGoogleAudioGeneratorService googleTts,
        IOpenAIAudioGeneratorService openAITts,
        IConfiguration configuration,
        IAuth0UserService auth0)
    {
        _googleTts = googleTts;
        _openAITts = openAITts;
        _configuration = configuration;
        _auth0 = auth0;
    }

    public record GenerateAudioRequest(
        string Text,
        string TileId,
        string LanguageCode,
        string? VoiceName = null,
        string? StyleInstructions = null,  // Optional: style instructions for tone
        string? Provider = null  // Optional: "Google" or "OpenAI"
    );

    public record GenerateAudioResponse(
        string AudioData, // Base64-encoded audio
        string AudioFormat,
        double Duration, // Estimated duration in seconds
        string PreviewUrl // Data URL for immediate preview
    );

    [Route("/api/{locale}/story-editor/generate-audio")]
    [Authorize]
    public static async Task<Results<Ok<GenerateAudioResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] GenerateAudioEndpoint ep,
        [FromBody] GenerateAudioRequest request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.Text))
        {
            return TypedResults.BadRequest("Text is required");
        }

        if (string.IsNullOrWhiteSpace(request.TileId))
        {
            return TypedResults.BadRequest("TileId is required");
        }

        if (string.IsNullOrWhiteSpace(request.LanguageCode))
        {
            return TypedResults.BadRequest("LanguageCode is required");
        }

        // Determine provider: from request, or from config, or default to OpenAI
        var provider = request.Provider?.ToLowerInvariant() 
            ?? ep._configuration["AIGeneration:Provider"]?.ToLowerInvariant() 
            ?? "openai";

        if (provider != "google" && provider != "openai")
        {
            return TypedResults.BadRequest("Provider must be either 'Google' or 'OpenAI'");
        }

        try
        {
            (byte[] audioData, string format) result;
            
            if (provider == "openai")
            {
                result = await ep._openAITts.GenerateAudioAsync(
                    request.Text,
                    request.LanguageCode,
                    request.VoiceName,
                    ct);
            }
            else
            {
                result = await ep._googleTts.GenerateAudioAsync(
                    request.Text,
                    request.LanguageCode,
                    request.VoiceName,
                    request.StyleInstructions,
                    apiKeyOverride: null, // Use default from config
                    ct);
            }

            var (audioData, format) = result;

            var base64Audio = Convert.ToBase64String(audioData);
            var previewUrl = $"data:audio/{format};base64,{base64Audio}";

            // Estimate duration (rough calculation: 24000 samples/sec, 16-bit = 2 bytes per sample)
            // This is approximate; actual duration may vary
            var estimatedDuration = audioData.Length / (24000.0 * 2);

            return TypedResults.Ok(new GenerateAudioResponse(
                AudioData: base64Audio,
                AudioFormat: format,
                Duration: estimatedDuration,
                PreviewUrl: previewUrl
            ));
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest($"Failed to generate audio: {ex.Message}");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"An error occurred: {ex.Message}");
        }
    }
}

