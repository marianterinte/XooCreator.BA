using Microsoft.Extensions.Logging;
using Moq;
using XooCreator.BA.Features.StoryEditor.Services;
using Xunit;

namespace XooCreator.Tests.Editor;

public class StoryBibleGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_VaguePrompt_AutoCompletesMissingCharacterAnchors()
    {
        var googleText = new Mock<IGoogleTextService>();
        var logger = new Mock<ILogger<StoryBibleGenerator>>();

        var weakBibleJson = """
            {
              "title":"Pisici prietene",
              "language":"ro-RO",
              "ageRange":"4-6",
              "tone":"amuzant",
              "visualStyle":"storybook",
              "setting":{"place":"oras","time":"zi"},
              "characters":[
                {
                  "id":"cat1",
                  "name":"Mimi",
                  "role":"main",
                  "species":"cat",
                  "visual":{"primaryColor":"","size":"","features":[],"accessories":[]},
                  "personality":["jucausa"]
                }
              ],
              "plot":{"problem":"se despart","resolution":"se reunesc","moral":"prietenia"},
              "sceneSkeleton":["scena 1"]
            }
            """;

        googleText
            .Setup(x => x.GenerateContentAsync(
                It.IsAny<string>(),
                It.IsAny<string>(),
                "key",
                It.IsAny<string?>(),
                "application/json",
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(weakBibleJson);

        var sut = new StoryBibleGenerator(googleText.Object, logger.Object);

        var result = await sut.GenerateAsync(
            "Doua pisici prietene se despart si vor sa se reuneasca",
            null,
            10,
            "ro-RO",
            "key",
            null,
            CancellationToken.None);

        Assert.NotEmpty(result.Characters);
        var cat = result.Characters[0];
        Assert.False(string.IsNullOrWhiteSpace(cat.Visual.PrimaryColor));
        Assert.False(string.IsNullOrWhiteSpace(cat.Visual.Size));
        Assert.True(cat.Visual.Features.Count >= 2);
        Assert.True(cat.Visual.Accessories.Count >= 1);
        Assert.Equal(10, result.SceneSkeleton.Count);
    }
}

