using System.Linq;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IEpicPublishChangeLogService
{
    EpicDraftSnapshot CaptureSnapshot(StoryEpicCraft craft, string languageCode);
    Task AppendChangesAsync(StoryEpicCraft craft, EpicDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct);
}

public class EpicPublishChangeLogService : IEpicPublishChangeLogService
{
    private static readonly JsonSerializerOptions SerializerOptions = new(JsonSerializerOptions.Default)
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        WriteIndented = false
    };

    private readonly XooDbContext _context;

    public EpicPublishChangeLogService(XooDbContext context)
    {
        _context = context;
    }

    public EpicDraftSnapshot CaptureSnapshot(StoryEpicCraft craft, string languageCode)
        => EpicDraftSnapshot.CreateFromCraft(craft, languageCode);

    public async Task AppendChangesAsync(StoryEpicCraft craft, EpicDraftSnapshot? previousSnapshot, string languageCode, Guid userId, CancellationToken ct)
    {
        try
        {
            var freshCraft = await LoadCraftAsync(craft.Id, languageCode, ct);
            if (freshCraft == null)
            {
                return;
            }

            var currentSnapshot = EpicDraftSnapshot.CreateFromCraft(freshCraft, languageCode);
            var diffEntries = EpicDraftSnapshotDiff.Calculate(previousSnapshot, currentSnapshot, SerializerOptions);

            if (diffEntries.Count == 0)
            {
                return;
            }

            var nextVersion = craft.LastDraftVersion + 1;
            foreach (var entry in diffEntries)
            {
                entry.Id = Guid.NewGuid();
                entry.EpicId = craft.Id;
                entry.DraftVersion = nextVersion;
                entry.LanguageCode = languageCode;
                entry.CreatedAt = DateTime.UtcNow;
                entry.CreatedBy = userId;
            }

            craft.LastDraftVersion = nextVersion;
            _context.EpicPublishChangeLogs.AddRange(diffEntries);
            await _context.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            throw;
        }
    }

    private async Task<StoryEpicCraft?> LoadCraftAsync(string epicId, string languageCode, CancellationToken ct)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        return await _context.StoryEpicCrafts
            .AsNoTracking()
            .Include(x => x.Translations.Where(t => t.LanguageCode == lang))
            .Include(x => x.Regions)
            .Include(x => x.StoryNodes)
            .Include(x => x.UnlockRules)
            .Include(x => x.HeroReferences)
            .Include(x => x.Topics).ThenInclude(t => t.StoryTopic)
            .Include(x => x.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
            .FirstOrDefaultAsync(x => x.Id == epicId, ct);
    }

    private sealed class EpicDraftSnapshotDiff
    {
        public static List<EpicPublishChangeLog> Calculate(EpicDraftSnapshot? previousSnapshot, EpicDraftSnapshot currentSnapshot, JsonSerializerOptions serializerOptions)
        {
            var previous = previousSnapshot ?? EpicDraftSnapshot.Empty(currentSnapshot.LanguageCode);
            var changes = new List<EpicPublishChangeLog>();

            // Check header changes
            if (!string.Equals(previous.Header.Hash, currentSnapshot.Header.Hash, StringComparison.Ordinal))
            {
                var changeType = previous.Header.HasContent ? "Updated" : "Added";
                changes.Add(new EpicPublishChangeLog
                {
                    EntityType = EpicPublishEntityTypes.Header,
                    ChangeType = changeType,
                    Hash = currentSnapshot.Header.Hash,
                    PayloadJson = JsonSerializer.Serialize(currentSnapshot.Header.ToPayload(), serializerOptions),
                    AssetDraftPath = currentSnapshot.Header.CoverImageUrl
                });
            }

            // Check region reference changes
            foreach (var kvp in currentSnapshot.RegionReferences)
            {
                if (!previous.RegionReferences.TryGetValue(kvp.Key, out var oldRegion))
                {
                    changes.Add(CreateRegionReferenceChange("Added", kvp.Value, serializerOptions));
                }
                else if (!string.Equals(oldRegion.Hash, kvp.Value.Hash, StringComparison.Ordinal))
                {
                    changes.Add(CreateRegionReferenceChange("Updated", kvp.Value, serializerOptions));
                }
            }

            foreach (var kvp in previous.RegionReferences)
            {
                if (!currentSnapshot.RegionReferences.ContainsKey(kvp.Key))
                {
                    changes.Add(CreateRegionReferenceChange("Removed", kvp.Value, serializerOptions));
                }
            }

            // Check story node changes
            foreach (var kvp in currentSnapshot.StoryNodes)
            {
                if (!previous.StoryNodes.TryGetValue(kvp.Key, out var oldNode))
                {
                    changes.Add(CreateStoryNodeChange("Added", kvp.Value, serializerOptions));
                }
                else if (!string.Equals(oldNode.Hash, kvp.Value.Hash, StringComparison.Ordinal))
                {
                    changes.Add(CreateStoryNodeChange("Updated", kvp.Value, serializerOptions));
                }
            }

            foreach (var kvp in previous.StoryNodes)
            {
                if (!currentSnapshot.StoryNodes.ContainsKey(kvp.Key))
                {
                    changes.Add(CreateStoryNodeChange("Removed", kvp.Value, serializerOptions));
                }
            }

            // Check unlock rule changes
            foreach (var kvp in currentSnapshot.UnlockRules)
            {
                if (!previous.UnlockRules.TryGetValue(kvp.Key, out var oldRule))
                {
                    changes.Add(CreateUnlockRuleChange("Added", kvp.Value, serializerOptions));
                }
                else if (!string.Equals(oldRule.Hash, kvp.Value.Hash, StringComparison.Ordinal))
                {
                    changes.Add(CreateUnlockRuleChange("Updated", kvp.Value, serializerOptions));
                }
            }

            foreach (var kvp in previous.UnlockRules)
            {
                if (!currentSnapshot.UnlockRules.ContainsKey(kvp.Key))
                {
                    changes.Add(CreateUnlockRuleChange("Removed", kvp.Value, serializerOptions));
                }
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

            // Check hero reference changes
            foreach (var kvp in currentSnapshot.HeroReferences)
            {
                if (!previous.HeroReferences.TryGetValue(kvp.Key, out var oldHeroRef))
                {
                    changes.Add(CreateHeroReferenceChange("Added", kvp.Value, serializerOptions));
                }
                else if (!string.Equals(oldHeroRef.Hash, kvp.Value.Hash, StringComparison.Ordinal))
                {
                    changes.Add(CreateHeroReferenceChange("Updated", kvp.Value, serializerOptions));
                }
            }

            foreach (var kvp in previous.HeroReferences)
            {
                if (!currentSnapshot.HeroReferences.ContainsKey(kvp.Key))
                {
                    changes.Add(CreateHeroReferenceChange("Removed", kvp.Value, serializerOptions));
                }
            }

            return changes;
        }

        private static EpicPublishChangeLog CreateRegionReferenceChange(string changeType, EpicDraftSnapshot.RegionReferenceState region, JsonSerializerOptions serializerOptions)
        {
            return new EpicPublishChangeLog
            {
                EntityType = EpicPublishEntityTypes.RegionReference,
                EntityId = region.RegionId,
                ChangeType = changeType,
                Hash = region.Hash,
                PayloadJson = JsonSerializer.Serialize(region.ToPayload(), serializerOptions),
                AssetDraftPath = region.ImageUrl
            };
        }

        private static EpicPublishChangeLog CreateStoryNodeChange(string changeType, EpicDraftSnapshot.StoryNodeState node, JsonSerializerOptions serializerOptions)
        {
            return new EpicPublishChangeLog
            {
                EntityType = EpicPublishEntityTypes.StoryNode,
                EntityId = node.StoryId,
                ChangeType = changeType,
                Hash = node.Hash,
                PayloadJson = JsonSerializer.Serialize(node.ToPayload(), serializerOptions),
                AssetDraftPath = node.RewardImageUrl
            };
        }

        private static EpicPublishChangeLog CreateUnlockRuleChange(string changeType, EpicDraftSnapshot.UnlockRuleState rule, JsonSerializerOptions serializerOptions)
        {
            return new EpicPublishChangeLog
            {
                EntityType = EpicPublishEntityTypes.UnlockRule,
                EntityId = rule.RuleId,
                ChangeType = changeType,
                Hash = rule.Hash,
                PayloadJson = JsonSerializer.Serialize(rule.ToPayload(), serializerOptions)
            };
        }

        private static EpicPublishChangeLog CreateTranslationChange(string changeType, EpicDraftSnapshot.TranslationState translation, JsonSerializerOptions serializerOptions)
        {
            return new EpicPublishChangeLog
            {
                EntityType = EpicPublishEntityTypes.Translation,
                EntityId = translation.LanguageCode,
                ChangeType = changeType,
                Hash = translation.Hash,
                PayloadJson = JsonSerializer.Serialize(translation.ToPayload(), serializerOptions)
            };
        }

        private static EpicPublishChangeLog CreateHeroReferenceChange(string changeType, EpicDraftSnapshot.HeroReferenceState heroRef, JsonSerializerOptions serializerOptions)
        {
            return new EpicPublishChangeLog
            {
                EntityType = EpicPublishEntityTypes.HeroReference,
                EntityId = heroRef.HeroId,
                ChangeType = changeType,
                Hash = heroRef.Hash,
                PayloadJson = JsonSerializer.Serialize(heroRef.ToPayload(), serializerOptions)
            };
        }
    }

    private static class EpicPublishEntityTypes
    {
        public const string Header = "Header";
        public const string RegionReference = "RegionReference";
        public const string StoryNode = "StoryNode";
        public const string UnlockRule = "UnlockRule";
        public const string Translation = "Translation";
        public const string HeroReference = "HeroReference";
    }
}

public sealed class EpicDraftSnapshot
{
    private EpicDraftSnapshot(
        string languageCode,
        HeaderState header,
        Dictionary<string, RegionReferenceState> regionReferences,
        Dictionary<string, StoryNodeState> storyNodes,
        Dictionary<string, UnlockRuleState> unlockRules,
        Dictionary<string, TranslationState> translations,
        Dictionary<string, HeroReferenceState> heroReferences)
    {
        LanguageCode = languageCode;
        Header = header;
        RegionReferences = regionReferences;
        StoryNodes = storyNodes;
        UnlockRules = unlockRules;
        Translations = translations;
        HeroReferences = heroReferences;
    }

    public string LanguageCode { get; }
    public HeaderState Header { get; }
    public Dictionary<string, RegionReferenceState> RegionReferences { get; }
    public Dictionary<string, StoryNodeState> StoryNodes { get; }
    public Dictionary<string, UnlockRuleState> UnlockRules { get; }
    public Dictionary<string, TranslationState> Translations { get; }
    public Dictionary<string, HeroReferenceState> HeroReferences { get; }

    public static EpicDraftSnapshot CreateFromCraft(StoryEpicCraft craft, string languageCode)
    {
        var lang = (languageCode ?? string.Empty).ToLowerInvariant();
        var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode == lang);

        var header = HeaderState.Create(
            craft.Name,
            craft.Description,
            craft.CoverImageUrl,
            craft.MarketplaceCoverImageUrl,
            craft.Topics.Select(t => t.StoryTopic?.TopicId ?? t.StoryTopicId.ToString()),
            craft.AgeGroups.Select(ag => ag.StoryAgeGroup?.AgeGroupId ?? ag.StoryAgeGroupId.ToString()));

        var regionReferences = new Dictionary<string, RegionReferenceState>(StringComparer.OrdinalIgnoreCase);
        foreach (var region in craft.Regions.OrderBy(r => r.SortOrder))
        {
            var regionState = RegionReferenceState.Create(
                region.RegionId,
                region.Label,
                region.ImageUrl,
                region.SortOrder,
                region.IsLocked,
                region.IsStartupRegion,
                region.X,
                region.Y);
            regionReferences[regionState.RegionId] = regionState;
        }

        var storyNodes = new Dictionary<string, StoryNodeState>(StringComparer.OrdinalIgnoreCase);
        foreach (var node in craft.StoryNodes.OrderBy(n => n.SortOrder))
        {
            var nodeState = StoryNodeState.Create(
                node.StoryId,
                node.RegionId,
                node.RewardImageUrl,
                node.SortOrder,
                node.X,
                node.Y);
            storyNodes[nodeState.StoryId] = nodeState;
        }

        var unlockRules = new Dictionary<string, UnlockRuleState>(StringComparer.OrdinalIgnoreCase);
        foreach (var rule in craft.UnlockRules.OrderBy(r => r.SortOrder))
        {
            var ruleId = $"{rule.Type}:{rule.FromId}:{rule.ToRegionId}:{rule.ToStoryId}";
            var ruleState = UnlockRuleState.Create(
                ruleId,
                rule.Type,
                rule.FromId,
                rule.ToRegionId,
                rule.ToStoryId,
                rule.RequiredStoriesCsv,
                rule.MinCount,
                rule.StoryId,
                rule.SortOrder);
            unlockRules[ruleState.RuleId] = ruleState;
        }

        var translations = new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase);
        foreach (var trans in craft.Translations.OrderBy(t => t.LanguageCode))
        {
            var transState = TranslationState.Create(
                trans.LanguageCode,
                trans.Name,
                trans.Description);
            translations[transState.LanguageCode] = transState;
        }

        var heroReferences = new Dictionary<string, HeroReferenceState>(StringComparer.OrdinalIgnoreCase);
        foreach (var heroRef in craft.HeroReferences)
        {
            var heroRefState = HeroReferenceState.Create(
                heroRef.HeroId,
                heroRef.StoryId);
            heroReferences[heroRefState.HeroId] = heroRefState;
        }

        return new EpicDraftSnapshot(lang, header, regionReferences, storyNodes, unlockRules, translations, heroReferences);
    }

    public static EpicDraftSnapshot Empty(string languageCode)
        => new(
            languageCode,
            HeaderState.Empty(),
            new Dictionary<string, RegionReferenceState>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, StoryNodeState>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, UnlockRuleState>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, TranslationState>(StringComparer.OrdinalIgnoreCase),
            new Dictionary<string, HeroReferenceState>(StringComparer.OrdinalIgnoreCase));

    public sealed class HeaderState
    {
        private HeaderState(string name, string? description, string? coverImageUrl, string? marketplaceCoverImageUrl, IReadOnlyList<string> topicIds, IReadOnlyList<string> ageGroupIds)
        {
            Name = name;
            Description = description;
            CoverImageUrl = coverImageUrl;
            MarketplaceCoverImageUrl = marketplaceCoverImageUrl;
            TopicIds = topicIds;
            AgeGroupIds = ageGroupIds;
            Hash = HashHelper.ComputeHash(new { Name, Description, CoverImageUrl, MarketplaceCoverImageUrl, TopicIds, AgeGroupIds });
        }

        public string Name { get; }
        public string? Description { get; }
        public string? CoverImageUrl { get; }
        public string? MarketplaceCoverImageUrl { get; }
        public IReadOnlyList<string> TopicIds { get; }
        public IReadOnlyList<string> AgeGroupIds { get; }
        public string Hash { get; }
        public bool HasContent => !string.IsNullOrEmpty(Name) || !string.IsNullOrEmpty(Description) || !string.IsNullOrEmpty(CoverImageUrl) || !string.IsNullOrEmpty(MarketplaceCoverImageUrl);

        public object ToPayload() => new
        {
            name = Name,
            description = Description,
            coverImageUrl = CoverImageUrl,
            marketplaceCoverImageUrl = MarketplaceCoverImageUrl,
            topicIds = TopicIds,
            ageGroupIds = AgeGroupIds
        };

        public static HeaderState Create(string name, string? description, string? coverImageUrl, string? marketplaceCoverImageUrl, IEnumerable<string> topicIds, IEnumerable<string> ageGroupIds)
        {
            var normalizedTopicIds = topicIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                .ToList();
            var normalizedAgeGroupIds = ageGroupIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .OrderBy(id => id, StringComparer.OrdinalIgnoreCase)
                .ToList();

            return new HeaderState(name, description, coverImageUrl, marketplaceCoverImageUrl, normalizedTopicIds, normalizedAgeGroupIds);
        }

        public static HeaderState Empty() => new(string.Empty, null, null, null, Array.Empty<string>(), Array.Empty<string>());
    }

    public sealed class RegionReferenceState
    {
        private RegionReferenceState(string regionId, string label, string? imageUrl, int sortOrder, bool isLocked, bool isStartupRegion, double? x, double? y)
        {
            RegionId = regionId;
            Label = label;
            ImageUrl = imageUrl;
            SortOrder = sortOrder;
            IsLocked = isLocked;
            IsStartupRegion = isStartupRegion;
            X = x;
            Y = y;
            Hash = HashHelper.ComputeHash(new { RegionId, Label, ImageUrl, SortOrder, IsLocked, IsStartupRegion, X, Y });
        }

        public string RegionId { get; }
        public string Label { get; }
        public string? ImageUrl { get; }
        public int SortOrder { get; }
        public bool IsLocked { get; }
        public bool IsStartupRegion { get; }
        public double? X { get; }
        public double? Y { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            regionId = RegionId,
            label = Label,
            imageUrl = ImageUrl,
            sortOrder = SortOrder,
            isLocked = IsLocked,
            isStartupRegion = IsStartupRegion,
            x = X,
            y = Y
        };

        public static RegionReferenceState Create(string regionId, string label, string? imageUrl, int sortOrder, bool isLocked, bool isStartupRegion, double? x, double? y)
            => new(regionId, label, imageUrl, sortOrder, isLocked, isStartupRegion, x, y);
    }

    public sealed class StoryNodeState
    {
        private StoryNodeState(string storyId, string regionId, string? rewardImageUrl, int sortOrder, double? x, double? y)
        {
            StoryId = storyId;
            RegionId = regionId;
            RewardImageUrl = rewardImageUrl;
            SortOrder = sortOrder;
            X = x;
            Y = y;
            Hash = HashHelper.ComputeHash(new { StoryId, RegionId, RewardImageUrl, SortOrder, X, Y });
        }

        public string StoryId { get; }
        public string RegionId { get; }
        public string? RewardImageUrl { get; }
        public int SortOrder { get; }
        public double? X { get; }
        public double? Y { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            storyId = StoryId,
            regionId = RegionId,
            rewardImageUrl = RewardImageUrl,
            sortOrder = SortOrder,
            x = X,
            y = Y
        };

        public static StoryNodeState Create(string storyId, string regionId, string? rewardImageUrl, int sortOrder, double? x, double? y)
            => new(storyId, regionId, rewardImageUrl, sortOrder, x, y);
    }

    public sealed class UnlockRuleState
    {
        private UnlockRuleState(string ruleId, string type, string fromId, string toRegionId, string? toStoryId, string? requiredStoriesCsv, int? minCount, string? storyId, int sortOrder)
        {
            RuleId = ruleId;
            Type = type;
            FromId = fromId;
            ToRegionId = toRegionId;
            ToStoryId = toStoryId;
            RequiredStoriesCsv = requiredStoriesCsv;
            MinCount = minCount;
            StoryId = storyId;
            SortOrder = sortOrder;
            Hash = HashHelper.ComputeHash(new { RuleId, Type, FromId, ToRegionId, ToStoryId, RequiredStoriesCsv, MinCount, StoryId, SortOrder });
        }

        public string RuleId { get; }
        public string Type { get; }
        public string FromId { get; }
        public string ToRegionId { get; }
        public string? ToStoryId { get; }
        public string? RequiredStoriesCsv { get; }
        public int? MinCount { get; }
        public string? StoryId { get; }
        public int SortOrder { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            ruleId = RuleId,
            type = Type,
            fromId = FromId,
            toRegionId = ToRegionId,
            toStoryId = ToStoryId,
            requiredStoriesCsv = RequiredStoriesCsv,
            minCount = MinCount,
            storyId = StoryId,
            sortOrder = SortOrder
        };

        public static UnlockRuleState Create(string ruleId, string type, string fromId, string toRegionId, string? toStoryId, string? requiredStoriesCsv, int? minCount, string? storyId, int sortOrder)
            => new(ruleId, type, fromId, toRegionId, toStoryId, requiredStoriesCsv, minCount, storyId, sortOrder);
    }

    public sealed class TranslationState
    {
        private TranslationState(string languageCode, string name, string? description)
        {
            LanguageCode = languageCode;
            Name = name;
            Description = description;
            Hash = HashHelper.ComputeHash(new { LanguageCode, Name, Description });
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

    public sealed class HeroReferenceState
    {
        private HeroReferenceState(string heroId, string? storyId)
        {
            HeroId = heroId;
            StoryId = storyId;
            Hash = HashHelper.ComputeHash(new { HeroId, StoryId });
        }

        public string HeroId { get; }
        public string? StoryId { get; }
        public string Hash { get; }

        public object ToPayload() => new
        {
            heroId = HeroId,
            storyId = StoryId
        };

        public static HeroReferenceState Create(string heroId, string? storyId)
            => new(heroId, storyId);
    }
}

