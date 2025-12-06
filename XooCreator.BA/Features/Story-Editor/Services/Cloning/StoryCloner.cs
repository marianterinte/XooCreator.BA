using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Services.Cloning;

/// <summary>
/// Service responsible for creating StoryCraft from unified clone data.
/// </summary>
public interface IStoryCloner
{
    StoryCraft CreateCraft(StoryCloneData cloneData, Guid ownerUserId, string newStoryId);
}

public class StoryCloner : IStoryCloner
{
    public StoryCraft CreateCraft(StoryCloneData cloneData, Guid ownerUserId, string newStoryId)
    {
        var craft = new StoryCraft
        {
            StoryId = newStoryId,
            OwnerUserId = ownerUserId,
            Status = StoryStatus.Draft.ToDb(),
            StoryType = cloneData.StoryType,
            StoryTopic = cloneData.StoryTopic,
            CoverImageUrl = cloneData.CoverImageUrl,
            PriceInCredits = cloneData.PriceInCredits,
            AuthorName = cloneData.AuthorName,
            ClassicAuthorId = cloneData.ClassicAuthorId,
            BaseVersion = cloneData.BaseVersion ?? 0,
            IsEvaluative = cloneData.IsEvaluative,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        // Clone translations
        foreach (var translation in cloneData.Translations)
        {
            craft.Translations.Add(new StoryCraftTranslation
            {
                LanguageCode = translation.LanguageCode,
                Title = translation.Title,
                Summary = translation.Summary
            });
        }

        // Clone tiles
        var sortOrder = 0;
        foreach (var tileData in cloneData.Tiles)
        {
            var tile = new StoryCraftTile
            {
                TileId = tileData.TileId,
                Type = tileData.Type,
                SortOrder = sortOrder++,
                ImageUrl = tileData.ImageUrl
            };

            // Clone tile translations
            foreach (var tileTranslation in tileData.Translations)
            {
                tile.Translations.Add(new StoryCraftTileTranslation
                {
                    LanguageCode = tileTranslation.LanguageCode,
                    Caption = tileTranslation.Caption,
                    Text = tileTranslation.Text,
                    Question = tileTranslation.Question,
                    AudioUrl = tileTranslation.AudioUrl,
                    VideoUrl = tileTranslation.VideoUrl
                });
            }

            // Clone answers
            var answerSort = 0;
            foreach (var answerData in tileData.Answers)
            {
                var answer = new StoryCraftAnswer
                {
                    AnswerId = answerData.AnswerId,
                    IsCorrect = answerData.IsCorrect,
                    SortOrder = answerSort++
                };

                // Clone answer translations
                foreach (var answerTranslation in answerData.Translations)
                {
                    answer.Translations.Add(new StoryCraftAnswerTranslation
                    {
                        LanguageCode = answerTranslation.LanguageCode,
                        Text = answerTranslation.Text
                    });
                }

                // Clone tokens
                foreach (var tokenData in answerData.Tokens)
                {
                    answer.Tokens.Add(new StoryCraftAnswerToken
                    {
                        Type = tokenData.Type,
                        Value = tokenData.Value,
                        Quantity = tokenData.Quantity
                    });
                }

                tile.Answers.Add(answer);
            }

            craft.Tiles.Add(tile);
        }

        // Clone topics
        foreach (var topicId in cloneData.Topics)
        {
            craft.Topics.Add(new StoryCraftTopic
            {
                StoryTopicId = topicId,
                CreatedAt = DateTime.UtcNow
            });
        }

        // Clone age groups
        foreach (var ageGroupId in cloneData.AgeGroups)
        {
            craft.AgeGroups.Add(new StoryCraftAgeGroup
            {
                StoryAgeGroupId = ageGroupId,
                CreatedAt = DateTime.UtcNow
            });
        }

        return craft;
    }
}
