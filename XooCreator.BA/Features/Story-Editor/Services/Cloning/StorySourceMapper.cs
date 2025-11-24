using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Services.Cloning;

/// <summary>
/// Unified mapper for converting different story sources into a common structure for cloning.
/// </summary>
public interface IStorySourceMapper
{
    StoryCloneData MapFromCraft(StoryCraft source, bool isCopy);
    StoryCloneData MapFromDefinition(StoryDefinition definition, bool isCopy);
}

public class StorySourceMapper : IStorySourceMapper
{
    public StoryCloneData MapFromCraft(StoryCraft source, bool isCopy)
    {
        return new StoryCloneData
        {
            StoryType = source.StoryType,
            StoryTopic = source.StoryTopic,
            CoverImageUrl = source.CoverImageUrl,
            PriceInCredits = source.PriceInCredits,
            AuthorName = source.AuthorName,
            ClassicAuthorId = source.ClassicAuthorId,
            BaseVersion = source.BaseVersion,
            Translations = source.Translations.Select(t => new TranslationCloneData
            {
                LanguageCode = t.LanguageCode,
                Title = isCopy && !string.IsNullOrWhiteSpace(t.Title) && !t.Title.StartsWith("Copy of ", StringComparison.OrdinalIgnoreCase)
                    ? $"Copy of {t.Title}"
                    : t.Title,
                Summary = t.Summary
            }).ToList(),
            Tiles = source.Tiles.OrderBy(t => t.SortOrder).Select(tile => new TileCloneData
            {
                TileId = tile.TileId,
                Type = tile.Type,
                ImageUrl = tile.ImageUrl,
                Translations = tile.Translations.Select(tt => new TileTranslationCloneData
                {
                    LanguageCode = tt.LanguageCode,
                    Caption = tt.Caption,
                    Text = tt.Text,
                    Question = tt.Question,
                    AudioUrl = tt.AudioUrl,
                    VideoUrl = tt.VideoUrl
                }).ToList(),
                Answers = tile.Answers.OrderBy(a => a.SortOrder).Select(answer => new AnswerCloneData
                {
                    AnswerId = answer.AnswerId,
                    Translations = answer.Translations.Select(at => new AnswerTranslationCloneData
                    {
                        LanguageCode = at.LanguageCode,
                        Text = at.Text ?? string.Empty
                    }).ToList(),
                    Tokens = answer.Tokens.Select(token => new TokenCloneData
                    {
                        Type = token.Type ?? string.Empty,
                        Value = token.Value ?? string.Empty,
                        Quantity = token.Quantity
                    }).ToList()
                }).ToList()
            }).ToList(),
            Topics = source.Topics.Select(t => t.StoryTopicId).ToList(),
            AgeGroups = source.AgeGroups.Select(ag => ag.StoryAgeGroupId).ToList()
        };
    }

    public StoryCloneData MapFromDefinition(StoryDefinition definition, bool isCopy)
    {
        return new StoryCloneData
        {
            StoryType = definition.StoryType,
            StoryTopic = definition.StoryTopic,
            CoverImageUrl = ExtractFileName(definition.CoverImageUrl),
            PriceInCredits = definition.PriceInCredits,
            AuthorName = definition.AuthorName,
            ClassicAuthorId = definition.ClassicAuthorId,
            BaseVersion = definition.Version,
            Translations = definition.Translations.Select(t => new TranslationCloneData
            {
                LanguageCode = t.LanguageCode,
                Title = isCopy && !string.IsNullOrWhiteSpace(t.Title) && !t.Title.StartsWith("Copy of ", StringComparison.OrdinalIgnoreCase)
                    ? $"Copy of {t.Title}"
                    : t.Title,
                Summary = definition.Summary
            }).ToList(),
            Tiles = definition.Tiles.OrderBy(t => t.SortOrder).Select(tile => new TileCloneData
            {
                TileId = tile.TileId,
                Type = tile.Type,
                ImageUrl = ExtractFileName(tile.ImageUrl),
                Translations = tile.Translations.Select(tt => new TileTranslationCloneData
                {
                    LanguageCode = tt.LanguageCode,
                    Caption = tt.Caption ?? string.Empty,
                    Text = tt.Text ?? string.Empty,
                    Question = tt.Question ?? string.Empty,
                    AudioUrl = ExtractFileName(tt.AudioUrl),
                    VideoUrl = ExtractFileName(tt.VideoUrl)
                }).ToList(),
                Answers = tile.Answers.OrderBy(a => a.SortOrder).Select(answer => new AnswerCloneData
                {
                    AnswerId = answer.AnswerId,
                    Translations = answer.Translations.Select(at => new AnswerTranslationCloneData
                    {
                        LanguageCode = at.LanguageCode,
                        Text = at.Text ?? string.Empty
                    }).ToList(),
                    Tokens = answer.Tokens.Select(token => new TokenCloneData
                    {
                        Type = token.Type ?? string.Empty,
                        Value = token.Value ?? string.Empty,
                        Quantity = token.Quantity
                    }).ToList()
                }).ToList()
            }).ToList(),
            Topics = definition.Topics.Select(t => t.StoryTopicId).ToList(),
            AgeGroups = definition.AgeGroups.Select(ag => ag.StoryAgeGroupId).ToList()
        };
    }

    private static string? ExtractFileName(string? path)
    {
        return string.IsNullOrWhiteSpace(path) ? null : Path.GetFileName(path);
    }
}

// Data structures for unified cloning
public class StoryCloneData
{
    public StoryType StoryType { get; set; }
    public string? StoryTopic { get; set; }
    public string? CoverImageUrl { get; set; }
    public double PriceInCredits { get; set; }
    public string? AuthorName { get; set; }
    public Guid? ClassicAuthorId { get; set; }
    public int? BaseVersion { get; set; }
    public List<TranslationCloneData> Translations { get; set; } = new();
    public List<TileCloneData> Tiles { get; set; } = new();
    public List<Guid> Topics { get; set; } = new();
    public List<Guid> AgeGroups { get; set; } = new();
}

public class TranslationCloneData
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }
}

public class TileCloneData
{
    public string TileId { get; set; } = string.Empty;
    public string Type { get; set; } = "page";
    public string? ImageUrl { get; set; }
    public List<TileTranslationCloneData> Translations { get; set; } = new();
    public List<AnswerCloneData> Answers { get; set; } = new();
}

public class TileTranslationCloneData
{
    public string LanguageCode { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? Question { get; set; }
    public string? AudioUrl { get; set; }
    public string? VideoUrl { get; set; }
}

public class AnswerCloneData
{
    public string AnswerId { get; set; } = string.Empty;
    public List<AnswerTranslationCloneData> Translations { get; set; } = new();
    public List<TokenCloneData> Tokens { get; set; } = new();
}

public class AnswerTranslationCloneData
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class TokenCloneData
{
    public string Type { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
