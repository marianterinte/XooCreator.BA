namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// OpenAI TTS with caller-provided API key.
/// Used by Generate Full Story Draft without modifying existing OpenAIAudioGeneratorService.
/// </summary>
public interface IOpenAIAudioWithApiKey
{
    Task<(byte[] AudioData, string Format)> GenerateAudioAsync(
        string text,
        string languageCode,
        string? voiceName = null,
        string? apiKeyOverride = null,
        string? modelOverride = null,
        CancellationToken ct = default);
}
