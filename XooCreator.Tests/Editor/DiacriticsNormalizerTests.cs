using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Services;
using Xunit;

namespace XooCreator.Tests.Editor;

public class DiacriticsNormalizerTests
{
    [Fact]
    public async Task NormalizeAsync_WhenLanguageUnsupported_ReturnsOriginalText()
    {
        var googleText = new Mock<IGoogleTextService>();
        var logger = new Mock<ILogger<DiacriticsNormalizer>>();
        var service = new DiacriticsNormalizer(googleText.Object, logger.Object);

        const string text = "Some english text without diacritics.";

        var result = await service.NormalizeAsync(text, "ja-JP", "key", null, CancellationToken.None);

        Assert.Equal(text, result);
        googleText.Verify(
            x => x.GenerateContentAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task NormalizeAsync_WhenGoogleThrows_ReturnsOriginalText()
    {
        var googleText = new Mock<IGoogleTextService>();
        googleText.Setup(x => x.GenerateContentAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var logger = new Mock<ILogger<DiacriticsNormalizer>>();
        var service = new DiacriticsNormalizer(googleText.Object, logger.Object);

        const string text = "Un text fara diacritice suficient de lung.";

        var result = await service.NormalizeAsync(text, "ro-RO", "key", null, CancellationToken.None);

        Assert.Equal(text, result);
    }

    [Fact]
    public async Task NormalizeAsync_WhenGoogleReturnsText_UsesNormalized()
    {
        var googleText = new Mock<IGoogleTextService>();
        googleText.Setup(x => x.GenerateContentAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("Un text cu diacritice.");

        var logger = new Mock<ILogger<DiacriticsNormalizer>>();
        var service = new DiacriticsNormalizer(googleText.Object, logger.Object);

        const string text = "Un text fara diacritice suficient de lung.";

        var result = await service.NormalizeAsync(text, "ro-RO", "key", null, CancellationToken.None);

        Assert.Equal("Un text cu diacritice.", result);
    }
}

