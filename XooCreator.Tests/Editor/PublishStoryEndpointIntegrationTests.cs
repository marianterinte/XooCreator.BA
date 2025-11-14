using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Services;
using Xunit;
using Moq;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Infrastructure.Services.Blob;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;

namespace XooCreator.Tests.Editor;

/// <summary>
/// Integration tests for PublishStoryEndpoint to verify multi-language asset collection and copying.
/// </summary>
public class PublishStoryEndpointIntegrationTests
{
    [Fact]
    public void CollectAllAssets_ShouldCollectAudioForAllLanguages()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<StoryPublishAssetService>>();
        var sasMock = new Mock<IBlobSasService>();
        var service = new StoryPublishAssetService(sasMock.Object, loggerMock.Object);

        var craft = CreateTestCraftWithMultipleLanguages();

        // Act
        var assets = service.CollectAllAssets(craft);

        // Assert
        // Should have 1 cover image
        var images = assets.Where(a => a.Type == StoryAssetPathMapper.AssetType.Image).ToList();
        Assert.Single(images);
        Assert.Equal("cover.png", images[0].Filename);
        Assert.Null(images[0].Lang); // Images are language-agnostic

        // Should have audio for both languages (p1 has audio for both, p2 has audio only for ro-ro)
        var audioAssets = assets.Where(a => a.Type == StoryAssetPathMapper.AssetType.Audio).ToList();
        Assert.Equal(3, audioAssets.Count); // hu-audio.wav, ro-audio.wav, 4.cave.wav

        // Verify Hungarian audio from first tile
        var huAudio = audioAssets.FirstOrDefault(a => a.Lang == "hu-hu" && a.Filename == "hu-audio.wav");
        Assert.NotNull(huAudio);
        Assert.Equal("hu-hu", huAudio.Lang);

        // Verify Romanian audio from first tile
        var roAudio1 = audioAssets.FirstOrDefault(a => a.Lang == "ro-ro" && a.Filename == "ro-audio.wav");
        Assert.NotNull(roAudio1);
        Assert.Equal("ro-ro", roAudio1.Lang);

        // Verify Romanian audio from second tile (4.cave.wav)
        var roAudio2 = audioAssets.FirstOrDefault(a => a.Lang == "ro-ro" && a.Filename == "4.cave.wav");
        Assert.NotNull(roAudio2);
        Assert.Equal("ro-ro", roAudio2.Lang);
        Assert.Equal("4.cave.wav", roAudio2.Filename);
    }

    [Fact]
    public void CollectAllAssets_ShouldCollectVideoForAllLanguages()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<StoryPublishAssetService>>();
        var sasMock = new Mock<IBlobSasService>();
        var service = new StoryPublishAssetService(sasMock.Object, loggerMock.Object);

        var craft = CreateTestCraftWithMultipleLanguages(includeVideo: true);

        // Act
        var assets = service.CollectAllAssets(craft);

        // Assert
        // Should have video for both languages
        var videoAssets = assets.Where(a => a.Type == StoryAssetPathMapper.AssetType.Video).ToList();
        Assert.Equal(2, videoAssets.Count);

        // Verify Hungarian video
        var huVideo = videoAssets.FirstOrDefault(a => a.Lang == "hu-hu");
        Assert.NotNull(huVideo);
        Assert.Equal("hu-video.mp4", huVideo.Filename);
        Assert.Equal("hu-hu", huVideo.Lang);

        // Verify Romanian video
        var roVideo = videoAssets.FirstOrDefault(a => a.Lang == "ro-ro");
        Assert.NotNull(roVideo);
        Assert.Equal("ro-video.mp4", roVideo.Filename);
        Assert.Equal("ro-ro", roVideo.Lang);
    }

    [Fact]
    public void CollectAllAssets_ShouldDeduplicateImages()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<StoryPublishAssetService>>();
        var sasMock = new Mock<IBlobSasService>();
        var service = new StoryPublishAssetService(sasMock.Object, loggerMock.Object);

        var craft = CreateTestCraftWithMultipleLanguages();
        // Add same image to multiple tiles
        craft.Tiles[0].ImageUrl = "tile1.png";
        craft.Tiles[1].ImageUrl = "tile1.png"; // Same image

        // Act
        var assets = service.CollectAllAssets(craft);

        // Assert
        // Should have cover + 1 tile image (deduplicated)
        var images = assets.Where(a => a.Type == StoryAssetPathMapper.AssetType.Image).ToList();
        Assert.Equal(2, images.Count); // cover.png + tile1.png (deduplicated)
    }

    [Fact]
    public void CollectAllAssets_ShouldHandleMissingTranslations()
    {
        // Arrange
        var loggerMock = new Mock<ILogger<StoryPublishAssetService>>();
        var sasMock = new Mock<IBlobSasService>();
        var service = new StoryPublishAssetService(sasMock.Object, loggerMock.Object);

        var craft = CreateTestCraftWithMultipleLanguages();
        // Remove translations from one tile
        craft.Tiles[1].Translations.Clear();

        // Act
        var assets = service.CollectAllAssets(craft);

        // Assert
        // Should still collect assets from tiles that have translations
        var audioAssets = assets.Where(a => a.Type == StoryAssetPathMapper.AssetType.Audio).ToList();
        Assert.Single(audioAssets); // Only from first tile
    }

    // TODO: Add integration test for CopyAssetsToPublishedAsync to verify that assets are copied
    // to the correct language-specific paths in blob storage. This would require:
    // - Mocking IBlobSasService and BlobClient
    // - Verifying that BuildPublishedPath is called with correct language tags
    // - Verifying that all assets (for all languages) are processed

    /// <summary>
    /// Creates a test StoryCraft with multiple languages (hu-hu and ro-ro) and assets.
    /// </summary>
    private StoryCraft CreateTestCraftWithMultipleLanguages(bool includeVideo = false)
    {
        var craft = new StoryCraft
        {
            Id = Guid.NewGuid(),
            StoryId = "test-story-1",
            OwnerUserId = Guid.NewGuid(),
            Status = StoryStatus.Approved.ToDb(),
            StoryType = StoryType.Indie,
            CoverImageUrl = "cover.png",
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            Translations = new List<StoryCraftTranslation>
            {
                new StoryCraftTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftId = Guid.NewGuid(),
                    LanguageCode = "hu-hu",
                    Title = "Hungarian Title",
                    Summary = "Hungarian Summary"
                },
                new StoryCraftTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftId = Guid.NewGuid(),
                    LanguageCode = "ro-ro",
                    Title = "Romanian Title",
                    Summary = "Romanian Summary"
                }
            },
            Tiles = new List<StoryCraftTile>
            {
                new StoryCraftTile
                {
                    Id = Guid.NewGuid(),
                    StoryCraftId = Guid.NewGuid(),
                    TileId = "p1",
                    Type = "page",
                    SortOrder = 1,
                    ImageUrl = "tile1.png",
                    Translations = new List<StoryCraftTileTranslation>
                    {
                        new StoryCraftTileTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftTileId = Guid.NewGuid(),
                            LanguageCode = "hu-hu",
                            Caption = "Hungarian Caption",
                            Text = "Hungarian Text",
                            AudioUrl = "hu-audio.wav",
                            VideoUrl = includeVideo ? "hu-video.mp4" : null
                        },
                        new StoryCraftTileTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftTileId = Guid.NewGuid(),
                            LanguageCode = "ro-ro",
                            Caption = "Romanian Caption",
                            Text = "Romanian Text",
                            AudioUrl = "ro-audio.wav",
                            VideoUrl = includeVideo ? "ro-video.mp4" : null
                        }
                    }
                },
                new StoryCraftTile
                {
                    Id = Guid.NewGuid(),
                    StoryCraftId = Guid.NewGuid(),
                    TileId = "p2",
                    Type = "page",
                    SortOrder = 2,
                    ImageUrl = "tile2.png",
                    Translations = new List<StoryCraftTileTranslation>
                    {
                        new StoryCraftTileTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftTileId = Guid.NewGuid(),
                            LanguageCode = "hu-hu",
                            Caption = "Hungarian Caption 2",
                            Text = "Hungarian Text 2",
                            AudioUrl = null, // No audio for this tile
                            VideoUrl = null
                        },
                        new StoryCraftTileTranslation
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftTileId = Guid.NewGuid(),
                            LanguageCode = "ro-ro",
                            Caption = "Romanian Caption 2",
                            Text = "Romanian Text 2",
                            AudioUrl = "4.cave.wav", // Romanian audio for second tile
                            VideoUrl = null
                        }
                    }
                }
            }
        };

        // Set StoryCraftId references
        foreach (var translation in craft.Translations)
        {
            translation.StoryCraftId = craft.Id;
        }

        foreach (var tile in craft.Tiles)
        {
            tile.StoryCraftId = craft.Id;
            foreach (var translation in tile.Translations)
            {
                translation.StoryCraftTileId = tile.Id;
            }
        }

        return craft;
    }
}

