using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Services;

public interface IHeroDefinitionPublishChangeLogService
{
    HeroDefinitionDraftSnapshot CaptureSnapshot(HeroDefinitionCraft craft, string languageCode);
    Task AppendChangesAsync(HeroDefinitionCraft craft, HeroDefinitionDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct);
}

public class HeroDefinitionPublishChangeLogService : IHeroDefinitionPublishChangeLogService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly XooDbContext _context;

    public HeroDefinitionPublishChangeLogService(XooDbContext context)
    {
        _context = context;
    }

    public HeroDefinitionDraftSnapshot CaptureSnapshot(HeroDefinitionCraft craft, string languageCode)
        => HeroDefinitionDraftSnapshot.CreateFromCraft(craft, languageCode);

    public async Task AppendChangesAsync(HeroDefinitionCraft craft, HeroDefinitionDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct)
    {
        try
        {
            var freshCraft = await LoadCraftAsync(craft.Id, languageCode, ct);
            if (freshCraft == null) return;

            var currentSnapshot = HeroDefinitionDraftSnapshot.CreateFromCraft(freshCraft, languageCode);
            var diffEntries = HeroDefinitionDraftSnapshotDiff.Calculate(previousSnapshot, currentSnapshot, SerializerOptions);

            if (diffEntries.Count == 0) return;

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
            _context.HeroDefinitionPublishChangeLogs.AddRange(diffEntries);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception)
        {
            throw;
        }
    }

    private async Task<HeroDefinitionCraft?> LoadCraftAsync(string heroId, string languageCode, CancellationToken ct)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        return await _context.HeroDefinitionCrafts
            .AsNoTracking()
            .Include(x => x.Translations.Where(t => t.LanguageCode == lang))
            .FirstOrDefaultAsync(x => x.Id == heroId, ct);
    }

    private sealed class HeroDefinitionDraftSnapshotDiff
    {
        public static List<HeroDefinitionPublishChangeLog> Calculate(HeroDefinitionDraftSnapshot? previousSnapshot, HeroDefinitionDraftSnapshot currentSnapshot, JsonSerializerOptions serializerOptions)
        {
            var previous = previousSnapshot ?? HeroDefinitionDraftSnapshot.Empty(currentSnapshot.LanguageCode);
            var changes = new List<HeroDefinitionPublishChangeLog>();

            // Check details changes
            if (!string.Equals(previous.Details.Hash, currentSnapshot.Details.Hash, StringComparison.Ordinal))
            {
                var changeType = previous.Details.HasContent ? "Updated" : "Added";
                changes.Add(new HeroDefinitionPublishChangeLog
                {
                    EntityType = "Details",
                    ChangeType = changeType,
                    Hash = currentSnapshot.Details.Hash,
                    PayloadJson = JsonSerializer.Serialize(currentSnapshot.Details.ToPayload(), serializerOptions),
                    AssetDraftPath = currentSnapshot.Details.Image
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

        private static HeroDefinitionPublishChangeLog CreateTranslationChange(string changeType, HeroDefinitionDraftSnapshot.TranslationState translation, JsonSerializerOptions serializerOptions)
        {
            return new HeroDefinitionPublishChangeLog
            {
                EntityType = "Translation",
                EntityId = translation.LanguageCode,
                ChangeType = changeType,
                Hash = translation.Hash,
                PayloadJson = JsonSerializer.Serialize(translation.ToPayload(), serializerOptions)
            };
        }
    }
}

public sealed class HeroDefinitionDraftSnapshot
{
    private HeroDefinitionDraftSnapshot(
        string languageCode,
        DetailsState details,
        Dictionary<string, TranslationState> translations)
    {
        LanguageCode = languageCode;
        Details = details;
        Translations = translations;
    }

    public string LanguageCode { get; }
    public DetailsState Details { get; }
    public Dictionary<string, TranslationState> Translations { get; }

    public static HeroDefinitionDraftSnapshot CreateFromCraft(HeroDefinitionCraft craft, string languageCode)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        
        var details = DetailsState.Create(
            craft.CourageCost,
            craft.CuriosityCost,
            craft.ThinkingCost,
            craft.CreativityCost,
            craft.SafetyCost,
            craft.PrerequisitesJson,
            craft.RewardsJson,
            craft.IsUnlocked,
            craft.PositionX,
            craft.PositionY,
            craft.Image
        );

        var translations = new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase);
        foreach (var trans in craft.Translations)
        {
            var transState = TranslationState.Create(
                trans.LanguageCode,
                trans.Name,
                trans.Description,
                trans.Story,
                trans.AudioUrl
            );
            translations[transState.LanguageCode] = transState;
        }

        return new HeroDefinitionDraftSnapshot(lang, details, translations);
    }

    public static HeroDefinitionDraftSnapshot Empty(string languageCode)
        => new(
            languageCode,
            DetailsState.Empty(),
            new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase));

    public sealed class DetailsState
    {
        private DetailsState(
            int courageCost, int curiosityCost, int thinkingCost, int creativityCost, int safetyCost,
            string? prerequisitesJson, string? rewardsJson, bool isUnlocked, double positionX, double positionY, string? image)
        {
            CourageCost = courageCost;
            CuriosityCost = curiosityCost;
            ThinkingCost = thinkingCost;
            CreativityCost = creativityCost;
            SafetyCost = safetyCost;
            PrerequisitesJson = prerequisitesJson;
            RewardsJson = rewardsJson;
            IsUnlocked = isUnlocked;
            PositionX = positionX;
            PositionY = positionY;
            Image = image;

            Hash = HashHelper.ComputeHash(new { 
                CourageCost, CuriosityCost, ThinkingCost, CreativityCost, SafetyCost, 
                PrerequisitesJson, RewardsJson, IsUnlocked, PositionX, PositionY, Image 
            });
        }

        public int CourageCost { get; }
        public int CuriosityCost { get; }
        public int ThinkingCost { get; }
        public int CreativityCost { get; }
        public int SafetyCost { get; }
        public string? PrerequisitesJson { get; }
        public string? RewardsJson { get; }
        public bool IsUnlocked { get; }
        public double PositionX { get; }
        public double PositionY { get; }
        public string? Image { get; }
        
        public string Hash { get; }
        public bool HasContent => !string.IsNullOrEmpty(Image) || !string.IsNullOrEmpty(PrerequisitesJson); // Simplified check

        public object ToPayload() => new
        {
            courageCost = CourageCost,
            curiosityCost = CuriosityCost,
            thinkingCost = ThinkingCost,
            creativityCost = CreativityCost,
            safetyCost = SafetyCost,
            prerequisitesJson = PrerequisitesJson,
            rewardsJson = RewardsJson,
            isUnlocked = IsUnlocked,
            positionX = PositionX,
            positionY = PositionY,
            image = Image
        };

        public static DetailsState Create(int courage, int curiosity, int thinking, int creativity, int safety, string? prereqs, string? rewards, bool unlocked, double x, double y, string? image)
            => new(courage, curiosity, thinking, creativity, safety, prereqs, rewards, unlocked, x, y, image);

        public static DetailsState Empty() => new(0, 0, 0, 0, 0, null, null, false, 0, 0, null);
    }

    public sealed class TranslationState
    {
        private TranslationState(string languageCode, string name, string? description, string? story, string? audioUrl)
        {
             LanguageCode = languageCode;
             Name = name;
             Description = description;
             Story = story;
             AudioUrl = audioUrl;
             Hash = HashHelper.ComputeHash(new { LanguageCode, Name, Description, Story, AudioUrl });
        }

        public string LanguageCode { get; }
        public string Name { get; }
        public string? Description { get; }
        public string? Story { get; }
        public string? AudioUrl { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            languageCode = LanguageCode,
            name = Name,
            description = Description,
            story = Story,
            audioUrl = AudioUrl
        };

        public static TranslationState Create(string languageCode, string name, string? description, string? story, string? audioUrl)
            => new(languageCode, name, description, story, audioUrl);
    }
}
