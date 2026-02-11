using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data.SeedData;

namespace XooCreator.BA.Features.StoryEditor.Services.Cloning;

/// <summary>
/// Unified mapper for converting different story sources into a common structure for cloning.
/// </summary>
public interface IStorySourceMapper
{
    StoryCloneData MapFromCraft(StoryCraft source, bool isCopy, bool isFork = false);
    StoryCloneData MapFromDefinition(StoryDefinition definition, bool isCopy, bool isFork = false);
}

public class StorySourceMapper : IStorySourceMapper
{
    public StoryCloneData MapFromCraft(StoryCraft source, bool isCopy, bool isFork = false)
    {
        return new StoryCloneData
        {
            StoryType = source.StoryType,
            StoryTopic = source.StoryTopic,
            CoverImageUrl = source.CoverImageUrl,
            PriceInCredits = source.PriceInCredits,
            AuthorName = isFork ? null : source.AuthorName,
            ClassicAuthorId = source.ClassicAuthorId,
            BaseVersion = source.BaseVersion,
            IsEvaluative = source.IsEvaluative,
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
                BranchId = tile.BranchId,
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
                    IsCorrect = answer.IsCorrect,
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
                }).ToList(),
                DialogRootNodeId = tile.DialogTile?.RootNodeId,
                DialogNodes = tile.DialogTile?.Nodes
                    .OrderBy(n => n.SortOrder)
                    .Select(n => new DialogNodeCloneData
                    {
                        NodeId = n.NodeId,
                        SpeakerType = n.SpeakerType,
                        SpeakerHeroId = n.SpeakerHeroId,
                        Translations = n.Translations.Select(t => new DialogNodeTranslationCloneData
                        {
                            LanguageCode = t.LanguageCode,
                            Text = t.Text
                        }).ToList(),
                        Options = n.OutgoingEdges
                            .OrderBy(e => e.OptionOrder)
                            .Select(e => new DialogEdgeCloneData
                            {
                                EdgeId = e.EdgeId,
                                ToNodeId = e.ToNodeId,
                                JumpToTileId = e.JumpToTileId,
                                SetBranchId = e.SetBranchId,
                                OptionOrder = e.OptionOrder,
                                Translations = e.Translations.Select(et => new DialogEdgeTranslationCloneData
                                {
                                    LanguageCode = et.LanguageCode,
                                    OptionText = et.OptionText
                                }).ToList(),
                                Tokens = e.Tokens.Select(tok => new TokenCloneData
                                {
                                    Type = tok.Type,
                                    Value = tok.Value,
                                    Quantity = tok.Quantity
                                }).ToList()
                            }).ToList()
                    }).ToList() ?? new List<DialogNodeCloneData>()
            }).ToList(),
            Topics = source.Topics.Select(t => t.StoryTopicId).ToList(),
            AgeGroups = source.AgeGroups.Select(ag => ag.StoryAgeGroupId).ToList(),
            UnlockedStoryHeroes = GetUnlockedHeroesFromCraft(source),
            DialogParticipants = source.DialogParticipants.OrderBy(p => p.SortOrder).Select(p => p.HeroId).ToList()
        };
    }

    public StoryCloneData MapFromDefinition(StoryDefinition definition, bool isCopy, bool isFork = false)
    {
        var isSeeded = LooksLikeSeededStory(definition);

        return new StoryCloneData
        {
            StoryType = definition.StoryType,
            StoryTopic = definition.StoryTopic,
            CoverImageUrl = ExtractFileName(definition.CoverImageUrl),
            PriceInCredits = definition.PriceInCredits,
            AuthorName = isFork ? null : definition.AuthorName,
            ClassicAuthorId = definition.ClassicAuthorId,
            BaseVersion = definition.Version,
            IsEvaluative = definition.IsEvaluative,
            Translations = definition.Translations.Select(t => new TranslationCloneData
            {
                LanguageCode = t.LanguageCode,
                Title = isCopy && !string.IsNullOrWhiteSpace(t.Title) && !t.Title.StartsWith("Copy of ", StringComparison.OrdinalIgnoreCase)
                    ? $"Copy of {t.Title}"
                    : t.Title,
                Summary = definition.Summary
            }).ToList(),
            Tiles = definition.Tiles.OrderBy(t => t.SortOrder).Select(tile =>
            {
                var imageFilename = ExtractFileName(tile.ImageUrl);
                var derivedAudioFilename = isSeeded ? DeriveSeededAudioFilename(imageFilename) : null;

                return new TileCloneData
                {
                    TileId = tile.TileId,
                    Type = tile.Type,
                    BranchId = tile.BranchId,
                    ImageUrl = imageFilename,
                    Translations = tile.Translations.Select(tt =>
                    {
                        var audioFilename = ExtractFileName(tt.AudioUrl);
                        if (string.IsNullOrWhiteSpace(audioFilename))
                        {
                            audioFilename = derivedAudioFilename;
                        }

                        return new TileTranslationCloneData
                        {
                            LanguageCode = tt.LanguageCode,
                            Caption = tt.Caption ?? string.Empty,
                            Text = tt.Text ?? string.Empty,
                            Question = tt.Question ?? string.Empty,
                            AudioUrl = audioFilename,
                            VideoUrl = ExtractFileName(tt.VideoUrl)
                        };
                    }).ToList(),
                Answers = tile.Answers.OrderBy(a => a.SortOrder).Select(answer => new AnswerCloneData
                {
                    AnswerId = answer.AnswerId,
                    IsCorrect = answer.IsCorrect,
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
                }).ToList(),
                DialogRootNodeId = tile.DialogTile?.RootNodeId,
                DialogNodes = tile.DialogTile?.Nodes
                    .OrderBy(n => n.SortOrder)
                    .Select(n => new DialogNodeCloneData
                    {
                        NodeId = n.NodeId,
                        SpeakerType = n.SpeakerType,
                        SpeakerHeroId = n.SpeakerHeroId,
                        Translations = n.Translations.Select(t => new DialogNodeTranslationCloneData
                        {
                            LanguageCode = t.LanguageCode,
                            Text = t.Text
                        }).ToList(),
                        Options = n.OutgoingEdges
                            .OrderBy(e => e.OptionOrder)
                            .Select(e => new DialogEdgeCloneData
                            {
                                EdgeId = e.EdgeId,
                                ToNodeId = e.ToNodeId,
                                JumpToTileId = e.JumpToTileId,
                                SetBranchId = e.SetBranchId,
                                OptionOrder = e.OptionOrder,
                                Translations = e.Translations.Select(et => new DialogEdgeTranslationCloneData
                                {
                                    LanguageCode = et.LanguageCode,
                                    OptionText = et.OptionText
                                }).ToList(),
                                Tokens = e.Tokens.Select(tok => new TokenCloneData
                                {
                                    Type = tok.Type,
                                    Value = tok.Value,
                                    Quantity = tok.Quantity
                                }).ToList()
                            }).ToList()
                    }).ToList() ?? new List<DialogNodeCloneData>()
            };
            }).ToList(),
            Topics = definition.Topics.Select(t => t.StoryTopicId).ToList(),
            AgeGroups = definition.AgeGroups.Select(ag => ag.StoryAgeGroupId).ToList(),
            UnlockedStoryHeroes = definition.UnlockedHeroes.Select(h => h.HeroId).ToList(),
            DialogParticipants = definition.DialogParticipants.OrderBy(p => p.SortOrder).Select(p => p.HeroId).ToList()
        };
    }

    private static string? ExtractFileName(string? path)
    {
        return string.IsNullOrWhiteSpace(path) ? null : Path.GetFileName(path);
    }

    private static string? DeriveSeededAudioFilename(string? imageFilename)
    {
        if (string.IsNullOrWhiteSpace(imageFilename)) return null;
        var baseName = Path.GetFileNameWithoutExtension(imageFilename);
        if (string.IsNullOrWhiteSpace(baseName)) return null;
        return $"{baseName}.wav";
    }

    private static bool LooksLikeSeededStory(StoryDefinition definition)
    {
        if (definition == null) return false;
        if (LooksLikeSeededPath(definition.CoverImageUrl)) return true;
        return definition.Tiles.Any(t => LooksLikeSeededPath(t.ImageUrl));
    }

    private static bool LooksLikeSeededPath(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return false;
        return path.Contains(SeedUserHelper.SeedUserEmail, StringComparison.OrdinalIgnoreCase)
               || path.Contains("/tol/stories/", StringComparison.OrdinalIgnoreCase);
    }

    private static List<string> GetUnlockedHeroesFromCraft(StoryCraft craft)
    {
        return craft.UnlockedHeroes.Select(h => h.HeroId).ToList();
    }

    // JSON-based unlocked heroes removed (DB is source of truth).
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
    public bool IsEvaluative { get; set; } = false; // If true, this story contains quizzes that should be evaluated
    public List<TranslationCloneData> Translations { get; set; } = new();
    public List<TileCloneData> Tiles { get; set; } = new();
    public List<Guid> Topics { get; set; } = new();
    public List<Guid> AgeGroups { get; set; } = new();
    public List<string> UnlockedStoryHeroes { get; set; } = new();
    public List<string> DialogParticipants { get; set; } = new();
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
    public string? BranchId { get; set; }
    public string? ImageUrl { get; set; }
    public List<TileTranslationCloneData> Translations { get; set; } = new();
    public List<AnswerCloneData> Answers { get; set; } = new();
    public string? DialogRootNodeId { get; set; }
    public List<DialogNodeCloneData> DialogNodes { get; set; } = new();
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
    public bool IsCorrect { get; set; } = false; // True if this is the correct answer for the quiz
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

public class DialogNodeCloneData
{
    public string NodeId { get; set; } = string.Empty;
    public string SpeakerType { get; set; } = "reader";
    public string? SpeakerHeroId { get; set; }
    public List<DialogNodeTranslationCloneData> Translations { get; set; } = new();
    public List<DialogEdgeCloneData> Options { get; set; } = new();
}

public class DialogNodeTranslationCloneData
{
    public string LanguageCode { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}

public class DialogEdgeCloneData
{
    public string EdgeId { get; set; } = string.Empty;
    public string ToNodeId { get; set; } = string.Empty;
    public string? JumpToTileId { get; set; }
    public string? SetBranchId { get; set; }
    public int OptionOrder { get; set; }
    public List<DialogEdgeTranslationCloneData> Translations { get; set; } = new();
    public List<TokenCloneData> Tokens { get; set; } = new();
}

public class DialogEdgeTranslationCloneData
{
    public string LanguageCode { get; set; } = string.Empty;
    public string OptionText { get; set; } = string.Empty;
}
