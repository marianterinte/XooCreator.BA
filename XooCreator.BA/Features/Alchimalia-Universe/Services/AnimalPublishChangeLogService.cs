using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface IAnimalPublishChangeLogService
{
    AnimalDraftSnapshot CaptureSnapshot(AnimalCraft craft, string languageCode);
    Task AppendChangesAsync(AnimalCraft craft, AnimalDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct);
}

public class AnimalPublishChangeLogService : IAnimalPublishChangeLogService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly XooDbContext _context;

    public AnimalPublishChangeLogService(XooDbContext context)
    {
        _context = context;
    }

    public AnimalDraftSnapshot CaptureSnapshot(AnimalCraft craft, string languageCode)
        => AnimalDraftSnapshot.CreateFromCraft(craft, languageCode);

    public async Task AppendChangesAsync(AnimalCraft craft, AnimalDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct)
    {
        try
        {
            var freshCraft = await LoadCraftAsync(craft.Id, languageCode, ct);
            if (freshCraft == null) return;

            var currentSnapshot = AnimalDraftSnapshot.CreateFromCraft(freshCraft, languageCode);
            var diffEntries = AnimalDraftSnapshotDiff.Calculate(previousSnapshot, currentSnapshot, SerializerOptions);

            if (diffEntries.Count == 0) return;

            // AnimalCraft does not explicitly store LastDraftVersion in all snippets I saw, but it was in my update list.
            // I'll assume it exists as I added it.
            // Wait, I added it to AnimalCraft.cs in Phase 1.
            // But let's verify if LastDraftVersion is there. I added it.

            // Wait, does AnimalCraft store LastDraftVersion as int?
            // Yes, I added: public int LastDraftVersion { get; set; } = 0;

            var nextVersion = (craft.LastDraftVersion) + 1;
            foreach (var entry in diffEntries)
            {
                entry.Id = Guid.NewGuid();
                entry.AnimalId = craft.Id;
                entry.DraftVersion = nextVersion;
                entry.LanguageCode = languageCode;
                entry.CreatedAt = DateTime.UtcNow;
                entry.CreatedBy = userId;
            }

            craft.LastDraftVersion = nextVersion;
            _context.AnimalPublishChangeLogs.AddRange(diffEntries);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<AnimalCraft?> LoadCraftAsync(Guid animalId, string languageCode, CancellationToken ct)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        return await _context.AnimalCrafts
            .AsNoTracking()
            .Include(x => x.Translations.Where(t => t.LanguageCode == lang))
            .Include(x => x.SupportedParts)
            .Include(x => x.HybridParts)
            .FirstOrDefaultAsync(x => x.Id == animalId, ct);
    }

    private sealed class AnimalDraftSnapshotDiff
    {
        public static List<AnimalPublishChangeLog> Calculate(AnimalDraftSnapshot? previousSnapshot, AnimalDraftSnapshot currentSnapshot, JsonSerializerOptions serializerOptions)
        {
            var previous = previousSnapshot ?? AnimalDraftSnapshot.Empty(currentSnapshot.LanguageCode);
            var changes = new List<AnimalPublishChangeLog>();

            // Check details changes
            if (!string.Equals(previous.Details.Hash, currentSnapshot.Details.Hash, StringComparison.Ordinal))
            {
                var changeType = previous.Details.HasContent ? "Updated" : "Added";
                changes.Add(new AnimalPublishChangeLog
                {
                    EntityType = "Details",
                    ChangeType = changeType,
                    Hash = currentSnapshot.Details.Hash,
                    PayloadJson = JsonSerializer.Serialize(currentSnapshot.Details.ToPayload(), serializerOptions),
                    AssetDraftPath = currentSnapshot.Details.Src
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
            
            // Check supported parts
            foreach (var kvp in currentSnapshot.SupportedParts)
            {
                 if (!previous.SupportedParts.TryGetValue(kvp.Key, out var oldPart))
                {
                    changes.Add(CreateSupportedPartChange("Added", kvp.Value, serializerOptions));
                }
                // No update for simple key existence? Actually if key exists it's same.
            }
             foreach (var kvp in previous.SupportedParts)
            {
                if (!currentSnapshot.SupportedParts.ContainsKey(kvp.Key))
                {
                    changes.Add(CreateSupportedPartChange("Removed", kvp.Value, serializerOptions));
                }
            }

            // Check hybrid parts
            foreach (var kvp in currentSnapshot.HybridParts)
            {
                 if (!previous.HybridParts.TryGetValue(kvp.Key, out var oldPart))
                {
                    changes.Add(CreateHybridPartChange("Added", kvp.Value, serializerOptions));
                }
                else if (!string.Equals(oldPart.Hash, kvp.Value.Hash, StringComparison.Ordinal))
                {
                    // Usually hybrid part is identified by something?
                    // Actually key is BodyPartKey. If details change (SourceAnimalId) then Updated.
                    changes.Add(CreateHybridPartChange("Updated", kvp.Value, serializerOptions));
                }
            }
             foreach (var kvp in previous.HybridParts)
            {
                if (!currentSnapshot.HybridParts.ContainsKey(kvp.Key))
                {
                    changes.Add(CreateHybridPartChange("Removed", kvp.Value, serializerOptions));
                }
            }

            return changes;
        }

        private static AnimalPublishChangeLog CreateTranslationChange(string changeType, AnimalDraftSnapshot.TranslationState translation, JsonSerializerOptions serializerOptions)
        {
            return new AnimalPublishChangeLog
            {
                EntityType = "Translation",
                EntityId = translation.LanguageCode,
                ChangeType = changeType,
                Hash = translation.Hash,
                PayloadJson = JsonSerializer.Serialize(translation.ToPayload(), serializerOptions)
            };
        }
        
        private static AnimalPublishChangeLog CreateSupportedPartChange(string changeType, AnimalDraftSnapshot.SupportedPartState part, JsonSerializerOptions serializerOptions)
        {
            return new AnimalPublishChangeLog
            {
                EntityType = "SupportedPart",
                EntityId = part.BodyPartKey,
                ChangeType = changeType,
                Hash = part.Hash,
                PayloadJson = JsonSerializer.Serialize(part.ToPayload(), serializerOptions)
            };
        }

        private static AnimalPublishChangeLog CreateHybridPartChange(string changeType, AnimalDraftSnapshot.HybridPartState part, JsonSerializerOptions serializerOptions)
        {
            return new AnimalPublishChangeLog
            {
                EntityType = "HybridPart",
                EntityId = part.BodyPartKey, // Keyed by BodyPartKey
                ChangeType = changeType,
                Hash = part.Hash,
                PayloadJson = JsonSerializer.Serialize(part.ToPayload(), serializerOptions)
            };
        }
    }
}

public sealed class AnimalDraftSnapshot
{
    private AnimalDraftSnapshot(
        string languageCode,
        DetailsState details,
        Dictionary<string, TranslationState> translations,
        Dictionary<string, SupportedPartState> supportedParts,
        Dictionary<string, HybridPartState> hybridParts)
    {
        LanguageCode = languageCode;
        Details = details;
        Translations = translations;
        SupportedParts = supportedParts;
        HybridParts = hybridParts;
    }

    public string LanguageCode { get; }
    public DetailsState Details { get; }
    public Dictionary<string, TranslationState> Translations { get; }
    public Dictionary<string, SupportedPartState> SupportedParts { get; }
    public Dictionary<string, HybridPartState> HybridParts { get; }


    public static AnimalDraftSnapshot CreateFromCraft(AnimalCraft craft, string languageCode)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        
        var details = DetailsState.Create(
            craft.Label,
            craft.Src,
            craft.IsHybrid,
            craft.RegionId
        );

        var translations = new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase);
        foreach (var trans in craft.Translations)
        {
            var transState = TranslationState.Create(
                trans.LanguageCode,
                trans.Label,
                trans.AudioUrl
            );
            translations[transState.LanguageCode] = transState;
        }
        
        var supportedParts = new Dictionary<string, SupportedPartState>(StringComparer.OrdinalIgnoreCase);
        foreach(var part in craft.SupportedParts)
        {
             var state = SupportedPartState.Create(part.BodyPartKey);
             supportedParts[state.BodyPartKey] = state;
        }

        var hybridParts = new Dictionary<string, HybridPartState>(StringComparer.OrdinalIgnoreCase);
        foreach(var part in craft.HybridParts)
        {
             var state = HybridPartState.Create(part.BodyPartKey, part.SourceAnimalId, part.OrderIndex);
             hybridParts[state.BodyPartKey] = state;
        }

        return new AnimalDraftSnapshot(lang, details, translations, supportedParts, hybridParts);
    }

    public static AnimalDraftSnapshot Empty(string languageCode)
        => new(
            languageCode,
            DetailsState.Empty(),
            new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, SupportedPartState>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, HybridPartState>(StringComparer.OrdinalIgnoreCase));

    public sealed class DetailsState
    {
        private DetailsState(
            string label, string src, bool isHybrid, Guid? regionId)
        {
            Label = label;
            Src = src;
            IsHybrid = isHybrid;
            RegionId = regionId;

            Hash = HashHelper.ComputeHash(new { 
                Label, Src, IsHybrid, RegionId
            });
        }

        public string Label { get; }
        public string Src { get; }
        public bool IsHybrid { get; }
        public Guid? RegionId { get; }
        
        public string Hash { get; }
        public bool HasContent => !string.IsNullOrEmpty(Label) || !string.IsNullOrEmpty(Src);

        public object ToPayload() => new
        {
            label = Label,
            src = Src,
            isHybrid = IsHybrid,
            regionId = RegionId
        };

        public static DetailsState Create(string label, string src, bool isHybrid, Guid? regionId)
            => new(label, src, isHybrid, regionId);

        public static DetailsState Empty() => new(string.Empty, string.Empty, false, null);
    }

    public sealed class TranslationState
    {
        private TranslationState(string languageCode, string label, string? audioUrl)
        {
             LanguageCode = languageCode;
             Label = label;
             AudioUrl = audioUrl;
             Hash = HashHelper.ComputeHash(new { LanguageCode, Label, AudioUrl });
        }

        public string LanguageCode { get; }
        public string Label { get; }
        public string? AudioUrl { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            languageCode = LanguageCode,
            label = Label,
            audioUrl = AudioUrl
        };

        public static TranslationState Create(string languageCode, string label, string? audioUrl)
            => new(languageCode, label, audioUrl);
    }
    
    public sealed class SupportedPartState
    {
        private SupportedPartState(string bodyPartKey)
        {
             BodyPartKey = bodyPartKey;
             Hash = HashHelper.ComputeHash(new { BodyPartKey });
        }

        public string BodyPartKey { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            bodyPartKey = BodyPartKey
        };

        public static SupportedPartState Create(string bodyPartKey)
            => new(bodyPartKey);
    }

    public sealed class HybridPartState
    {
        private HybridPartState(string bodyPartKey, Guid sourceAnimalId, int orderIndex)
        {
             BodyPartKey = bodyPartKey;
             SourceAnimalId = sourceAnimalId;
             OrderIndex = orderIndex;
             Hash = HashHelper.ComputeHash(new { BodyPartKey, SourceAnimalId, OrderIndex });
        }

        public string BodyPartKey { get; }
        public Guid SourceAnimalId { get; }
        public int OrderIndex { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            bodyPartKey = BodyPartKey,
            sourceAnimalId = SourceAnimalId,
            orderIndex = OrderIndex
        };

        public static HybridPartState Create(string bodyPartKey, Guid sourceAnimalId, int orderIndex)
            => new(bodyPartKey, sourceAnimalId, orderIndex);
    }
}
