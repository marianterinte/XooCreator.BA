using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services;
using Xunit;

namespace XooCreator.Tests.Editor;

/// <summary>
/// Unit tests for publish language merge (VersionCopyLanguageMode=selected).
/// </summary>
public class StoryPublishLanguageMergeTests
{
    [Fact]
    public void TryParseSelectedLanguagesJson_ValidSingleLanguage_ReturnsHashSet()
    {
        var result = StoryPublishLanguageMergeHelper.TryParseSelectedLanguagesJson("""["en-us"]""");
        Assert.NotNull(result);
        Assert.Single(result);
        Assert.Contains("en-us", result!);
    }

    [Fact]
    public void TryParseSelectedLanguagesJson_ValidMultipleLanguages_ReturnsHashSet()
    {
        var result = StoryPublishLanguageMergeHelper.TryParseSelectedLanguagesJson("""["en-us", "ro-ro"]""");
        Assert.NotNull(result);
        Assert.Equal(2, result!.Count);
        Assert.Contains("en-us", result);
        Assert.Contains("ro-ro", result);
    }

    [Fact]
    public void TryParseSelectedLanguagesJson_NormalizesToLower()
    {
        var result = StoryPublishLanguageMergeHelper.TryParseSelectedLanguagesJson("""["EN-US", "Ro-Ro"]""");
        Assert.NotNull(result);
        Assert.Contains("en-us", result!);
        Assert.Contains("ro-ro", result);
    }

    [Fact]
    public void TryParseSelectedLanguagesJson_EmptyArray_ReturnsNull()
    {
        var result = StoryPublishLanguageMergeHelper.TryParseSelectedLanguagesJson("""[]""");
        Assert.Null(result);
    }

    [Fact]
    public void TryParseSelectedLanguagesJson_NullOrWhitespace_ReturnsNull()
    {
        Assert.Null(StoryPublishLanguageMergeHelper.TryParseSelectedLanguagesJson(""));
        Assert.Null(StoryPublishLanguageMergeHelper.TryParseSelectedLanguagesJson("   "));
        Assert.Null(StoryPublishLanguageMergeHelper.TryParseSelectedLanguagesJson(null!));
    }

    [Fact]
    public void TryParseSelectedLanguagesJson_InvalidJson_ReturnsNull()
    {
        var result = StoryPublishLanguageMergeHelper.TryParseSelectedLanguagesJson("not valid json");
        Assert.Null(result);
    }

    [Fact]
    public void GetSelectedLanguagesFromCraft_WithValidJson_ReturnsHashSet()
    {
        var craft = new StoryCraft
        {
            StoryId = "test-story",
            Status = "draft",
            VersionCopySelectedLanguagesJson = """["en-us"]"""
        };
        var result = StoryPublishLanguageMergeHelper.GetSelectedLanguagesFromCraft(craft);
        Assert.NotNull(result);
        Assert.Single(result!);
        Assert.Contains("en-us", result);
    }

    [Fact]
    public void GetSelectedLanguagesFromCraft_NullJson_ReturnsNull()
    {
        var craft = new StoryCraft
        {
            StoryId = "test-story",
            Status = "draft",
            VersionCopySelectedLanguagesJson = null
        };
        Assert.Null(StoryPublishLanguageMergeHelper.GetSelectedLanguagesFromCraft(craft));
    }

    [Fact]
    public void ShouldUseLanguageMerge_NewDefinition_ReturnsFalse()
    {
        var craft = new StoryCraft
        {
            StoryId = "test-story",
            Status = "draft",
            VersionCopyLanguageMode = "selected",
            VersionCopySelectedLanguagesJson = """["en-us"]"""
        };
        Assert.False(StoryPublishLanguageMergeHelper.ShouldUseLanguageMerge(craft, isNewDefinition: true));
    }

    [Fact]
    public void ShouldUseLanguageMerge_ExistingDefinition_SelectedMode_ReturnsTrue()
    {
        var craft = new StoryCraft
        {
            StoryId = "test-story",
            Status = "draft",
            VersionCopyLanguageMode = "selected",
            VersionCopySelectedLanguagesJson = """["en-us"]"""
        };
        Assert.True(StoryPublishLanguageMergeHelper.ShouldUseLanguageMerge(craft, isNewDefinition: false));
    }

    [Fact]
    public void ShouldUseLanguageMerge_AllMode_ReturnsFalse()
    {
        var craft = new StoryCraft
        {
            StoryId = "test-story",
            Status = "draft",
            VersionCopyLanguageMode = "all",
            VersionCopySelectedLanguagesJson = """["en-us"]"""
        };
        Assert.False(StoryPublishLanguageMergeHelper.ShouldUseLanguageMerge(craft, isNewDefinition: false));
    }

    [Fact]
    public void ShouldUseLanguageMerge_SelectedMode_EmptyLanguages_ReturnsFalse()
    {
        var craft = new StoryCraft
        {
            StoryId = "test-story",
            Status = "draft",
            VersionCopyLanguageMode = "selected",
            VersionCopySelectedLanguagesJson = """[]"""
        };
        Assert.False(StoryPublishLanguageMergeHelper.ShouldUseLanguageMerge(craft, isNewDefinition: false));
    }
}
