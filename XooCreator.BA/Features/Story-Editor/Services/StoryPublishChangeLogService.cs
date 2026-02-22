using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryPublishChangeLogService
{
    StoryDraftSnapshot CaptureSnapshot(StoryCraft craft, string languageCode);
    Task AppendChangesAsync(StoryCraft craft, StoryDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct);
}

public class StoryPublishChangeLogService : IStoryPublishChangeLogService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly XooDbContext _context;

    public StoryPublishChangeLogService(XooDbContext context)
    {
        _context = context;
    }

    public StoryDraftSnapshot CaptureSnapshot(StoryCraft craft, string languageCode)
        => StoryDraftSnapshot.CreateFromCraft(craft, languageCode);

    public async Task AppendChangesAsync(StoryCraft craft, StoryDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct)
    {
        try
        {
            var freshCraft = await LoadCraftAsync(craft.Id, languageCode, ct);
            if (freshCraft == null)
            {
                return;
            }

            var currentSnapshot = StoryDraftSnapshot.CreateFromCraft(freshCraft, languageCode);
            var diffEntries = StoryDraftSnapshotDiff.Calculate(previousSnapshot, currentSnapshot, SerializerOptions);

            if (diffEntries.Count == 0)
            {
                return;
            }

            var nextVersion = craft.LastDraftVersion + 1;
            foreach (var entry in diffEntries)
            {
                entry.Id = Guid.NewGuid();
                entry.StoryId = craft.StoryId;
                entry.DraftVersion = nextVersion;
                entry.LanguageCode = languageCode;
                entry.CreatedAt = DateTime.UtcNow;
                entry.CreatedBy = userId;
            }

            craft.LastDraftVersion = nextVersion;
            _context.StoryPublishChangeLogs.AddRange(diffEntries);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {

            throw;
        }
    }

    private async Task<StoryCraft?> LoadCraftAsync(Guid craftId, string languageCode, CancellationToken ct)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        return await _context.StoryCrafts
            .AsNoTracking()
            .Include(x => x.Translations.Where(t => t.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Translations.Where(tr => tr.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations.Where(at => at.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Tokens)
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.Translations.Where(nt => nt.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Translations.Where(et => et.LanguageCode == lang))
            .Include(x => x.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Tokens)
            .Include(x => x.Topics)
                .ThenInclude(t => t.StoryTopic)
            .Include(x => x.AgeGroups)
                .ThenInclude(ag => ag.StoryAgeGroup)
            .Include(x => x.CoAuthors)
            .FirstOrDefaultAsync(x => x.Id == craftId, ct);
    }

    private sealed class StoryDraftSnapshotDiff
    {
        public static List<StoryPublishChangeLog> Calculate(StoryDraftSnapshot? previousSnapshot, StoryDraftSnapshot currentSnapshot, JsonSerializerOptions serializerOptions)
        {
            var previous = previousSnapshot ?? StoryDraftSnapshot.Empty(currentSnapshot.LanguageCode);
            var changes = new List<StoryPublishChangeLog>();

            if (!string.Equals(previous.Header.Hash, currentSnapshot.Header.Hash, StringComparison.Ordinal))
            {
                var changeType = previous.Header.HasContent ? "Updated" : "Added";
                changes.Add(new StoryPublishChangeLog
                {
                    EntityType = StoryPublishEntityTypes.Header,
                    ChangeType = changeType,
                    Hash = currentSnapshot.Header.Hash,
                    PayloadJson = JsonSerializer.Serialize(currentSnapshot.Header.ToPayload(), serializerOptions),
                    AssetDraftPath = currentSnapshot.Header.CoverImage
                });
            }

            foreach (var kvp in currentSnapshot.Tiles)
            {
                if (!previous.Tiles.TryGetValue(kvp.Key, out var oldTile))
                {
                    changes.Add(CreateTileChange("Added", kvp.Value, serializerOptions));
                }
                else if (!string.Equals(oldTile.Hash, kvp.Value.Hash, StringComparison.Ordinal))
                {
                    changes.Add(CreateTileChange("Updated", kvp.Value, serializerOptions));
                }
            }

            foreach (var kvp in previous.Tiles)
            {
                if (!currentSnapshot.Tiles.ContainsKey(kvp.Key))
                {
                    changes.Add(CreateTileChange("Removed", kvp.Value, serializerOptions));
                }
            }

            return changes;
        }

        private static StoryPublishChangeLog CreateTileChange(string changeType, StoryDraftSnapshot.TileState tile, JsonSerializerOptions serializerOptions)
        {
            return new StoryPublishChangeLog
            {
                EntityType = StoryPublishEntityTypes.Tile,
                EntityId = tile.TileId,
                ChangeType = changeType,
                Hash = tile.Hash,
                PayloadJson = JsonSerializer.Serialize(tile.ToPayload(), serializerOptions),
                AssetDraftPath = tile.Image
            };
        }
    }

    private static class StoryPublishEntityTypes
    {
        public const string Header = "Header";
        public const string Tile = "Tile";
    }
}

public sealed class StoryDraftSnapshot
{
    private StoryDraftSnapshot(string languageCode, HeaderState header, Dictionary<string, TileState> tiles)
    {
        LanguageCode = languageCode;
        Header = header;
        Tiles = tiles;
    }

    public string LanguageCode { get; }
    public HeaderState Header { get; }
    public Dictionary<string, TileState> Tiles { get; }

    public static StoryDraftSnapshot CreateFromCraft(StoryCraft craft, string languageCode)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode == lang);

        var coAuthorsSnapshot = (craft.CoAuthors ?? Enumerable.Empty<StoryCraftCoAuthor>())
            .OrderBy(c => c.SortOrder)
            .Select(c => new HeaderState.CoAuthorSnapshot(c.UserId, c.DisplayName ?? string.Empty))
            .ToList();
        var header = HeaderState.Create(
            translation?.Title ?? string.Empty,
            translation?.Summary,
            craft.CoverImageUrl,
            craft.StoryTopic,
            craft.AuthorName,
            craft.ClassicAuthorId,
            craft.PriceInCredits,
            craft.StoryType,
            craft.IsEvaluative,
            craft.IsPartOfEpic,
            craft.IsFullyInteractive,
            craft.Topics.Select(t => t.StoryTopic.TopicId).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList(),
            craft.AgeGroups.Select(ag => ag.StoryAgeGroup.AgeGroupId).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList(),
            coAuthorsSnapshot);

        var tiles = new Dictionary<string, TileState>(StringComparer.OrdinalIgnoreCase);
        foreach (var tile in craft.Tiles.OrderBy(t => t.SortOrder))
        {
            var tileTranslation = tile.Translations.FirstOrDefault(tr => tr.LanguageCode == lang);
            var dialogHash = ComputeDialogHash(tile, lang);
            var tileState = TileState.Create(
                tile.TileId,
                tile.SortOrder,
                tile.Type ?? "page",
                tile.ImageUrl,
                tileTranslation?.Caption,
                tileTranslation?.Text,
                tileTranslation?.Question,
                tileTranslation?.AudioUrl,
                tileTranslation?.VideoUrl,
                dialogHash,
                tile.Answers.OrderBy(a => a.SortOrder).Select(a =>
                {
                    var answerTranslation = a.Translations.FirstOrDefault(at => at.LanguageCode == lang);
                    return AnswerState.Create(
                        a.AnswerId,
                        a.SortOrder,
                        answerTranslation?.Text ?? string.Empty,
                        a.Tokens.OrderBy(t => t.Type).ThenBy(t => t.Value).Select(t => TokenState.Create(t.Type, t.Value, t.Quantity)).ToList());
                }).ToList());

            tiles[tileState.TileId] = tileState;
        }

        return new StoryDraftSnapshot(lang, header, tiles);
    }

    private static string? ComputeDialogHash(StoryCraftTile tile, string languageCode)
    {
        if (!string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase) || tile.DialogTile == null)
        {
            return null;
        }

        var payload = new
        {
            rootNodeId = tile.DialogTile.RootNodeId ?? string.Empty,
            nodes = tile.DialogTile.Nodes
                .OrderBy(n => n.SortOrder)
                .Select(n =>
                {
                    var nodeTr = n.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
                                 ?? n.Translations.FirstOrDefault();
                    return new
                    {
                        nodeId = n.NodeId ?? string.Empty,
                        speakerType = n.SpeakerType ?? string.Empty,
                        speakerHeroId = n.SpeakerHeroId ?? string.Empty,
                        sortOrder = n.SortOrder,
                        text = nodeTr?.Text ?? string.Empty,
                        audio = nodeTr?.AudioUrl ?? string.Empty,
                        options = n.OutgoingEdges
                            .OrderBy(e => e.OptionOrder)
                            .Select(e =>
                            {
                                var edgeTr = e.Translations.FirstOrDefault(t => t.LanguageCode == languageCode)
                                             ?? e.Translations.FirstOrDefault();
                                return new
                                {
                                    edgeId = e.EdgeId ?? string.Empty,
                                    toNodeId = e.ToNodeId ?? string.Empty,
                                    jumpToTileId = e.JumpToTileId ?? string.Empty,
                                    setBranchId = e.SetBranchId ?? string.Empty,
                                    hideIfBranchSet = e.HideIfBranchSet ?? string.Empty,
                                    showOnlyIfBranchesSet = e.ShowOnlyIfBranchesSet ?? string.Empty,
                                    optionOrder = e.OptionOrder,
                                    optionText = edgeTr?.OptionText ?? string.Empty,
                                    tokens = e.Tokens
                                        .OrderBy(t => t.Type)
                                        .ThenBy(t => t.Value)
                                        .Select(t => new
                                        {
                                            type = t.Type ?? string.Empty,
                                            value = t.Value ?? string.Empty,
                                            quantity = t.Quantity
                                        })
                                };
                            })
                    };
                })
        };

        return HashHelper.ComputeHash(payload);
    }

    public static StoryDraftSnapshot Empty(string languageCode)
        => new(languageCode, HeaderState.Empty(), new Dictionary<string, TileState>(StringComparer.OrdinalIgnoreCase));

    public sealed class HeaderState
    {
        private HeaderState(
            string title,
            string? summary,
            string? coverImage,
            string? storyTopic,
            string? authorName,
            Guid? classicAuthorId,
            double priceInCredits,
            StoryType storyType,
            bool isEvaluative,
            bool isPartOfEpic,
            bool isFullyInteractive,
            IReadOnlyCollection<string> topicIds,
            IReadOnlyCollection<string> ageGroupIds,
            IReadOnlyCollection<CoAuthorSnapshot> coAuthors)
        {
            Title = title;
            Summary = summary;
            CoverImage = coverImage;
            StoryTopic = storyTopic;
            AuthorName = authorName;
            ClassicAuthorId = classicAuthorId;
            PriceInCredits = priceInCredits;
            StoryType = storyType;
            IsEvaluative = isEvaluative;
            IsPartOfEpic = isPartOfEpic;
            IsFullyInteractive = isFullyInteractive;
            TopicIds = topicIds;
            AgeGroupIds = ageGroupIds;
            CoAuthors = coAuthors;
            Hash = HashHelper.ComputeHash(new
            {
                Title,
                Summary,
                CoverImage,
                StoryTopic,
                AuthorName,
                ClassicAuthorId,
                PriceInCredits,
                StoryType,
                IsEvaluative,
                IsPartOfEpic,
                IsFullyInteractive,
                TopicIds,
                AgeGroupIds,
                CoAuthors = CoAuthors.Select(c => new { c.UserId, c.DisplayName }).ToList()
            });
        }

        public string Title { get; }
        public string? Summary { get; }
        public string? CoverImage { get; }
        public string? StoryTopic { get; }
        public string? AuthorName { get; }
        public Guid? ClassicAuthorId { get; }
        public double PriceInCredits { get; }
        public StoryType StoryType { get; }
        public bool IsEvaluative { get; }
        public bool IsPartOfEpic { get; }
        public bool IsFullyInteractive { get; }
        public IReadOnlyCollection<string> TopicIds { get; }
        public IReadOnlyCollection<string> AgeGroupIds { get; }
        public IReadOnlyCollection<CoAuthorSnapshot> CoAuthors { get; }
        public string Hash { get; }
        public bool HasContent => !string.IsNullOrEmpty(Title) || !string.IsNullOrEmpty(Summary) || !string.IsNullOrEmpty(CoverImage);

        public object ToPayload() => new
        {
            title = Title,
            summary = Summary,
            coverImage = CoverImage,
            storyTopic = StoryTopic,
            authorName = AuthorName,
            classicAuthorId = ClassicAuthorId,
            priceInCredits = PriceInCredits,
            storyType = StoryType,
            isEvaluative = IsEvaluative,
            isPartOfEpic = IsPartOfEpic,
            isFullyInteractive = IsFullyInteractive,
            topicIds = TopicIds,
            ageGroupIds = AgeGroupIds,
            coAuthors = CoAuthors.Select(c => new { userId = c.UserId, displayName = c.DisplayName }).ToList()
        };

        public static HeaderState Create(
            string title,
            string? summary,
            string? coverImage,
            string? storyTopic,
            string? authorName,
            Guid? classicAuthorId,
            double priceInCredits,
            StoryType storyType,
            bool isEvaluative,
            bool isPartOfEpic,
            bool isFullyInteractive,
            IReadOnlyCollection<string> topicIds,
            IReadOnlyCollection<string> ageGroupIds,
            IReadOnlyCollection<CoAuthorSnapshot>? coAuthors = null)
            => new(title, summary, coverImage, storyTopic, authorName, classicAuthorId, priceInCredits, storyType, isEvaluative, isPartOfEpic, isFullyInteractive, topicIds, ageGroupIds, coAuthors ?? Array.Empty<CoAuthorSnapshot>());

        public static HeaderState Empty()
            => new(string.Empty, null, null, null, null, null, 0, StoryType.Indie, false, false, false, Array.Empty<string>(), Array.Empty<string>(), Array.Empty<CoAuthorSnapshot>());

        public sealed record CoAuthorSnapshot(Guid? UserId, string DisplayName);
    }


    public sealed class TileState
    {
        private TileState(string tileId, int sortOrder, string type, string? image, string? caption, string? text, string? question, string? audio, string? video, string? dialogHash, IReadOnlyCollection<AnswerState> answers)
        {
            TileId = tileId;
            SortOrder = sortOrder;
            Type = type;
            Image = image;
            Caption = caption;
            Text = text;
            Question = question;
            Audio = audio;
            Video = video;
            DialogHash = dialogHash;
            Answers = answers;
            Hash = HashHelper.ComputeHash(new
            {
                TileId,
                SortOrder,
                Type,
                Image,
                Caption,
                Text,
                Question,
                Audio,
                Video,
                DialogHash,
                Answers = answers.Select(a => a.Hash)
            });
        }

        public string TileId { get; }
        public int SortOrder { get; }
        public string Type { get; }
        public string? Image { get; }
        public string? Caption { get; }
        public string? Text { get; }
        public string? Question { get; }
        public string? Audio { get; }
        public string? Video { get; }
        public string? DialogHash { get; }
        public IReadOnlyCollection<AnswerState> Answers { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            tileId = TileId,
            sortOrder = SortOrder,
            type = Type,
            imageUrl = Image,
            caption = Caption,
            text = Text,
            question = Question,
            audioUrl = Audio,
            videoUrl = Video,
            dialogHash = DialogHash,
            answers = Answers.Select(a => a.ToPayload())
        };

        public static TileState Create(string tileId, int sortOrder, string type, string? image, string? caption, string? text, string? question, string? audio, string? video, string? dialogHash, IReadOnlyCollection<AnswerState> answers)
            => new(tileId, sortOrder, type, image, caption, text, question, audio, video, dialogHash, answers);
    }

    public sealed class AnswerState
    {
        private AnswerState(string answerId, int sortOrder, string text, IReadOnlyCollection<TokenState> tokens)
        {
            AnswerId = answerId;
            SortOrder = sortOrder;
            Text = text;
            Tokens = tokens;
            Hash = HashHelper.ComputeHash(new
            {
                AnswerId,
                SortOrder,
                Text,
                Tokens = tokens.Select(t => t.Hash)
            });
        }

        public string AnswerId { get; }
        public int SortOrder { get; }
        public string Text { get; }
        public IReadOnlyCollection<TokenState> Tokens { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            answerId = AnswerId,
            sortOrder = SortOrder,
            text = Text,
            tokens = Tokens.Select(t => t.ToPayload())
        };

        public static AnswerState Create(string answerId, int sortOrder, string text, IReadOnlyCollection<TokenState> tokens)
            => new(answerId, sortOrder, text, tokens);
    }

    public sealed class TokenState
    {
        private TokenState(string? type, string? value, int quantity)
        {
            Type = type ?? string.Empty;
            Value = value ?? string.Empty;
            Quantity = quantity;
            Hash = HashHelper.ComputeHash(new { Type, Value, Quantity });
        }

        public string Type { get; }
        public string Value { get; }
        public int Quantity { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            type = Type,
            value = Value,
            quantity = Quantity
        };

        public static TokenState Create(string? type, string? value, int quantity)
            => new(type, value, quantity);
    }
}

// HashHelper moved to HashHelper.cs

