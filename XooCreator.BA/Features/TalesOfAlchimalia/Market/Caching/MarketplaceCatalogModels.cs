using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;

public readonly record struct StoryMarketplaceStats(int ReadersCount, double AverageRating, int TotalReviews);

public sealed record StoryMarketplaceBaseItem
{
    public required Guid DefinitionId { get; init; }
    public required string StoryId { get; init; }
    public required string Title { get; init; }
    public string? DefaultTitle { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? CreatedByName { get; init; }
    public required string Summary { get; init; }
    public double PriceInCredits { get; init; }
    public int SortOrder { get; init; }
    public DateTime CreatedAt { get; init; }
    public string? StoryTopic { get; init; }
    public required StoryType StoryType { get; init; }
    public required StoryStatus Status { get; init; }
    public bool IsEvaluative { get; init; }
    public List<string> AvailableLanguages { get; init; } = new();
    public List<string> AudioLanguages { get; init; } = new();
    public List<string> TopicIds { get; init; } = new();
    public List<string> AgeGroupIds { get; init; } = new();
    public List<string> SearchTitles { get; init; } = new();
    public List<string> SearchAuthors { get; init; } = new();
    public string AgeRating { get; init; } = string.Empty;
    public List<string> Characters { get; init; } = new();
}

public readonly record struct EpicMarketplaceStats(int ReadersCount, double AverageRating, int TotalReviews);

public sealed record EpicMarketplaceBaseItem
{
    public required string EpicId { get; init; }
    public required string Name { get; init; }
    public string? Description { get; init; }
    public string? CoverImageUrl { get; init; }
    public Guid? CreatedBy { get; init; }
    public string? CreatedByName { get; init; }
    public DateTime CreatedAt { get; init; }
    public DateTime? PublishedAtUtc { get; init; }
    public int StoryCount { get; init; }
    public int RegionCount { get; init; }
    public List<string> AvailableLanguages { get; init; } = new();
    public List<string> SearchTexts { get; init; } = new();
    public List<string> SearchAuthors { get; init; } = new();
    public List<string> TopicIds { get; init; } = new();
    public List<string> AgeGroupIds { get; init; } = new();
}


