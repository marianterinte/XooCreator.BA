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
            .Include(x => x.Topics)
                .ThenInclude(t => t.StoryTopic)
            .Include(x => x.AgeGroups)
                .ThenInclude(ag => ag.StoryAgeGroup)
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

        var header = HeaderState.Create(
            translation?.Title ?? string.Empty,
            translation?.Summary,
            craft.CoverImageUrl,
            craft.StoryTopic,
            craft.AuthorName,
            craft.ClassicAuthorId,
            craft.PriceInCredits,
            craft.StoryType,
            craft.Topics.Select(t => t.StoryTopic.TopicId).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList(),
            craft.AgeGroups.Select(ag => ag.StoryAgeGroup.AgeGroupId).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(x => x, StringComparer.OrdinalIgnoreCase).ToList());

        var tiles = new Dictionary<string, TileState>(StringComparer.OrdinalIgnoreCase);
        foreach (var tile in craft.Tiles.OrderBy(t => t.SortOrder))
        {
            var tileTranslation = tile.Translations.FirstOrDefault(tr => tr.LanguageCode == lang);
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

    public static StoryDraftSnapshot Empty(string languageCode)
        => new(languageCode, HeaderState.Empty(), new Dictionary<string, TileState>(StringComparer.OrdinalIgnoreCase));

    public sealed class HeaderState
    {
        private HeaderState(string title, string? summary, string? coverImage, string? storyTopic, string? authorName, Guid? classicAuthorId, double priceInCredits, StoryType storyType, IReadOnlyCollection<string> topicIds, IReadOnlyCollection<string> ageGroupIds)
        {
            Title = title;
            Summary = summary;
            CoverImage = coverImage;
            StoryTopic = storyTopic;
            AuthorName = authorName;
            ClassicAuthorId = classicAuthorId;
            PriceInCredits = priceInCredits;
            StoryType = storyType;
            TopicIds = topicIds;
            AgeGroupIds = ageGroupIds;
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
                TopicIds,
                AgeGroupIds
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
        public IReadOnlyCollection<string> TopicIds { get; }
        public IReadOnlyCollection<string> AgeGroupIds { get; }
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
            topicIds = TopicIds,
            ageGroupIds = AgeGroupIds
        };

        public static HeaderState Create(string title, string? summary, string? coverImage, string? storyTopic, string? authorName, Guid? classicAuthorId, double priceInCredits, StoryType storyType, IReadOnlyCollection<string> topicIds, IReadOnlyCollection<string> ageGroupIds)
            => new(title, summary, coverImage, storyTopic, authorName, classicAuthorId, priceInCredits, storyType, topicIds, ageGroupIds);

        public static HeaderState Empty() => new(string.Empty, null, null, null, null, null, 0, StoryType.Indie, Array.Empty<string>(), Array.Empty<string>());
    }

    public sealed class TileState
    {
        private TileState(string tileId, int sortOrder, string type, string? image, string? caption, string? text, string? question, string? audio, string? video, IReadOnlyCollection<AnswerState> answers)
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
            answers = Answers.Select(a => a.ToPayload())
        };

        public static TileState Create(string tileId, int sortOrder, string type, string? image, string? caption, string? text, string? question, string? audio, string? video, IReadOnlyCollection<AnswerState> answers)
            => new(tileId, sortOrder, type, image, caption, text, question, audio, video, answers);
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

internal static class HashHelper
{
    public static string ComputeHash(object data)
    {
        var json = JsonSerializer.Serialize(data);
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hashBytes);
    }
}

