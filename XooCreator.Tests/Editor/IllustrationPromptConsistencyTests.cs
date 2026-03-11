using XooCreator.BA.Features.StoryEditor.Models;
using XooCreator.BA.Features.StoryEditor.Services;
using Xunit;

namespace XooCreator.Tests.Editor;

public class IllustrationPromptConsistencyTests
{
    [Fact]
    public void Build_RecurringCharacter_HasStableAnchorsIncludingUniqueMarker()
    {
        var builder = new IllustrationPromptBuilder();
        var bible = CreateBible();
        var scene1 = CreateScene("s1");
        var scene10 = CreateScene("s10");

        var p1 = builder.Build(bible, scene1);
        var p10 = builder.Build(bible, scene10);

        Assert.Contains("Mimi", p1.PromptText);
        Assert.Contains("Mimi", p10.PromptText);
        Assert.Contains("red collar", p1.PromptText);
        Assert.Contains("red collar", p10.PromptText);
        Assert.Contains("Keep recurring characters visually identical across pages 1-10", p1.PromptText);
    }

    [Fact]
    public void ValidateAndPatch_MissingAnchor_AddsPatchedAnchorsAndRules()
    {
        var validator = new StoryImagePromptConsistencyValidator();
        var bible = CreateBible();
        var scene = CreateScene("s1");
        var anchors = new List<string> { "Mimi white and orange small cat striped tail unique marker: red collar" };
        var initialPrompt = "Children's book illustration. Scene: Mimi runs.";

        var result = validator.ValidateAndPatch(initialPrompt, bible, scene, anchors);

        Assert.True(result.PatchedCount > 0);
        Assert.Contains("Missing character anchors (patched)", result.PromptText);
        Assert.Contains("IMMUTABLE RULES", result.PromptText);
    }

    private static StoryBible CreateBible()
    {
        return new StoryBible
        {
            Title = "Cats",
            Language = "en-US",
            AgeRange = "4-6",
            Tone = "warm",
            VisualStyle = "storybook",
            Setting = new StorySetting { Place = "town", Time = "day" },
            Characters =
            [
                new CharacterProfile
                {
                    Id = "cat1",
                    Name = "Mimi",
                    Role = "main",
                    Species = "cat",
                    Visual = new VisualProfile
                    {
                        PrimaryColor = "white and orange",
                        Size = "small",
                        Features = ["striped tail", "round eyes"],
                        Accessories = ["red collar"]
                    },
                    Personality = ["playful"]
                }
            ],
            Plot = new PlotOutline
            {
                Problem = "lost",
                Resolution = "found",
                Moral = "friendship"
            },
            SceneSkeleton = ["s1", "s2"]
        };
    }

    private static SceneDefinition CreateScene(string id)
    {
        return new SceneDefinition
        {
            SceneId = id,
            OrderIndex = 1,
            Title = "Scene",
            Summary = "Summary",
            CharactersPresent = ["cat1"],
            Environment = "street",
            Emotion = "happy",
            VisualFocus = "running",
            SourceText = "text"
        };
    }
}

