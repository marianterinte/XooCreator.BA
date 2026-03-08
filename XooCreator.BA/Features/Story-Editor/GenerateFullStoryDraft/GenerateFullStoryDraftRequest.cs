namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Request for generating a full story draft from the story list (user-provided API key).
/// </summary>
public sealed class GenerateFullStoryDraftRequest
{
    public string ApiKey { get; init; } = string.Empty;
    public string Provider { get; init; } = "Google"; // "Google" | "OpenAI"
    public string TextSeed { get; init; } = string.Empty;
    public string? ImageSeedBase64 { get; init; }
    public string? ImageSeedMimeType { get; init; }
    public string? ImageSeedInstructions { get; init; }
    public string? Instructions { get; init; }
    public int NumberOfPages { get; init; } = 5;
    public bool GenerateImages { get; init; }
    public bool GenerateAudio { get; init; }
    /// <summary>When true, images are generated sequentially with reference chaining for consistent style. When false, images are generated in parallel (faster, less consistent).</summary>
    public bool UseConsistentImageStyle { get; init; } = true;
    public string LanguageCode { get; init; } = string.Empty;
    public string? Title { get; init; }
    public string? TextModel { get; init; }
    public string? ImageModel { get; init; }
    public string? AudioModel { get; init; }
    /// <summary>Image quality: light | medium | heavy. Same 4:5 aspect ratio; affects resolution/cost (e.g. OpenAI size).</summary>
    public string? ImageQuality { get; init; }
    /// <summary>When true, endpoint returns 202 and enqueues job; when false, runs synchronously and returns 200 with storyId.</summary>
    public bool RunInBackground { get; init; }
}
