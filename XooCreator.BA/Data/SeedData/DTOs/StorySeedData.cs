namespace XooCreator.BA.Data.SeedData.DTOs;

public class StoryDefinitionSeedData
{
    public string StoryId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string? StoryTopic { get; set; } // Story topic/theme (e.g., "Matematică", "Literatură")
    public string? Summary { get; set; } // Story summary/description
    public int SortOrder { get; set; }
    public bool IsEvaluative { get; set; } = false; // If true, this story contains quizzes that should be evaluated

    public double PriceInCredits { get; set; } = 0; // Price in credits for purchasing the story

    public List<string> UnlockedStoryHeroes { get; set; } = new();
    public List<StoryTileSeedData> Tiles { get; set; } = new();
}

public class StoryTileSeedData
{
    public string TileId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string Caption { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string? ImageUrl { get; set; }

    public string? Question { get; set; }
    public List<StoryAnswerSeedData> Answers { get; set; } = new();
}

public class StoryAnswerSeedData
{
    public string AnswerId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false; // True if this is the correct answer (only present in JSON for correct answers)
    public string? NextTileId { get; set; }
    public List<StoryAnswerTokenSeedData> Tokens { get; set; } = new();
}

public class StoryAnswerTokenSeedData
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
