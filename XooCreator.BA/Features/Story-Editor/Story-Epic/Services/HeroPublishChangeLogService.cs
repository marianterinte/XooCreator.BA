using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IHeroPublishChangeLogService
{
    HeroDraftSnapshot CaptureSnapshot(EpicHeroCraft craft, string languageCode);
    Task AppendChangesAsync(EpicHeroCraft craft, HeroDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct);
}

public class HeroPublishChangeLogService : IHeroPublishChangeLogService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly XooDbContext _context;

    public HeroPublishChangeLogService(XooDbContext context)
    {
        _context = context;
    }

    public HeroDraftSnapshot CaptureSnapshot(EpicHeroCraft craft, string languageCode)
        => HeroDraftSnapshot.CreateFromCraft(craft, languageCode);

    public async Task AppendChangesAsync(EpicHeroCraft craft, HeroDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct)
    {
        try
        {
            var freshCraft = await LoadCraftAsync(craft.Id, languageCode, ct);
            if (freshCraft == null)
            {
                return;
            }

            var currentSnapshot = HeroDraftSnapshot.CreateFromCraft(freshCraft, languageCode);
            var diffEntries = HeroDraftSnapshotDiff.Calculate(previousSnapshot, currentSnapshot, SerializerOptions);

            if (diffEntries.Count == 0)
            {
                return;
            }

            var nextVersion = craft.LastDraftVersion + 1;
            foreach (var entry in diffEntries)
            {
                entry.Id = Guid.NewGuid();
                entry.HeroId = craft.Id;
                entry.DraftVersion = nextVersion;
                entry.LanguageCode = languageCode;
                entry.CreatedAt = DateTime.UtcNow;
                entry.CreatedBy = userId;
            }

            craft.LastDraftVersion = nextVersion;
            _context.HeroPublishChangeLogs.AddRange(diffEntries);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task<EpicHeroCraft?> LoadCraftAsync(string heroId, string languageCode, CancellationToken ct)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        return await _context.EpicHeroCrafts
            .AsNoTracking()
            .Include(x => x.Translations.Where(t => t.LanguageCode == lang))
            .FirstOrDefaultAsync(x => x.Id == heroId, ct);
    }

    private sealed class HeroDraftSnapshotDiff
    {
        public static List<HeroPublishChangeLog> Calculate(HeroDraftSnapshot? previousSnapshot, HeroDraftSnapshot currentSnapshot, JsonSerializerOptions serializerOptions)
        {
            var previous = previousSnapshot ?? HeroDraftSnapshot.Empty(currentSnapshot.LanguageCode);
            var changes = new List<HeroPublishChangeLog>();

            // Check header (ImageUrl) changes
            if (!string.Equals(previous.Header.Hash, currentSnapshot.Header.Hash, StringComparison.Ordinal))
            {
                var changeType = previous.Header.HasContent ? "Updated" : "Added";
                changes.Add(new HeroPublishChangeLog
                {
                    EntityType = HeroPublishEntityTypes.Header,
                    ChangeType = changeType,
                    Hash = currentSnapshot.Header.Hash,
                    PayloadJson = JsonSerializer.Serialize(currentSnapshot.Header.ToPayload(), serializerOptions),
                    AssetDraftPath = currentSnapshot.Header.ImageUrl
                });
            }

            // Check translation changes
            foreach (var kvp in currentSnapshot.Translations)
            {
                if (!previous.Translations.TryGetValue(kvp.Key, out var oldTranslation))
                {
                    changes.Add(CreateTranslationChange("Added", kvp.Value, serializerOptions));
                }
                else if (!string.Equals(oldTranslation.Hash, kvp.Value.Hash, StringComparison.Ordinal))
                {
                    changes.Add(CreateTranslationChange("Updated", kvp.Value, serializerOptions));
                }
            }

            foreach (var kvp in previous.Translations)
            {
                if (!currentSnapshot.Translations.ContainsKey(kvp.Key))
                {
                    changes.Add(CreateTranslationChange("Removed", kvp.Value, serializerOptions));
                }
            }

            return changes;
        }

        private static HeroPublishChangeLog CreateTranslationChange(string changeType, HeroDraftSnapshot.TranslationState translation, JsonSerializerOptions serializerOptions)
        {
            return new HeroPublishChangeLog
            {
                EntityType = HeroPublishEntityTypes.Translation,
                EntityId = translation.LanguageCode,
                ChangeType = changeType,
                Hash = translation.Hash,
                PayloadJson = JsonSerializer.Serialize(translation.ToPayload(), serializerOptions),
                AssetDraftPath = translation.GreetingAudioUrl
            };
        }
    }

    private static class HeroPublishEntityTypes
    {
        public const string Header = "Header";
        public const string Translation = "Translation";
    }
}

public sealed class HeroDraftSnapshot
{
    private HeroDraftSnapshot(string languageCode, HeaderState header, Dictionary<string, TranslationState> translations)
    {
        LanguageCode = languageCode;
        Header = header;
        Translations = translations;
    }

    public string LanguageCode { get; }
    public HeaderState Header { get; }
    public Dictionary<string, TranslationState> Translations { get; }

    public static HeroDraftSnapshot CreateFromCraft(EpicHeroCraft craft, string languageCode)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();

        var header = HeaderState.Create(craft.ImageUrl);

        var translations = new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase);
        foreach (var translation in craft.Translations.OrderBy(t => t.LanguageCode))
        {
            var translationState = TranslationState.Create(
                translation.LanguageCode,
                translation.Name,
                translation.Description,
                translation.GreetingText,
                translation.GreetingAudioUrl);

            translations[translationState.LanguageCode] = translationState;
        }

        return new HeroDraftSnapshot(lang, header, translations);
    }

    public static HeroDraftSnapshot Empty(string languageCode)
        => new(languageCode, HeaderState.Empty(), new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase));

    public sealed class HeaderState
    {
        private HeaderState(string? imageUrl)
        {
            ImageUrl = imageUrl;
            Hash = HashHelper.ComputeHash(new { ImageUrl });
        }

        public string? ImageUrl { get; }
        public string Hash { get; }
        public bool HasContent => !string.IsNullOrEmpty(ImageUrl);

        public object ToPayload() => new
        {
            imageUrl = ImageUrl
        };

        public static HeaderState Create(string? imageUrl)
            => new(imageUrl);

        public static HeaderState Empty() => new(null);
    }

    public sealed class TranslationState
    {
        private TranslationState(string languageCode, string name, string? description, string? greetingText, string? greetingAudioUrl)
        {
            LanguageCode = languageCode;
            Name = name;
            Description = description;
            GreetingText = greetingText;
            GreetingAudioUrl = greetingAudioUrl;
            Hash = HashHelper.ComputeHash(new
            {
                LanguageCode,
                Name,
                Description,
                GreetingText,
                GreetingAudioUrl
            });
        }

        public string LanguageCode { get; }
        public string Name { get; }
        public string? Description { get; }
        public string? GreetingText { get; }
        public string? GreetingAudioUrl { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            languageCode = LanguageCode,
            name = Name,
            description = Description,
            greetingText = GreetingText,
            greetingAudioUrl = GreetingAudioUrl
        };

        public static TranslationState Create(string languageCode, string name, string? description, string? greetingText, string? greetingAudioUrl)
            => new(languageCode, name, description, greetingText, greetingAudioUrl);
    }
}

// HashHelper is defined in XooCreator.BA.Features.StoryEditor.Services.HashHelper

