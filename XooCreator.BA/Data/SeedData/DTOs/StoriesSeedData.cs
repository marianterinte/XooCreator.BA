namespace XooCreator.BA.Data.SeedData.DTOs;

// JSON deserialization models for Stories
public class StoriesSeedData
{
    public List<StorySeedData> Stories { get; set; } = new();
}

public class StorySeedData
{
    public string StoryId { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string Category { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public List<TileSeedData>? Tiles { get; set; }
}

public class TileSeedData
{
    public string TileId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public int SortOrder { get; set; }
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? Question { get; set; }
    public List<AnswerSeedData>? Answers { get; set; }
}

public class AnswerSeedData
{
    public string AnswerId { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public List<TokenSeedData>? Tokens { get; set; }
    public int SortOrder { get; set; }
}

public class TokenSeedData
{
    public string Type { get; set; } = string.Empty; // e.g. "TreeOfHeroes" | "Personality" | "Alchemy"
    public string Value { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
