using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using Xunit;

namespace XooCreator.Tests.Editor;

/// <summary>
/// Regression tests for Story Bible pipeline: provider gating and graceful fallback.
/// </summary>
public class StoryBiblePipelineTests
{
    private static GenerateFullStoryDraftHandler CreateHandler(
        out Mock<IStoryBibleGenerator> bibleMock,
        out Mock<IStoryValidator> validatorMock,
        out Mock<IStoryRepairService> repairMock,
        out Mock<IScenePlanner> scenePlannerMock,
        out Mock<IGenerateFullStoryDraftAssetsGenerator> assetsMock)
    {
        var storyIdGen = new Mock<IStoryIdGenerator>();
        storyIdGen.Setup(x => x.GenerateNextAsync(It.IsAny<Guid>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync("story-1");

        var editorService = new Mock<IStoryEditorService>();
        var googleText = new Mock<IGoogleTextService>();
        var openAiText = new Mock<IOpenAITextWithApiKey>();

        assetsMock = new Mock<IGenerateFullStoryDraftAssetsGenerator>();

        bibleMock = new Mock<IStoryBibleGenerator>();
        validatorMock = new Mock<IStoryValidator>();
        repairMock = new Mock<IStoryRepairService>();
        scenePlannerMock = new Mock<IScenePlanner>();
        var promptBuilder = new Mock<IIllustrationPromptBuilder>();
        var logger = new Mock<ILogger<GenerateFullStoryDraftHandler>>();

        // Minimal google text implementation so that story text generation succeeds.
        googleText.Setup(x => x.GenerateContentAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync("###T\nTitle\n###S\nSummary\n###P1\nPage 1\n");

        return new GenerateFullStoryDraftHandler(
            storyIdGen.Object,
            editorService.Object,
            googleText.Object,
            openAiText.Object,
            assetsMock.Object,
            bibleMock.Object,
            validatorMock.Object,
            repairMock.Object,
            scenePlannerMock.Object,
            promptBuilder.Object,
            logger.Object);
    }

    [Fact]
    public async Task ExecuteAsync_OpenAiProvider_DoesNotCallStoryBibleGenerator()
    {
        // Arrange
        var handler = CreateHandler(
            out var bibleMock,
            out _,
            out _,
            out _,
            out _);

        var request = new GenerateFullStoryDraftRequest
        {
            ApiKey = "test-key",
            Provider = "OpenAI",
            TextSeed = "Some seed text that is long enough",
            NumberOfPages = 5,
            LanguageCode = "en-us",
            UseStoryBible = true
        };

        // Act
        await handler.ExecuteAsync(request, Guid.NewGuid(), "A", "B", "test@example.com", CancellationToken.None);

        // Assert
        bibleMock.Verify(
            x => x.GenerateAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task ExecuteAsync_StoryBibleThrows_FallsBackToClassicFlow()
    {
        // Arrange
        var handler = CreateHandler(
            out var bibleMock,
            out _,
            out _,
            out _,
            out var assetsMock);

        bibleMock
            .Setup(x => x.GenerateAsync(It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<int>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("boom"));

        var request = new GenerateFullStoryDraftRequest
        {
            ApiKey = "test-key",
            Provider = "Google",
            TextSeed = "Some seed text that is long enough",
            NumberOfPages = 5,
            LanguageCode = "en-us",
            UseStoryBible = true,
            GenerateImages = true,
            GenerateAudio = true
        };

        // Act
        await handler.ExecuteAsync(request, Guid.NewGuid(), "A", "B", "test@example.com", CancellationToken.None);

        // Assert: assets generator is still called, but StoryBible is null (classic flow).
        assetsMock.Verify(x => x.FillImagesAndAudioAsync(
                It.IsAny<GenerateFullStoryDraftRequest>(),
                It.IsAny<EditableStoryDto>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<bool>(),
                It.IsAny<bool>(),
                It.IsAny<CancellationToken>(),
                null,
                null,
                It.IsAny<IIllustrationPromptBuilder>()),
            Times.Once);
    }
}

