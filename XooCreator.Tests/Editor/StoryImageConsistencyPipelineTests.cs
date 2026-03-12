using Microsoft.Extensions.Logging;
using Moq;
using XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;
using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using XooCreator.BA.Infrastructure.Services.Blob;
using Xunit;

namespace XooCreator.Tests.Editor;

public class StoryImageConsistencyPipelineTests
{
    [Fact]
    public async Task FillImagesAndAudioAsync_StoryBible_UsesFullContextAndSpeciesHintPatch()
    {
        var blobSas = new Mock<IBlobSasService>();
        var googleImage = new Mock<IGoogleImageService>();
        var openAiImage = new Mock<IOpenAIImageWithApiKey>();
        var sequential = new Mock<ISequentialStoryImageGenerator>();
        var googleAudio = new Mock<IGoogleAudioGeneratorService>();
        var openAiAudio = new Mock<IOpenAIAudioWithApiKey>();
        var diacritics = new Mock<IDiacriticsNormalizer>();
        var promptValidator = new Mock<IStoryImagePromptConsistencyValidator>();
        var characterPresenceResolver = new Mock<ICharacterPresenceResolver>();
        var logger = new Mock<ILogger<GenerateFullStoryDraftAssetsGenerator>>();

        SceneDefinition? capturedScene = null;
        var promptBuilder = new Mock<IIllustrationPromptBuilder>();
        promptBuilder
            .Setup(x => x.Build(It.IsAny<StoryBible>(), It.IsAny<SceneDefinition>()))
            .Callback<StoryBible, SceneDefinition>((_, s) => capturedScene = s)
            .Returns(new IllustrationPrompt
            {
                SceneId = "scene-01",
                PromptText = "base prompt",
                StyleNotes = "storybook",
                CharacterAnchors = ["Mimi orange cat", "Nori black cat"],
                NegativePrompt = "none"
            });
        promptBuilder
            .Setup(x => x.GetPromptText(It.IsAny<IllustrationPrompt>()))
            .Returns((IllustrationPrompt p) => p.PromptText);

        promptValidator
            .Setup(x => x.ValidateAndPatch(
                It.IsAny<string>(),
                It.IsAny<StoryBible>(),
                It.IsAny<SceneDefinition?>(),
                It.IsAny<IReadOnlyList<string>>()))
            .Returns((string p, StoryBible _, SceneDefinition? __, IReadOnlyList<string> ___) => (p, 0));

        string? capturedPrompt = null;
        googleImage
            .Setup(x => x.GenerateFromBuiltPromptAsync(
                It.IsAny<string>(),
                It.IsAny<byte[]?>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>(),
                It.IsAny<string?>(),
                It.IsAny<string?>()))
            .Callback<string, byte[]?, string?, CancellationToken, string?, string?>(
                (prompt, _, _, _, _, _) => capturedPrompt = prompt)
            .ThrowsAsync(new InvalidOperationException("forced for test"));

        characterPresenceResolver
            .Setup(x => x.ResolveAsync(
                It.IsAny<StoryBible>(),
                It.IsAny<SceneDefinition>(),
                It.IsAny<string>(),
                It.IsAny<string>(),
                It.IsAny<string?>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(["cat_black"]);

        var sut = new GenerateFullStoryDraftAssetsGenerator(
            blobSas.Object,
            googleImage.Object,
            openAiImage.Object,
            sequential.Object,
            googleAudio.Object,
            openAiAudio.Object,
            diacritics.Object,
            promptValidator.Object,
            characterPresenceResolver.Object,
            logger.Object);

        var request = new GenerateFullStoryDraftRequest
        {
            ApiKey = "key",
            Provider = "Google",
            LanguageCode = "ro-RO",
            TextSeed = "seed long enough",
            NumberOfPages = 2,
            GenerateImages = true,
            GenerateAudio = false
        };

        var dto = new EditableStoryDto
        {
            Id = "s1",
            Title = "Pisici",
            Summary = "Doua pisici",
            Language = "ro-RO",
            StoryType = 1,
            Tiles =
            [
                new EditableTileDto { Id = "p1", Type = "page", Text = "Doua pisici pleaca la stapani diferiti." },
                new EditableTileDto { Id = "p2", Type = "page", Text = "Pisicile fac farse ca sa fie impreuna." }
            ]
        };

        var bible = new StoryBible
        {
            Title = "Pisici",
            Language = "ro-RO",
            AgeRange = "4-6",
            Tone = "warm",
            VisualStyle = "storybook",
            Setting = new StorySetting { Place = "oras", Time = "zi" },
            Characters =
            [
                new CharacterProfile
                {
                    Id = "cat_orange",
                    Name = "Mimi",
                    Role = "main",
                    Species = "cat",
                    Visual = new VisualProfile
                    {
                        PrimaryColor = "orange",
                        Size = "small",
                        Features = ["round eyes", "striped tail"],
                        Accessories = ["red collar"]
                    },
                    Personality = ["playful"]
                },
                new CharacterProfile
                {
                    Id = "cat_black",
                    Name = "Nori",
                    Role = "supporting",
                    Species = "cat",
                    Visual = new VisualProfile
                    {
                        PrimaryColor = "black",
                        Size = "small",
                        Features = ["bright eyes", "sleek fur"],
                        Accessories = ["blue scarf"]
                    },
                    Personality = ["smart"]
                }
            ],
            Plot = new PlotOutline { Problem = "split", Resolution = "reunited", Moral = "friendship" },
            SceneSkeleton = ["s1", "s2"]
        };

        var scenePlan = new ScenePlan
        {
            StoryId = "s1",
            Scenes =
            [
                new SceneDefinition
                {
                    SceneId = "scene-01",
                    OrderIndex = 0,
                    Title = "Start",
                    Summary = "Start",
                    CharactersPresent = ["cat_orange"],
                    Environment = "street",
                    Emotion = "sad",
                    VisualFocus = "cat",
                    SourceText = dto.Tiles[0].Text ?? string.Empty
                },
                new SceneDefinition
                {
                    SceneId = "scene-02",
                    OrderIndex = 1,
                    Title = "Plan",
                    Summary = "Plan",
                    CharactersPresent = ["cat_orange"],
                    Environment = "house",
                    Emotion = "funny",
                    VisualFocus = "cats",
                    SourceText = dto.Tiles[1].Text ?? string.Empty
                }
            ]
        };

        await sut.FillImagesAndAudioAsync(
            request,
            dto,
            "s1",
            "owner@example.com",
            isOpenAi: false,
            usePublishedPaths: false,
            CancellationToken.None,
            bible,
            scenePlan,
            promptBuilder.Object);

        Assert.NotNull(capturedPrompt);
        Assert.Contains("CHARACTER ROSTER LOCK", capturedPrompt);
        Assert.NotNull(capturedScene);
        Assert.Contains("cat_black", capturedScene!.CharactersPresent);
    }
}

