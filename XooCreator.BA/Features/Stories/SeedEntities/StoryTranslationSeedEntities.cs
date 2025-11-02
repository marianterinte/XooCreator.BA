namespace XooCreator.BA.Features.Stories.SeedEntities;

/// <summary>
/// Seed entity for independent story translations loaded from JSON.
/// </summary>
internal sealed class IndependentStoryTranslationSeed
{
    public string Locale { get; set; } = "en-us";
    public string StoryId { get; set; } = string.Empty;
    public string? Title { get; set; }
    public List<TileTranslationSeed>? Tiles { get; set; }
}

/// <summary>
/// Seed entity for story translations loaded from JSON.
/// </summary>
internal sealed class StoryTranslationSeed
{
    public string Locale { get; set; } = "en-us";
    public string StoryId { get; set; } = string.Empty;
    public string? Title { get; set; }
    public List<TileTranslationSeed>? Tiles { get; set; }
}

/// <summary>
/// Seed entity for tile translations loaded from JSON.
/// </summary>
internal sealed class TileTranslationSeed
{
    public string TileId { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? Question { get; set; }
    public List<AnswerTranslationSeed>? Answers { get; set; }
}

/// <summary>
/// Seed entity for answer translations loaded from JSON.
/// </summary>
internal sealed class AnswerTranslationSeed
{
    public string AnswerId { get; set; } = string.Empty;
    public string? Text { get; set; }
}

