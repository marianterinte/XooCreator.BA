using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryAssetCopyService
{
    List<StoryAssetPathMapper.AssetInfo> CollectFromCraft(StoryCraft craft);
    List<StoryAssetPathMapper.AssetInfo> CollectFromDefinition(StoryDefinition definition);

    Task<AssetCopyResult> CopyDraftToDraftAsync(
        IEnumerable<StoryAssetPathMapper.AssetInfo> assets,
        string sourceEmail,
        string sourceStoryId,
        string targetEmail,
        string targetStoryId,
        CancellationToken ct);

    Task<AssetCopyResult> CopyPublishedToDraftAsync(
        IEnumerable<StoryAssetPathMapper.AssetInfo> assets,
        string publishedOwnerEmail,
        string sourceStoryId,
        string targetEmail,
        string targetStoryId,
        CancellationToken ct);
}

/// <summary>
/// New asset copy service dedicated to the Copy/Fork/Version flows.
/// Implementation is intentionally minimal for now; logic will be filled
/// once Copy/Fork endpoints are wired end-to-end.
/// </summary>
public class StoryAssetCopyService : IStoryAssetCopyService
{
    private readonly IBlobSasService _sasService;
    private readonly ILogger<StoryAssetCopyService> _logger;

    public StoryAssetCopyService(
        IBlobSasService sasService,
        ILogger<StoryAssetCopyService> logger)
    {
        _sasService = sasService;
        _logger = logger;
    }

    public List<StoryAssetPathMapper.AssetInfo> CollectFromCraft(StoryCraft craft)
    {
        ArgumentNullException.ThrowIfNull(craft);
        var results = new List<StoryAssetPathMapper.AssetInfo>();

        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            results.Add(new StoryAssetPathMapper.AssetInfo(craft.CoverImageUrl, StoryAssetPathMapper.AssetType.Image, null));
        }

        foreach (var tile in craft.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(tile.ImageUrl))
            {
                results.Add(new StoryAssetPathMapper.AssetInfo(tile.ImageUrl, StoryAssetPathMapper.AssetType.Image, null));
            }

            foreach (var translation in tile.Translations)
            {
                if (!string.IsNullOrWhiteSpace(translation.AudioUrl))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(translation.AudioUrl, StoryAssetPathMapper.AssetType.Audio, translation.LanguageCode));
                }
                if (!string.IsNullOrWhiteSpace(translation.VideoUrl))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(translation.VideoUrl, StoryAssetPathMapper.AssetType.Video, translation.LanguageCode));
                }
            }
        }

        return results;
    }

    public List<StoryAssetPathMapper.AssetInfo> CollectFromDefinition(StoryDefinition definition)
    {
        ArgumentNullException.ThrowIfNull(definition);
        var results = new List<StoryAssetPathMapper.AssetInfo>();

        if (!string.IsNullOrWhiteSpace(definition.CoverImageUrl))
        {
            results.Add(new StoryAssetPathMapper.AssetInfo(Path.GetFileName(definition.CoverImageUrl), StoryAssetPathMapper.AssetType.Image, null));
        }

        foreach (var tile in definition.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(tile.ImageUrl))
            {
                results.Add(new StoryAssetPathMapper.AssetInfo(Path.GetFileName(tile.ImageUrl), StoryAssetPathMapper.AssetType.Image, null));
            }

            foreach (var translation in tile.Translations)
            {
                if (!string.IsNullOrWhiteSpace(translation.AudioUrl))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(Path.GetFileName(translation.AudioUrl), StoryAssetPathMapper.AssetType.Audio, translation.LanguageCode));
                }
                if (!string.IsNullOrWhiteSpace(translation.VideoUrl))
                {
                    results.Add(new StoryAssetPathMapper.AssetInfo(Path.GetFileName(translation.VideoUrl), StoryAssetPathMapper.AssetType.Video, translation.LanguageCode));
                }
            }
        }

        return results;
    }

    public Task<AssetCopyResult> CopyDraftToDraftAsync(
        IEnumerable<StoryAssetPathMapper.AssetInfo> assets,
        string sourceEmail,
        string sourceStoryId,
        string targetEmail,
        string targetStoryId,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Draft → Draft asset copy scheduled: source={SourceEmail}/{SourceStoryId} target={TargetEmail}/{TargetStoryId} assets={Count}",
            sourceEmail,
            sourceStoryId,
            targetEmail,
            targetStoryId,
            assets.Count());

        // Implementation will be added in a follow-up step (placeholder).
        return Task.FromResult(AssetCopyResult.Success());
    }

    public Task<AssetCopyResult> CopyPublishedToDraftAsync(
        IEnumerable<StoryAssetPathMapper.AssetInfo> assets,
        string publishedOwnerEmail,
        string sourceStoryId,
        string targetEmail,
        string targetStoryId,
        CancellationToken ct)
    {
        _logger.LogInformation(
            "Published → Draft asset copy scheduled: publishedOwner={OwnerEmail} source={SourceStoryId} target={TargetEmail}/{TargetStoryId} assets={Count}",
            publishedOwnerEmail,
            sourceStoryId,
            targetEmail,
            targetStoryId,
            assets.Count());

        // Implementation will be added in a follow-up step (placeholder).
        return Task.FromResult(AssetCopyResult.Success());
    }
}

