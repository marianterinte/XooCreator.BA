using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryCopyService
{
    Task<StoryCraft> CreateCopyFromCraftAsync(
        StoryCraft sourceCraft,
        Guid ownerUserId,
        string newStoryId,
        CancellationToken ct,
        bool isCopy = false);

    Task<StoryCraft> CreateCopyFromDefinitionAsync(
        StoryDefinition definition,
        Guid ownerUserId,
        string newStoryId,
        CancellationToken ct,
        bool isCopy = false);
}

/// <summary>
/// Service responsible for cloning story structures for Copy/Fork/New Version flows.
/// </summary>
public class StoryCopyService : IStoryCopyService
{
    private readonly IStoryCraftsRepository _craftsRepository;
    private readonly ILogger<StoryCopyService> _logger;

    public StoryCopyService(
        IStoryCraftsRepository craftsRepository,
        ILogger<StoryCopyService> logger)
    {
        _craftsRepository = craftsRepository;
        _logger = logger;
    }

    public async Task<StoryCraft> CreateCopyFromCraftAsync(
        StoryCraft sourceCraft,
        Guid ownerUserId,
        string newStoryId,
        CancellationToken ct,
        bool isCopy = false)
    {
        ArgumentNullException.ThrowIfNull(sourceCraft);
        ValidateInputs(ownerUserId, newStoryId);

        var craft = CloneCraftFromCraft(sourceCraft, ownerUserId, newStoryId, isCopy);
        await _craftsRepository.SaveAsync(craft, ct);

        _logger.LogInformation(
            "Story copied from draft: sourceStoryId={SourceStoryId} -> newStoryId={NewStoryId} owner={OwnerId} isCopy={IsCopy}",
            sourceCraft.StoryId,
            newStoryId,
            ownerUserId,
            isCopy);

        return craft;
    }

    public async Task<StoryCraft> CreateCopyFromDefinitionAsync(
        StoryDefinition definition,
        Guid ownerUserId,
        string newStoryId,
        CancellationToken ct,
        bool isCopy = false)
    {
        ArgumentNullException.ThrowIfNull(definition);
        ValidateInputs(ownerUserId, newStoryId);

        var craft = CloneCraftFromDefinition(definition, ownerUserId, newStoryId, isCopy);
        await _craftsRepository.SaveAsync(craft, ct);

        _logger.LogInformation(
            "Story copied from published definition: sourceStoryId={SourceStoryId} -> newStoryId={NewStoryId} owner={OwnerId} isCopy={IsCopy}",
            definition.StoryId,
            newStoryId,
            ownerUserId,
            isCopy);

        return craft;
    }

    private static StoryCraft CloneCraftFromCraft(StoryCraft source, Guid ownerUserId, string newStoryId, bool isCopy = false)
    {
        var craft = CreateBaseCraft(ownerUserId, newStoryId, source.StoryType, source.StoryTopic, source.CoverImageUrl);
        craft.PriceInCredits = source.PriceInCredits;
        craft.BaseVersion = source.BaseVersion;

        CopyCraftTranslations(source, craft, isCopy);
        CopyCraftTiles(source, craft);
        CopyCraftTopics(source, craft);
        CopyCraftAgeGroups(source, craft);

        return craft;
    }

    private static StoryCraft CloneCraftFromDefinition(StoryDefinition definition, Guid ownerUserId, string newStoryId, bool isCopy = false)
    {
        var craft = CreateBaseCraft(ownerUserId, newStoryId, definition.StoryType, definition.StoryTopic, ExtractFileName(definition.CoverImageUrl));
        craft.PriceInCredits = definition.PriceInCredits;
        craft.BaseVersion = definition.Version;

        CopyDefinitionTranslations(definition, craft, isCopy);
        CopyDefinitionTiles(definition, craft);
        CopyDefinitionTopics(definition, craft);
        CopyDefinitionAgeGroups(definition, craft);

        return craft;
    }

    private static StoryCraft CreateBaseCraft(Guid ownerUserId, string newStoryId, StoryType storyType, string? storyTopic, string? coverImage)
    {
        return new StoryCraft
        {
            StoryId = newStoryId,
            OwnerUserId = ownerUserId,
            Status = StoryStatus.Draft.ToDb(),
            StoryType = storyType,
            StoryTopic = storyTopic,
            CoverImageUrl = coverImage,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
    }

    private static void CopyCraftTranslations(StoryCraft source, StoryCraft target, bool isCopy = false)
    {
        foreach (var translation in source.Translations)
        {
            var title = translation.Title;
            // For copy operations, prefix title with "Copy of "
            if (isCopy && !string.IsNullOrWhiteSpace(title) && !title.StartsWith("Copy of ", StringComparison.OrdinalIgnoreCase))
            {
                title = $"Copy of {title}";
            }
            
            target.Translations.Add(new StoryCraftTranslation
            {
                LanguageCode = translation.LanguageCode,
                Title = title,
                Summary = translation.Summary
            });
        }
    }

    private static void CopyCraftTiles(StoryCraft source, StoryCraft target)
    {
        var sortOrder = 0;
        foreach (var tile in source.Tiles.OrderBy(t => t.SortOrder))
        {
            var clonedTile = new StoryCraftTile
            {
                TileId = tile.TileId,
                Type = tile.Type,
                SortOrder = sortOrder++,
                ImageUrl = tile.ImageUrl
            };

            foreach (var tileTranslation in tile.Translations)
            {
                clonedTile.Translations.Add(new StoryCraftTileTranslation
                {
                    LanguageCode = tileTranslation.LanguageCode,
                    Caption = tileTranslation.Caption,
                    Text = tileTranslation.Text,
                    Question = tileTranslation.Question,
                    AudioUrl = tileTranslation.AudioUrl,
                    VideoUrl = tileTranslation.VideoUrl
                });
            }

            var answerSort = 0;
            foreach (var answer in tile.Answers.OrderBy(a => a.SortOrder))
            {
                var clonedAnswer = new StoryCraftAnswer
                {
                    AnswerId = answer.AnswerId,
                    SortOrder = answerSort++
                };

                foreach (var tr in answer.Translations)
                {
                    clonedAnswer.Translations.Add(new StoryCraftAnswerTranslation
                    {
                        LanguageCode = tr.LanguageCode,
                        Text = tr.Text ?? string.Empty
                    });
                }

                foreach (var token in answer.Tokens)
                {
                    clonedAnswer.Tokens.Add(new StoryCraftAnswerToken
                    {
                        Type = token.Type ?? string.Empty,
                        Value = token.Value ?? string.Empty,
                        Quantity = token.Quantity
                    });
                }

                clonedTile.Answers.Add(clonedAnswer);
            }

            target.Tiles.Add(clonedTile);
        }
    }

    private static void CopyCraftTopics(StoryCraft source, StoryCraft target)
    {
        foreach (var topic in source.Topics)
        {
            target.Topics.Add(new StoryCraftTopic
            {
                StoryTopicId = topic.StoryTopicId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static void CopyCraftAgeGroups(StoryCraft source, StoryCraft target)
    {
        foreach (var ageGroup in source.AgeGroups)
        {
            target.AgeGroups.Add(new StoryCraftAgeGroup
            {
                StoryAgeGroupId = ageGroup.StoryAgeGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static void CopyDefinitionTranslations(StoryDefinition definition, StoryCraft target, bool isCopy = false)
    {
        foreach (var defTr in definition.Translations)
        {
            var title = defTr.Title;
            // For copy operations, prefix title with "Copy of "
            if (isCopy && !string.IsNullOrWhiteSpace(title) && !title.StartsWith("Copy of ", StringComparison.OrdinalIgnoreCase))
            {
                title = $"Copy of {title}";
            }
            
            target.Translations.Add(new StoryCraftTranslation
            {
                LanguageCode = defTr.LanguageCode,
                Title = title,
                Summary = definition.Summary
            });
        }
    }

    private static void CopyDefinitionTiles(StoryDefinition definition, StoryCraft target)
    {
        var sortOrder = 0;
        foreach (var defTile in definition.Tiles.OrderBy(t => t.SortOrder))
        {
            var craftTile = new StoryCraftTile
            {
                TileId = defTile.TileId,
                Type = defTile.Type,
                SortOrder = sortOrder++,
                ImageUrl = ExtractFileName(defTile.ImageUrl)
            };

            foreach (var tileTranslation in defTile.Translations)
            {
                craftTile.Translations.Add(new StoryCraftTileTranslation
                {
                    LanguageCode = tileTranslation.LanguageCode,
                    Caption = tileTranslation.Caption ?? string.Empty,
                    Text = tileTranslation.Text ?? string.Empty,
                    Question = tileTranslation.Question ?? string.Empty,
                    AudioUrl = ExtractFileName(tileTranslation.AudioUrl),
                    VideoUrl = ExtractFileName(tileTranslation.VideoUrl)
                });
            }

            var answerSort = 0;
            foreach (var defAnswer in defTile.Answers.OrderBy(a => a.SortOrder))
            {
                var craftAnswer = new StoryCraftAnswer
                {
                    AnswerId = defAnswer.AnswerId,
                    SortOrder = answerSort++
                };

                foreach (var tr in defAnswer.Translations)
                {
                    craftAnswer.Translations.Add(new StoryCraftAnswerTranslation
                    {
                        LanguageCode = tr.LanguageCode,
                        Text = tr.Text ?? string.Empty
                    });
                }

                foreach (var token in defAnswer.Tokens)
                {
                    craftAnswer.Tokens.Add(new StoryCraftAnswerToken
                    {
                        Type = token.Type ?? string.Empty,
                        Value = token.Value ?? string.Empty,
                        Quantity = token.Quantity
                    });
                }

                craftTile.Answers.Add(craftAnswer);
            }

            target.Tiles.Add(craftTile);
        }
    }

    private static void CopyDefinitionTopics(StoryDefinition definition, StoryCraft target)
    {
        foreach (var defTopic in definition.Topics)
        {
            target.Topics.Add(new StoryCraftTopic
            {
                StoryTopicId = defTopic.StoryTopicId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static void CopyDefinitionAgeGroups(StoryDefinition definition, StoryCraft target)
    {
        foreach (var defAgeGroup in definition.AgeGroups)
        {
            target.AgeGroups.Add(new StoryCraftAgeGroup
            {
                StoryAgeGroupId = defAgeGroup.StoryAgeGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }
    }

    private static string? ExtractFileName(string? path)
    {
        return string.IsNullOrWhiteSpace(path) ? null : Path.GetFileName(path);
    }

    private static void ValidateInputs(Guid ownerUserId, string newStoryId)
    {
        if (ownerUserId == Guid.Empty)
        {
            throw new ArgumentException("ownerUserId must be valid", nameof(ownerUserId));
        }

        if (string.IsNullOrWhiteSpace(newStoryId))
        {
            throw new ArgumentException("newStoryId must be provided", nameof(newStoryId));
        }
    }
}

