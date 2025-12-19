using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IStoryEpicPublishingService
{
    Task<AssetCopyResult> CopyEpicAssetsAsync(List<EpicAssetInfo> assets, string ownerEmail, string epicId, CancellationToken ct);
    
    // Method for publishing from StoryEpicCraft to StoryEpicDefinition
    Task PublishFromCraftAsync(StoryEpicCraft craft, string requestedByEmail, string langTag, bool forceFull, CancellationToken ct = default);
}

public class ValidationResult
{
    public bool IsValid { get; set; }
    public string? ErrorMessage { get; set; }
    
    public static ValidationResult Success() => new() { IsValid = true };
    public static ValidationResult Unauthorized() => new() { IsValid = false, ErrorMessage = "You do not own this epic." };
    public static ValidationResult InvalidStatus(string message) => new() { IsValid = false, ErrorMessage = message };
    public static ValidationResult NoRegions() => new() { IsValid = false, ErrorMessage = "Epic must have at least one region." };
    public static ValidationResult NoStories() => new() { IsValid = false, ErrorMessage = "Epic must have at least one story." };
    public static ValidationResult UnpublishedRegions(List<StoryRegionDefinition> regions) => new() { IsValid = false, ErrorMessage = $"The following regions are not published: {string.Join(", ", regions.Select(r => r.Id))}" };
    public static ValidationResult UnpublishedHeroes(List<EpicHeroDefinition> heroes) => new() { IsValid = false, ErrorMessage = $"The following heroes are not published: {string.Join(", ", heroes.Select(h => h.Id))}" };
    public static ValidationResult UnpublishedStories(List<string> storyIds) => new() { IsValid = false, ErrorMessage = $"The following stories are not published: {string.Join(", ", storyIds)}" };
}

public class EpicAssetInfo
{
    public EpicAssetType Type { get; set; }
    public string? StoryId { get; set; } // Pentru reward images
    public required string DraftPath { get; set; }
    public required string PublishedPath { get; set; }
}

public enum EpicAssetType
{
    Cover,
    Reward
}

public class AssetCopyResult
{
    public bool HasError { get; set; }
    public string? ErrorMessage { get; set; }
    public string? FailedAsset { get; set; }
    
    public static AssetCopyResult Success() => new() { HasError = false };
    public static AssetCopyResult AssetNotFound(string assetPath, string epicId) => new() 
    { 
        HasError = true, 
        ErrorMessage = $"Asset not found: {assetPath}",
        FailedAsset = assetPath
    };
    public static AssetCopyResult CopyFailed(string assetPath, string reason) => new() 
    { 
        HasError = true, 
        ErrorMessage = $"Failed to copy asset {assetPath}: {reason}",
        FailedAsset = assetPath
    };
}
