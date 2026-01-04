using System.IO.Compression;
using System.Text;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public class EpicExportService : IEpicExportService
{
    private readonly XooDbContext _context;
    private readonly IStoryEpicService _epicService;
    private readonly IStoryRegionService _regionService;
    private readonly IEpicHeroService _heroService;
    private readonly IStoryEditorService _storyService;
    private readonly IBlobSasService _sas;
    private readonly ILogger<EpicExportService> _logger;

    public EpicExportService(
        XooDbContext context,
        IStoryEpicService epicService,
        IStoryRegionService regionService,
        IEpicHeroService heroService,
        IStoryEditorService storyService,
        IBlobSasService sas,
        ILogger<EpicExportService> logger)
    {
        _context = context;
        _epicService = epicService;
        _regionService = regionService;
        _heroService = heroService;
        _storyService = storyService;
        _sas = sas;
        _logger = logger;
    }

    public async Task<EpicExportManifestDto> BuildManifestAsync(string epicId, EpicExportRequest options, bool isDraft, CancellationToken ct)
    {
        // 1. Load epic data
        var epic = await _epicService.GetAsync(epicId, isDraft, ct);
        if (epic == null)
        {
            throw new InvalidOperationException($"Epic {epicId} not found");
        }

        // 2. Build epic data
        var epicData = new EpicExportDataDto
        {
            Id = epic.Id,
            Name = epic.Name,
            Description = epic.Description,
            CoverImageUrl = epic.CoverImageUrl,
            Status = epic.Status,
            PublishedAtUtc = epic.PublishedAtUtc,
            Translations = epic.Translations.Select(t => new EpicTranslationExportDto
            {
                LanguageCode = t.LanguageCode,
                Name = t.Name,
                Description = t.Description
            }).ToList()
        };

        // 3. Load regions if requested
        var regions = new Dictionary<string, RegionExportDto>();
        if (options.IncludeRegions)
        {
            foreach (var regionRef in epic.Regions)
            {
                var region = await _regionService.GetAsync(regionRef.Id, isDraft, ct);
                if (region != null)
                {
                    regions[region.Id] = new RegionExportDto
                    {
                        Id = region.Id,
                        Label = regionRef.Label,
                        ImageUrl = region.ImageUrl,
                        SortOrder = regionRef.SortOrder,
                        IsLocked = regionRef.IsLocked,
                        IsStartupRegion = regionRef.IsStartupRegion,
                        X = regionRef.X,
                        Y = regionRef.Y,
                        Translations = region.Translations.Select(t => new RegionTranslationExportDto
                        {
                            LanguageCode = t.LanguageCode,
                            Name = t.Name,
                            Description = t.Description
                        }).ToList()
                    };
                }
            }
        }

        // 4. Load heroes if requested
        var heroes = new Dictionary<string, HeroExportDto>();
        if (options.IncludeHeroes)
        {
            foreach (var heroRef in epic.Heroes)
            {
                var hero = await _heroService.GetAsync(heroRef.HeroId, isDraft, ct);
                if (hero != null)
                {
                    heroes[hero.Id] = new HeroExportDto
                    {
                        Id = hero.Id,
                        ImageUrl = hero.ImageUrl,
                        GreetingAudioUrl = hero.GreetingAudioUrl,
                        Status = hero.Status,
                        Translations = hero.Translations.Select(t => new HeroTranslationExportDto
                        {
                            LanguageCode = t.LanguageCode,
                            Name = t.Name,
                            Description = t.Description,
                            GreetingText = t.GreetingText,
                            GreetingAudioUrl = t.GreetingAudioUrl
                        }).ToList()
                    };
                }
            }
        }

        // 5. Load stories if requested
        var stories = new Dictionary<string, StoryExportDto>();
        if (options.IncludeStories)
        {
            foreach (var storyNode in epic.Stories)
            {
                var story = isDraft
                    ? await _storyService.GetDraftAsync(storyNode.StoryId, ct)
                    : await _storyService.GetPublishedStoryAsync(storyNode.StoryId, ct);

                if (story != null)
                {
                    stories[story.StoryId] = await BuildStoryExportDto(story, isDraft, ct);
                }
            }
        }

        // 6. Build epic structure
        var epicStructure = new EpicStructureExportDto
        {
            Regions = epic.Regions.Select(r => new EpicRegionReferenceDto
            {
                Id = r.Id,
                Label = r.Label,
                ImageUrl = r.ImageUrl,
                SortOrder = r.SortOrder,
                IsLocked = r.IsLocked,
                IsStartupRegion = r.IsStartupRegion,
                X = r.X,
                Y = r.Y
            }).ToList(),
            StoryNodes = epic.Stories.Select(s => new EpicStoryNodeDto
            {
                StoryId = s.StoryId,
                RegionId = s.RegionId,
                RewardImageUrl = s.RewardImageUrl,
                SortOrder = s.SortOrder,
                X = s.X,
                Y = s.Y
            }).ToList(),
            UnlockRules = epic.Rules.Select(r => new EpicUnlockRuleDto
            {
                Type = r.Type,
                FromId = r.FromId,
                ToRegionId = r.ToRegionId,
                ToStoryId = r.ToStoryId,
                RequiredStories = r.RequiredStories,
                MinCount = r.MinCount,
                StoryId = r.StoryId,
                SortOrder = r.SortOrder
            }).ToList(),
            HeroReferences = epic.Heroes.Select(h => new EpicHeroReferenceDto
            {
                HeroId = h.HeroId,
                StoryId = h.StoryId,
                HeroName = h.HeroName,
                HeroImageUrl = h.HeroImageUrl
            }).ToList()
        };

        // 7. Build asset map (will be populated during ZIP creation)
        var assetMap = new Dictionary<string, string>();

        return new EpicExportManifestDto
        {
            Version = "1.0",
            ExportedAtUtc = DateTime.UtcNow,
            EpicData = epicData,
            Regions = regions,
            Heroes = heroes,
            Stories = stories,
            EpicStructure = epicStructure,
            AssetMap = assetMap
        };
    }

    public async Task<Stream> CreateZipArchiveAsync(EpicExportManifestDto manifest, string epicId, bool isDraft, CancellationToken ct)
    {
        var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // 1. Add manifest JSON
            var manifestJson = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
            var manifestEntry = zip.CreateEntry("manifest/epic.json", CompressionLevel.Fastest);
            await using (var writer = new StreamWriter(manifestEntry.Open(), new UTF8Encoding(false)))
            {
                await writer.WriteAsync(manifestJson);
            }

            // 2. Add region JSONs
            foreach (var (regionId, region) in manifest.Regions)
            {
                var regionJson = JsonSerializer.Serialize(region, new JsonSerializerOptions { WriteIndented = true });
                var regionEntry = zip.CreateEntry($"manifest/regions/{regionId}.json", CompressionLevel.Fastest);
                await using (var writer = new StreamWriter(regionEntry.Open(), new UTF8Encoding(false)))
                {
                    await writer.WriteAsync(regionJson);
                }
            }

            // 3. Add hero JSONs
            foreach (var (heroId, hero) in manifest.Heroes)
            {
                var heroJson = JsonSerializer.Serialize(hero, new JsonSerializerOptions { WriteIndented = true });
                var heroEntry = zip.CreateEntry($"manifest/heroes/{heroId}.json", CompressionLevel.Fastest);
                await using (var writer = new StreamWriter(heroEntry.Open(), new UTF8Encoding(false)))
                {
                    await writer.WriteAsync(heroJson);
                }
            }

            // 4. Add story JSONs
            foreach (var (storyId, story) in manifest.Stories)
            {
                var storyJson = JsonSerializer.Serialize(story, new JsonSerializerOptions { WriteIndented = true });
                var storyEntry = zip.CreateEntry($"manifest/stories/{storyId}/story.json", CompressionLevel.Fastest);
                await using (var writer = new StreamWriter(storyEntry.Open(), new UTF8Encoding(false)))
                {
                    await writer.WriteAsync(storyJson);
                }
            }

            // 5. Download and add all media assets
            var assetMap = new Dictionary<string, string>();

            // Epic cover image
            if (!string.IsNullOrEmpty(manifest.EpicData.CoverImageUrl))
            {
                var zipPath = $"media/epic/cover{Path.GetExtension(manifest.EpicData.CoverImageUrl)}";
                if (await AddAssetToZip(zip, manifest.EpicData.CoverImageUrl, zipPath, isDraft, ct))
                {
                    assetMap[manifest.EpicData.CoverImageUrl] = zipPath;
                }
            }

            // Region images
            foreach (var region in manifest.Regions.Values)
            {
                if (!string.IsNullOrEmpty(region.ImageUrl))
                {
                    var zipPath = $"media/regions/{region.Id}/image{Path.GetExtension(region.ImageUrl)}";
                    if (await AddAssetToZip(zip, region.ImageUrl, zipPath, isDraft, ct))
                    {
                        assetMap[region.ImageUrl] = zipPath;
                    }
                }
            }

            // Hero assets
            foreach (var hero in manifest.Heroes.Values)
            {
                if (!string.IsNullOrEmpty(hero.ImageUrl))
                {
                    var zipPath = $"media/heroes/{hero.Id}/image{Path.GetExtension(hero.ImageUrl)}";
                    if (await AddAssetToZip(zip, hero.ImageUrl, zipPath, isDraft, ct))
                    {
                        assetMap[hero.ImageUrl] = zipPath;
                    }
                }

                // Hero greeting audio for each language
                foreach (var trans in hero.Translations)
                {
                    if (!string.IsNullOrEmpty(trans.GreetingAudioUrl))
                    {
                        var zipPath = $"media/heroes/{hero.Id}/audio/{trans.LanguageCode}/greeting{Path.GetExtension(trans.GreetingAudioUrl)}";
                        if (await AddAssetToZip(zip, trans.GreetingAudioUrl, zipPath, isDraft, ct))
                        {
                            assetMap[trans.GreetingAudioUrl] = zipPath;
                        }
                    }
                }
            }

            // Story assets (cover images and tile assets)
            foreach (var story in manifest.Stories.Values)
            {
                // Cover image
                if (!string.IsNullOrEmpty(story.CoverImageUrl))
                {
                    var zipPath = $"media/stories/{story.StoryId}/cover{Path.GetExtension(story.CoverImageUrl)}";
                    if (await AddAssetToZip(zip, story.CoverImageUrl, zipPath, isDraft, ct))
                    {
                        assetMap[story.CoverImageUrl] = zipPath;
                    }
                }

                // Tile assets
                foreach (var tile in story.Tiles)
                {
                    // Tile images, audio, video
                    if (!string.IsNullOrEmpty(tile.ImageUrl))
                    {
                        var zipPath = $"media/stories/{story.StoryId}/tiles/images/{Path.GetFileName(tile.ImageUrl)}";
                        if (await AddAssetToZip(zip, tile.ImageUrl, zipPath, isDraft, ct))
                        {
                            assetMap[tile.ImageUrl] = zipPath;
                        }
                    }

                    if (!string.IsNullOrEmpty(tile.AudioUrl))
                    {
                        var zipPath = $"media/stories/{story.StoryId}/tiles/audio/{Path.GetFileName(tile.AudioUrl)}";
                        if (await AddAssetToZip(zip, tile.AudioUrl, zipPath, isDraft, ct))
                        {
                            assetMap[tile.AudioUrl] = zipPath;
                        }
                    }

                    if (!string.IsNullOrEmpty(tile.VideoUrl))
                    {
                        var zipPath = $"media/stories/{story.StoryId}/tiles/video/{Path.GetFileName(tile.VideoUrl)}";
                        if (await AddAssetToZip(zip, tile.VideoUrl, zipPath, isDraft, ct))
                        {
                            assetMap[tile.VideoUrl] = zipPath;
                        }
                    }
                }
            }

            // Update manifest with asset map
            manifest.AssetMap = assetMap;

            // Re-write manifest with updated asset map
            manifestJson = JsonSerializer.Serialize(manifest, new JsonSerializerOptions { WriteIndented = true });
            var updatedManifestEntry = zip.GetEntry("manifest/epic.json");
            if (updatedManifestEntry != null)
            {
                updatedManifestEntry.Delete();
            }
            manifestEntry = zip.CreateEntry("manifest/epic.json", CompressionLevel.Fastest);
            await using (var writer = new StreamWriter(manifestEntry.Open(), new UTF8Encoding(false)))
            {
                await writer.WriteAsync(manifestJson);
            }
        }

        ms.Position = 0;
        return ms;
    }

    public async Task<EpicExportUploadResult> UploadExportAsync(Stream zipStream, string epicId, string fileName, CancellationToken ct)
    {
        var blobPath = $"epic-exports/{epicId}/{fileName}";
        var blobClient = _sas.GetBlobClient(_sas.DraftContainer, blobPath);

        await blobClient.UploadAsync(zipStream, overwrite: true, ct);

        var downloadUrl = _sas.GenerateSasUrl(blobClient, TimeSpan.FromHours(24));

        return new EpicExportUploadResult
        {
            BlobPath = blobPath,
            DownloadUrl = downloadUrl,
            SizeBytes = zipStream.Length
        };
    }

    private async Task<bool> AddAssetToZip(ZipArchive zip, string assetUrl, string zipPath, bool isDraft, CancellationToken ct)
    {
        try
        {
            // Determine container based on draft status
            var container = isDraft ? _sas.DraftContainer : _sas.PublishedContainer;

            // Extract blob path from URL
            var blobPath = ExtractBlobPath(assetUrl);
            if (string.IsNullOrEmpty(blobPath))
            {
                _logger.LogWarning("Could not extract blob path from URL: {Url}", assetUrl);
                return false;
            }

            var client = _sas.GetBlobClient(container, blobPath);
            if (!await client.ExistsAsync(ct))
            {
                _logger.LogWarning("Asset not found in {Container} storage: {Path}", container, blobPath);
                return false;
            }

            var entry = zip.CreateEntry(zipPath, CompressionLevel.Fastest);
            await using var entryStream = entry.Open();
            var download = await client.DownloadStreamingAsync(cancellationToken: ct);
            await download.Value.Content.CopyToAsync(entryStream, ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to download asset: {Url}", assetUrl);
            return false;
        }
    }

    private string ExtractBlobPath(string url)
    {
        // Extract path from various URL formats
        if (url.StartsWith("http://") || url.StartsWith("https://"))
        {
            var uri = new Uri(url);
            var path = uri.AbsolutePath.TrimStart('/');

            // Remove container name if present
            var parts = path.Split('/', 2);
            if (parts.Length > 1)
            {
                return parts[1];
            }
        }

        // Already a blob path
        return url;
    }

    private async Task<StoryExportDto> BuildStoryExportDto(object story, bool isDraft, CancellationToken ct)
    {
        // Implementation will depend on whether it's a StoryCraft or StoryDefinition
        // This is a placeholder - you'll need to implement based on your story structure

        dynamic s = story;
        return new StoryExportDto
        {
            StoryId = s.StoryId,
            Title = s.Title ?? string.Empty,
            Summary = s.Summary ?? string.Empty,
            CoverImageUrl = s.CoverImageUrl ?? string.Empty,
            StoryTopic = s.StoryTopic,
            AuthorName = s.AuthorName,
            ClassicAuthorId = s.ClassicAuthorId,
            LanguageCode = "ro-ro", // Default, should be from translation
            StoryType = s.StoryType,
            SortOrder = s.SortOrder ?? 0,
            IsEvaluative = s.IsEvaluative,
            UnlockedStoryHeroes = new List<string>(), // Need to populate from relationships
            Tiles = new List<StoryTileExportDto>(), // Need to populate from tiles
            Translations = new List<StoryTranslationExportDto>() // Need to populate from translations
        };
    }
}