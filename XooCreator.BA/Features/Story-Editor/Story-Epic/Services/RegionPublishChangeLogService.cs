using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IRegionPublishChangeLogService
{
    RegionDraftSnapshot CaptureSnapshot(StoryRegionCraft craft, string languageCode);
    Task AppendChangesAsync(StoryRegionCraft craft, RegionDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct);
}

public class RegionPublishChangeLogService : IRegionPublishChangeLogService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly XooDbContext _context;

    public RegionPublishChangeLogService(XooDbContext context)
    {
        _context = context;
    }

    public RegionDraftSnapshot CaptureSnapshot(StoryRegionCraft craft, string languageCode)
        => RegionDraftSnapshot.CreateFromCraft(craft, languageCode);

    public async Task AppendChangesAsync(StoryRegionCraft craft, RegionDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct)
    {
        try
        {
            var freshCraft = await LoadCraftAsync(craft.Id, languageCode, ct);
            if (freshCraft == null)
            {
                return;
            }

            var currentSnapshot = RegionDraftSnapshot.CreateFromCraft(freshCraft, languageCode);
            var diffEntries = RegionDraftSnapshotDiff.Calculate(previousSnapshot, currentSnapshot, SerializerOptions);

            if (diffEntries.Count == 0)
            {
                return;
            }

            var nextVersion = craft.LastDraftVersion + 1;
            foreach (var entry in diffEntries)
            {
                entry.Id = Guid.NewGuid();
                entry.RegionId = craft.Id;
                entry.DraftVersion = nextVersion;
                entry.LanguageCode = languageCode;
                entry.CreatedAt = DateTime.UtcNow;
                entry.CreatedBy = userId;
            }

            craft.LastDraftVersion = nextVersion;
            _context.RegionPublishChangeLogs.AddRange(diffEntries);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task<StoryRegionCraft?> LoadCraftAsync(string regionId, string languageCode, CancellationToken ct)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        return await _context.StoryRegionCrafts
            .AsNoTracking()
            .Include(x => x.Translations.Where(t => t.LanguageCode == lang))
            .FirstOrDefaultAsync(x => x.Id == regionId, ct);
    }

    private sealed class RegionDraftSnapshotDiff
    {
        public static List<RegionPublishChangeLog> Calculate(RegionDraftSnapshot? previousSnapshot, RegionDraftSnapshot currentSnapshot, JsonSerializerOptions serializerOptions)
        {
            var previous = previousSnapshot ?? RegionDraftSnapshot.Empty(currentSnapshot.LanguageCode);
            var changes = new List<RegionPublishChangeLog>();

            // Check header (ImageUrl) changes
            if (!string.Equals(previous.Header.Hash, currentSnapshot.Header.Hash, StringComparison.Ordinal))
            {
                var changeType = previous.Header.HasContent ? "Updated" : "Added";
                changes.Add(new RegionPublishChangeLog
                {
                    EntityType = RegionPublishEntityTypes.Header,
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

        private static RegionPublishChangeLog CreateTranslationChange(string changeType, RegionDraftSnapshot.TranslationState translation, JsonSerializerOptions serializerOptions)
        {
            return new RegionPublishChangeLog
            {
                EntityType = RegionPublishEntityTypes.Translation,
                EntityId = translation.LanguageCode,
                ChangeType = changeType,
                Hash = translation.Hash,
                PayloadJson = JsonSerializer.Serialize(translation.ToPayload(), serializerOptions)
            };
        }
    }

    private static class RegionPublishEntityTypes
    {
        public const string Header = "Header";
        public const string Translation = "Translation";
    }
}

public sealed class RegionDraftSnapshot
{
    private RegionDraftSnapshot(string languageCode, HeaderState header, Dictionary<string, TranslationState> translations)
    {
        LanguageCode = languageCode;
        Header = header;
        Translations = translations;
    }

    public string LanguageCode { get; }
    public HeaderState Header { get; }
    public Dictionary<string, TranslationState> Translations { get; }

    public static RegionDraftSnapshot CreateFromCraft(StoryRegionCraft craft, string languageCode)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();

        var header = HeaderState.Create(craft.ImageUrl);

        var translations = new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase);
        foreach (var translation in craft.Translations.OrderBy(t => t.LanguageCode))
        {
            var translationState = TranslationState.Create(
                translation.LanguageCode,
                translation.Name,
                translation.Description);

            translations[translationState.LanguageCode] = translationState;
        }

        return new RegionDraftSnapshot(lang, header, translations);
    }

    public static RegionDraftSnapshot Empty(string languageCode)
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
        private TranslationState(string languageCode, string name, string? description)
        {
            LanguageCode = languageCode;
            Name = name;
            Description = description;
            Hash = HashHelper.ComputeHash(new
            {
                LanguageCode,
                Name,
                Description
            });
        }

        public string LanguageCode { get; }
        public string Name { get; }
        public string? Description { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            languageCode = LanguageCode,
            name = Name,
            description = Description
        };

        public static TranslationState Create(string languageCode, string name, string? description)
            => new(languageCode, name, description);
    }
}

