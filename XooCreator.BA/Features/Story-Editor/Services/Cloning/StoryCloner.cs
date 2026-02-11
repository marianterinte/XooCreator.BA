using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using System.Linq;

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
                BranchId = tileData.BranchId,
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

            if (string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase))
            {
                var dialogTile = new StoryCraftDialogTile
                {
                    RootNodeId = tileData.DialogRootNodeId
                };

                foreach (var nodeData in tileData.DialogNodes.OrderBy(n => n.NodeId))
                {
                    var node = new StoryCraftDialogNode
                    {
                        NodeId = nodeData.NodeId,
                        SpeakerType = nodeData.SpeakerType,
                        SpeakerHeroId = nodeData.SpeakerHeroId,
                        SortOrder = dialogTile.Nodes.Count
                    };

                    foreach (var tr in nodeData.Translations)
                    {
                        node.Translations.Add(new StoryCraftDialogNodeTranslation
                        {
                            LanguageCode = tr.LanguageCode,
                            Text = tr.Text
                        });
                    }

                    foreach (var optionData in nodeData.Options.OrderBy(o => o.OptionOrder))
                    {
                        var edge = new StoryCraftDialogEdge
                        {
                            EdgeId = optionData.EdgeId,
                            ToNodeId = optionData.ToNodeId,
                            JumpToTileId = optionData.JumpToTileId,
                            SetBranchId = optionData.SetBranchId,
                            OptionOrder = optionData.OptionOrder
                        };
                        foreach (var edgeTr in optionData.Translations)
                        {
                            edge.Translations.Add(new StoryCraftDialogEdgeTranslation
                            {
                                LanguageCode = edgeTr.LanguageCode,
                                OptionText = edgeTr.OptionText
                            });
                        }
                        foreach (var token in optionData.Tokens)
                        {
                            edge.Tokens.Add(new StoryCraftDialogEdgeToken
                            {
                                Type = token.Type,
                                Value = token.Value,
                                Quantity = token.Quantity
                            });
                        }
                        node.OutgoingEdges.Add(edge);
                    }

                    dialogTile.Nodes.Add(node);
                }

                tile.DialogTile = dialogTile;
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

        // Clone unlocked heroes
        foreach (var heroId in cloneData.UnlockedStoryHeroes)
        {
            if (string.IsNullOrWhiteSpace(heroId))
                continue;

            craft.UnlockedHeroes.Add(new StoryCraftUnlockedHero
            {
                HeroId = heroId.Trim(),
                CreatedAt = DateTime.UtcNow
            });
        }

        foreach (var heroId in cloneData.DialogParticipants)
        {
            if (string.IsNullOrWhiteSpace(heroId))
            {
                continue;
            }

            craft.DialogParticipants.Add(new StoryCraftDialogParticipant
            {
                HeroId = heroId.Trim(),
                SortOrder = craft.DialogParticipants.Count,
                CreatedAt = DateTime.UtcNow
            });
        }

        return craft;
    }
}
