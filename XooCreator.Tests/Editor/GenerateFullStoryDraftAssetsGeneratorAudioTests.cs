using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using XooCreator.BA.Infrastructure.Services.Blob;
using Xunit;

namespace XooCreator.Tests.Editor;

public class GenerateFullStoryDraftAssetsGeneratorAudioTests
{
    private static GenerateFullStoryDraftAssetsGenerator CreateGenerator(
        out Mock<IDiacriticsNormalizer> diacriticsMock,
        out Mock<IGoogleAudioGeneratorService> googleAudioMock)
    {
        var blobSas = new Mock<IBlobSasService>();
        var googleImage = new Mock<IGoogleImageService>();
        var openAiImage = new Mock<IOpenAIImageWithApiKey>();
        var sequential = new Mock<ISequentialStoryImageGenerator>();
        googleAudioMock = new Mock<IGoogleAudioGeneratorService>();
        var openAiAudio = new Mock<IOpenAIAudioWithApiKey>();
        diacriticsMock = new Mock<IDiacriticsNormalizer>();
        var promptValidator = new Mock<IStoryImagePromptConsistencyValidator>();
        var characterPresenceResolver = new Mock<ICharacterPresenceResolver>();
        var logger = new Mock<ILogger<GenerateFullStoryDraftAssetsGenerator>>();

        // Blob client stub
        var blobClient = new Mock<Azure.Storage.Blobs.BlobClient>();
        blobSas.Setup(x => x.GetBlobClient(It.IsAny<string>(), It.IsAny<string>()))
            .Returns(blobClient.Object);

        // Audio stub
        googleAudioMock.Setup(x => x.GenerateAudioAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync((new byte[] { 1, 2, 3 }, "wav"));

        return new GenerateFullStoryDraftAssetsGenerator(
            blobSas.Object,
            googleImage.Object,
            openAiImage.Object,
            sequential.Object,
            googleAudioMock.Object,
            openAiAudio.Object,
            diacriticsMock.Object,
            promptValidator.Object,
            characterPresenceResolver.Object,
            logger.Object);
    }

    [Fact]
    public async Task FillImagesAndAudioAsync_UsesNormalizedTextForAudio_WhenAvailable()
    {
        var generator = CreateGenerator(out var diacriticsMock, out var googleAudioMock);

        diacriticsMock
            .Setup(x => x.NormalizeAsync("Original text", "ro-RO", "key", null, It.IsAny<CancellationToken>()))
            .ReturnsAsync("Text cu diacritice");

        var request = new GenerateFullStoryDraftRequest
        {
            ApiKey = "key",
            Provider = "Google",
            TextSeed = "seed long enough",
            NumberOfPages = 1,
            LanguageCode = "ro-RO",
            GenerateImages = false,
            GenerateAudio = true,
            AudioModel = "tts-model"
        };

        var dto = new EditableStoryDto
        {
            Id = "story-1",
            Title = "t",
            Summary = "s",
            Language = "ro-RO",
            StoryType = 1,
            Tiles = new List<EditableTileDto>
            {
                new()
                {
                    Id = "p1",
                    Type = "page",
                    Text = "Original text",
                    Caption = string.Empty,
                    ImageUrl = string.Empty,
                    AudioUrl = string.Empty
                }
            }
        };

        await generator.FillImagesAndAudioAsync(
            request,
            dto,
            "story-1",
            "owner@example.com",
            isOpenAi: false,
            usePublishedPaths: false,
            CancellationToken.None);

        googleAudioMock.Verify(x => x.GenerateAudioAsync(
                "Text cu diacritice",
                "ro-RO",
                null,
                null,
                "key",
                "tts-model",
                It.IsAny<CancellationToken>()),
            Times.Once);
    }
}

