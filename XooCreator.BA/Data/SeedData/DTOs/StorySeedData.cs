namespace XooCreator.BA.Data.SeedData.DTOs;

public class StoryDefinitionSeedData
{
    public string StoryId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public string? Summary { get; set; } // Story summary/description
    public int SortOrder { get; set; }
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
    public string? NextTileId { get; set; }
    public List<StoryAnswerTokenSeedData> Tokens { get; set; } = new();
}

public class StoryAnswerTokenSeedData
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
}
