using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using Xunit;

namespace XooCreator.Tests.Editor;

/// <summary>
/// Tests for save-boundary and pre-publish validation (token overflow, duplicate IDs, invalid refs, regression).
/// </summary>
public class PublishJsonConstraintsTests
{
    [Fact]
    public void SavePayloadValidator_ValidPayload_ReturnsNoErrors()
    {
        var dto = new EditableStoryDto
        {
            Id = "story-1",
            Language = "ro-ro",
            Tiles = new List<EditableTileDto>
            {
                new() { Id = "p1", Type = "page" },
                new() { Id = "q1", Type = "quiz", Answers = new List<EditableAnswerDto> { new() { Id = "a" }, new() { Id = "b" } } }
            }
        };
        var errors = SavePayloadValidator.Validate(dto);
        Assert.Empty(errors);
    }

    [Fact]
    public void SavePayloadValidator_DuplicateTileIds_ReturnsError()
    {
        var dto = new EditableStoryDto
        {
            Id = "story-1",
            Language = "ro-ro",
            Tiles = new List<EditableTileDto>
            {
                new() { Id = "p1", Type = "page" },
                new() { Id = "p1", Type = "page" }
            }
        };
        var errors = SavePayloadValidator.Validate(dto);
        Assert.Single(errors);
        Assert.Contains("Duplicate tile ID", errors[0]);
    }

    [Fact]
    public void SavePayloadValidator_AnswerTokenTypeOverflow_ReturnsError()
    {
        var longType = new string('x', 65);
        var dto = new EditableStoryDto
        {
            Id = "story-1",
            Language = "ro-ro",
            Tiles = new List<EditableTileDto>
            {
                new()
                {
                    Id = "q1",
                    Type = "quiz",
                    Answers = new List<EditableAnswerDto>
                    {
                        new() { Id = "a", Tokens = new List<EditableTokenDto> { new() { Type = longType, Value = "v", Quantity = 1 } } }
                    }
                }
            }
        };
        var errors = SavePayloadValidator.Validate(dto);
        Assert.NotEmpty(errors);
        Assert.True(errors.Any(e => e.Contains("Type length") && e.Contains("64")), string.Join("; ", errors));
    }

    [Fact]
    public void SavePayloadValidator_AnswerTokenValueOverflow_ReturnsError()
    {
        var longValue = new string('v', 129);
        var dto = new EditableStoryDto
        {
            Id = "story-1",
            Language = "ro-ro",
            Tiles = new List<EditableTileDto>
            {
                new()
                {
                    Id = "q1",
                    Type = "quiz",
                    Answers = new List<EditableAnswerDto>
                    {
                        new() { Id = "a", Tokens = new List<EditableTokenDto> { new() { Type = "Personality", Value = longValue, Quantity = 1 } } }
                    }
                }
            }
        };
        var errors = SavePayloadValidator.Validate(dto);
        Assert.NotEmpty(errors);
        Assert.True(errors.Any(e => e.Contains("Value length") && e.Contains("128")), string.Join("; ", errors));
    }

    [Fact]
    public void SavePayloadValidator_DuplicateAnswerIdsInTile_ReturnsError()
    {
        var dto = new EditableStoryDto
        {
            Id = "story-1",
            Language = "ro-ro",
            Tiles = new List<EditableTileDto>
            {
                new()
                {
                    Id = "q1",
                    Type = "quiz",
                    Answers = new List<EditableAnswerDto> { new() { Id = "a" }, new() { Id = "a" } }
                }
            }
        };
        var errors = SavePayloadValidator.Validate(dto);
        Assert.Single(errors);
        Assert.Contains("duplicate answer ID", errors[0]);
    }

    [Fact]
    public void SavePayloadValidator_DialogDuplicateNodeIds_ReturnsError()
    {
        var dto = new EditableStoryDto
        {
            Id = "story-1",
            Language = "ro-ro",
            Tiles = new List<EditableTileDto>
            {
                new()
                {
                    Id = "d1",
                    Type = "dialog",
                    DialogNodes = new List<EditableDialogNodeDto>
                    {
                        new() { NodeId = "n1", Text = "A" },
                        new() { NodeId = "n1", Text = "B" }
                    }
                }
            }
        };
        var errors = SavePayloadValidator.Validate(dto);
        Assert.Single(errors);
        Assert.Contains("duplicate dialog node ID", errors[0]);
    }

    [Fact]
    public void StoryPublishCraftValidator_ValidCraft_ReturnsValid()
    {
        var craft = CreateMinimalValidCraft();
        var validator = new StoryPublishCraftValidator();
        var result = validator.Validate(craft);
        Assert.True(result.IsValid);
    }

    [Fact]
    public void StoryPublishCraftValidator_AnswerTokenTypeOverflow_ReturnsFailure()
    {
        var craft = CreateMinimalValidCraft();
        var tile = craft.Tiles[0];
        var answerId = Guid.NewGuid();
        tile.Answers.Add(new StoryCraftAnswer
        {
            Id = answerId,
            StoryCraftTileId = tile.Id,
            AnswerId = "b",
            SortOrder = 1,
            Tokens = new List<StoryCraftAnswerToken>
            {
                new() { Id = Guid.NewGuid(), StoryCraftAnswerId = answerId, Type = new string('x', 65), Value = "v", Quantity = 1 }
            }
        });
        var validator = new StoryPublishCraftValidator();
        var result = validator.Validate(craft);
        Assert.False(result.IsValid);
        Assert.True(result.Failures.Any(f => f.ConstraintType == "MaxLength" && f.Message.Contains("Type")), result.ToDiagnosticMessage());
    }

    [Fact]
    public void StoryPublishCraftValidator_DuplicateTileIds_ReturnsFailure()
    {
        var craft = CreateMinimalValidCraft();
        craft.Tiles.Add(new StoryCraftTile { Id = Guid.NewGuid(), StoryCraftId = craft.Id, TileId = "p1", Type = "page", SortOrder = 1 });
        var validator = new StoryPublishCraftValidator();
        var result = validator.Validate(craft);
        Assert.False(result.IsValid);
        Assert.True(result.Failures.Any(f => f.Entity == "Tile" && f.ConstraintType == "UniqueViolation"), result.ToDiagnosticMessage());
    }

    [Fact]
    public void StoryPublishCraftValidator_HeroNotInDialogParticipants_ReturnsFailure()
    {
        var craft = CreateMinimalValidCraft();
        var dialogTileId = Guid.NewGuid();
        var dialogTile = new StoryCraftDialogTile
        {
            Id = dialogTileId,
            StoryCraftId = craft.Id,
            StoryCraftTileId = craft.Tiles[0].Id,
            Nodes = new List<StoryCraftDialogNode>
            {
                new()
                {
                    Id = Guid.NewGuid(),
                    StoryCraftDialogTileId = dialogTileId,
                    NodeId = "n1",
                    SpeakerType = "hero",
                    SpeakerHeroId = "unknown-hero"
                }
            }
        };
        craft.Tiles[0].Type = "dialog";
        craft.Tiles[0].DialogTile = dialogTile;
        craft.DialogParticipants = new List<StoryCraftDialogParticipant> { new() { HeroId = "other-hero", StoryCraftId = craft.Id } };
        var validator = new StoryPublishCraftValidator();
        var result = validator.Validate(craft);
        Assert.False(result.IsValid);
        Assert.True(result.Failures.Any(f => f.ConstraintType == "InvalidReference" && f.Message.Contains("DialogParticipants")), result.ToDiagnosticMessage());
    }

    private static StoryCraft CreateMinimalValidCraft()
    {
        var craftId = Guid.NewGuid();
        var tileId = Guid.NewGuid();
        var craft = new StoryCraft
        {
            Id = craftId,
            StoryId = "test-story",
            OwnerUserId = Guid.NewGuid(),
            Status = "draft",
            Tiles = new List<StoryCraftTile>
            {
                new()
                {
                    Id = tileId,
                    StoryCraftId = craftId,
                    TileId = "p1",
                    Type = "page",
                    SortOrder = 0,
                    Answers = new List<StoryCraftAnswer>(),
                    Translations = new List<StoryCraftTileTranslation>()
                }
            },
            DialogParticipants = new List<StoryCraftDialogParticipant>()
        };
        return craft;
    }
}
