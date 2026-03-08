using Microsoft.Extensions.Logging;
using Moq;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Services;
using Xunit;

namespace XooCreator.Tests.Editor;

/// <summary>
/// Unit tests for StoryAssetCopyService.FilterByVersionProfile.
/// </summary>
public class StoryAssetCopyServiceFilterByVersionProfileTests
{
    private readonly StoryAssetCopyService _service;
    private readonly List<StoryAssetPathMapper.AssetInfo> _mixedAssets;

    public StoryAssetCopyServiceFilterByVersionProfileTests()
    {
        var logger = new Mock<ILogger<StoryAssetCopyService>>();
        var sasMock = new Mock<XooCreator.BA.Infrastructure.Services.Blob.IBlobSasService>();
        _service = new StoryAssetCopyService(sasMock.Object, logger.Object);

        _mixedAssets = new List<StoryAssetPathMapper.AssetInfo>
        {
            new("cover.png", StoryAssetPathMapper.AssetType.Image, null),
            new("p1.png", StoryAssetPathMapper.AssetType.Image, null),
            new("p1-ro.wav", StoryAssetPathMapper.AssetType.Audio, "ro-ro"),
            new("p1-en.wav", StoryAssetPathMapper.AssetType.Audio, "en-us"),
            new("p1-hu.wav", StoryAssetPathMapper.AssetType.Audio, "hu-hu"),
            new("p1-ro.mp4", StoryAssetPathMapper.AssetType.Video, "ro-ro"),
            new("p1-en.mp4", StoryAssetPathMapper.AssetType.Video, "en-us")
        };
    }

    [Fact]
    public void FilterByVersionProfile_AllNull_ReturnsAllAssets()
    {
        var result = _service.FilterByVersionProfile(
            _mixedAssets,
            copyImages: null,
            copyAudio: null,
            copyVideo: null,
            languageMode: null,
            selectedLanguagesJson: null);

        Assert.Equal(7, result.Count);
    }

    [Fact]
    public void FilterByVersionProfile_CopyImagesFalse_ExcludesImages()
    {
        var result = _service.FilterByVersionProfile(
            _mixedAssets,
            copyImages: false,
            copyAudio: true,
            copyVideo: true,
            languageMode: "all",
            selectedLanguagesJson: null);

        Assert.Equal(5, result.Count);
        Assert.DoesNotContain(result, a => a.Type == StoryAssetPathMapper.AssetType.Image);
    }

    [Fact]
    public void FilterByVersionProfile_CopyAudioFalse_ExcludesAudio()
    {
        var result = _service.FilterByVersionProfile(
            _mixedAssets,
            copyImages: true,
            copyAudio: false,
            copyVideo: true,
            languageMode: "all",
            selectedLanguagesJson: null);

        Assert.Equal(5, result.Count);
        Assert.DoesNotContain(result, a => a.Type == StoryAssetPathMapper.AssetType.Audio);
    }

    [Fact]
    public void FilterByVersionProfile_CopyVideoFalse_ExcludesVideo()
    {
        var result = _service.FilterByVersionProfile(
            _mixedAssets,
            copyImages: true,
            copyAudio: true,
            copyVideo: false,
            languageMode: "all",
            selectedLanguagesJson: null);

        Assert.Equal(5, result.Count);
        Assert.DoesNotContain(result, a => a.Type == StoryAssetPathMapper.AssetType.Video);
    }

    [Fact]
    public void FilterByVersionProfile_SelectedLanguages_FiltersByLanguage()
    {
        var selectedJson = """["ro-ro", "en-us"]""";
        var result = _service.FilterByVersionProfile(
            _mixedAssets,
            copyImages: true,
            copyAudio: true,
            copyVideo: true,
            languageMode: "selected",
            selectedLanguagesJson: selectedJson);

        // Images (lang null) are always included when copyImages=true
        Assert.Contains(result, a => a.Filename == "cover.png");
        Assert.Contains(result, a => a.Filename == "p1.png");
        // Audio/Video for ro-ro and en-us
        Assert.Contains(result, a => a.Filename == "p1-ro.wav" && a.Lang == "ro-ro");
        Assert.Contains(result, a => a.Filename == "p1-en.wav" && a.Lang == "en-us");
        Assert.Contains(result, a => a.Filename == "p1-ro.mp4" && a.Lang == "ro-ro");
        Assert.Contains(result, a => a.Filename == "p1-en.mp4" && a.Lang == "en-us");
        // hu-hu excluded
        Assert.DoesNotContain(result, a => a.Lang == "hu-hu");
        Assert.Equal(6, result.Count);
    }

    [Fact]
    public void FilterByVersionProfile_SelectedLanguagesAndImagesOnly_ReturnsImagesAndNoMedia()
    {
        var selectedJson = """["ro-ro"]""";
        var result = _service.FilterByVersionProfile(
            _mixedAssets,
            copyImages: true,
            copyAudio: false,
            copyVideo: false,
            languageMode: "selected",
            selectedLanguagesJson: selectedJson);

        Assert.Equal(2, result.Count); // cover.png, p1.png
        Assert.All(result, a => Assert.Equal(StoryAssetPathMapper.AssetType.Image, a.Type));
    }
}
