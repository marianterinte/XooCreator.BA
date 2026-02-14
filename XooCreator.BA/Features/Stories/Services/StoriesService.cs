using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Features.Stories.DTOs;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services.Content;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.Stories.Services;

public interface IStoriesService
{
    Task<GetStoriesResponse> GetAllStoriesAsync(string locale);
    Task<GetStoryByIdResponse> GetStoryByIdAsync(Guid userId, string storyId, string locale);
    Task<MarkTileAsReadResponse> MarkTileAsReadAsync(Guid userId, MarkTileAsReadRequest request);
    Task<ResetStoryProgressResponse> ResetStoryProgressAsync(Guid userId, ResetStoryProgressRequest request);
    Task InitializeStoriesAsync();
    Task<EditableStoryDto?> GetStoryForEditAsync(string storyId, string locale);
}

public class StoriesService : IStoriesService
{
    private readonly IStoriesRepository _repository;
    private readonly IStoryCraftsRepository _crafts;
    private readonly XooDbContext _context;

    public StoriesService(IStoriesRepository repository, IStoryCraftsRepository crafts, XooDbContext context)
    {
        _repository = repository;
        _crafts = crafts;
        _context = context;
    }

    public async Task<GetStoriesResponse> GetAllStoriesAsync(string locale)
    {
        var stories = await _repository.GetAllStoriesAsync(locale);
        return new GetStoriesResponse
        {
            Stories = stories
        };
    }

    public async Task<GetStoryByIdResponse> GetStoryByIdAsync(Guid userId, string storyId, string locale)
    {
        var story = await _repository.GetStoryByIdAsync(storyId, locale);
        var userProgress = story != null
            ? await _repository.GetUserStoryProgressAsync(userId, storyId)
            : new List<UserStoryProgressDto>();

        // Check completion status from UserStoryReadHistory
        var isCompleted = false;
        DateTime? completedAt = null;
        if (story != null)
        {
            var completionInfo = await _repository.GetStoryCompletionStatusAsync(userId, storyId);
            isCompleted = completionInfo.IsCompleted;
            completedAt = completionInfo.CompletedAt;
        }

        // If story is completed and user is trying to read again, reset progress to allow re-reading
        if (story != null && story.Tiles?.Count > 0 && userProgress.Count >= story.Tiles.Count)
        {
            await _repository.ResetStoryProgressAsync(userId, storyId);
            userProgress = new List<UserStoryProgressDto>();
        }

        // Get available languages for this story and add to StoryContentDto
        if (story != null)
        {
            var availableLangs = await _crafts.GetAvailableLanguagesAsync(storyId);
            if (availableLangs.Count == 0)
            {
                // If no languages found in StoryCraft, try to get from StoryDefinition translations
                var storyDef = await _repository.GetStoryDefinitionByIdAsync(storyId);
                if (storyDef?.Translations != null && storyDef.Translations.Count > 0)
                {
                    availableLangs = storyDef.Translations.Select(t => t.LanguageCode).Distinct().ToList();
                }
                else
                {
                    availableLangs = new List<string> { "ro-ro" }; // Default to ro-ro if no languages found
                }
            }

            // Create new StoryContentDto with AvailableLanguages
            story = story with { AvailableLanguages = availableLangs };
        }

        return new GetStoryByIdResponse
        {
            Story = story,
            UserProgress = userProgress,
            IsCompleted = isCompleted,
            CompletedAt = completedAt
        };
    }

    public async Task<MarkTileAsReadResponse> MarkTileAsReadAsync(Guid userId, MarkTileAsReadRequest request)
    {
        try
        {
            var success = await _repository.MarkTileAsReadAsync(userId, request.StoryId, request.TileId);
            
            return new MarkTileAsReadResponse
            {
                Success = success,
                ErrorMessage = success ? null : "Failed to mark tile as read"
            };
        }
        catch (Exception ex)
        {
            return new MarkTileAsReadResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<ResetStoryProgressResponse> ResetStoryProgressAsync(Guid userId, ResetStoryProgressRequest request)
    {
        try
        {
            await _repository.ResetStoryProgressAsync(userId, request.StoryId);
            return new ResetStoryProgressResponse
            {
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new ResetStoryProgressResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task InitializeStoriesAsync()
    {
        await _repository.SeedStoriesAsync();
        await _repository.SeedIndependentStoriesAsync();
    }

    public async Task<EditableStoryDto?> GetStoryForEditAsync(string storyId, string locale)
    {
        // Get available languages for this story
        var availableLangs = await _crafts.GetAvailableLanguagesAsync(storyId);
        if (availableLangs.Count == 0)
        {
            availableLangs = new List<string> { "ro-ro" }; // Default to ro-ro if no languages found
        }

        // 1) Try to load StoryCraft (editor working copy) first
        var lang = locale.ToLowerInvariant();
        var craft = await _crafts.GetWithLanguageAsync(storyId, lang);
        if (craft != null)
        {
            var translation = craft.Translations.FirstOrDefault(t => t.LanguageCode == lang);
            
            // Load UnlockedStoryHeroes from many-to-many table
            var unlockedHeroes = craft.UnlockedHeroes.Select(h => h.HeroId).ToList();
            
            // Get owner email from OwnerUserId
            string? ownerEmail = null;
            if (craft.OwnerUserId != Guid.Empty)
            {
                ownerEmail = await _context.Set<AlchimaliaUser>()
                    .Where(u => u.Id == craft.OwnerUserId)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();
            }
            
            var coAuthors = (craft.CoAuthors ?? Enumerable.Empty<StoryCraftCoAuthor>())
                .OrderBy(c => c.SortOrder)
                .Select(c => new StoryCoAuthorDto
                {
                    UserId = c.UserId,
                    DisplayName = c.UserId.HasValue && c.User != null ? c.User.Name : (c.DisplayName ?? string.Empty)
                })
                .ToList();

            return new EditableStoryDto
            {
                Id = craft.StoryId,
                Title = translation?.Title ?? string.Empty,
                CoverImageUrl = craft.CoverImageUrl ?? string.Empty,
                Summary = translation?.Summary,
                StoryTopic = craft.StoryTopic, // Keep for backward compatibility
                AuthorName = craft.AuthorName,
                ClassicAuthorId = craft.ClassicAuthorId,
                TopicIds = craft.Topics.Select(t => t.StoryTopic.TopicId).ToList(),
                AgeGroupIds = craft.AgeGroups.Select(ag => ag.StoryAgeGroup.AgeGroupId).ToList(),
                CoAuthors = coAuthors,
                PriceInCredits = craft.PriceInCredits,
                StoryType = (int)craft.StoryType,
                IsEvaluative = craft.IsEvaluative,
                IsPartOfEpic = craft.IsPartOfEpic,
                UnlockedStoryHeroes = unlockedHeroes,
                DialogParticipants = craft.DialogParticipants
                    .OrderBy(p => p.SortOrder)
                    .Select(p => p.HeroId)
                    .ToList(),
                Status = MapStatusForFrontend(StoryStatusExtensions.FromDb(craft.Status)),
                AvailableLanguages = availableLangs,
                AudioLanguages = craft.AudioLanguages ?? new List<string>(),
                AssignedReviewerUserId = craft.AssignedReviewerUserId,
                ReviewedByUserId = craft.ReviewedByUserId,
                ApprovedByUserId = craft.ApprovedByUserId,
                ReviewNotes = craft.ReviewNotes,
                ReviewStartedAt = craft.ReviewStartedAt,
                ReviewEndedAt = craft.ReviewEndedAt,
                BaseVersion = craft.BaseVersion,
                OwnerEmail = ownerEmail,
                Tiles = craft.Tiles.OrderBy(t => t.SortOrder).Select(t =>
                {
                    var tileTranslation = t.Translations.FirstOrDefault(tr => tr.LanguageCode == lang);
                    
                    return new EditableTileDto
                    {
                        Type = t.Type,
                        Id = t.TileId,
                        BranchId = t.BranchId,
                        Caption = tileTranslation?.Caption,
                        Text = string.Equals(t.Type, "dialog", StringComparison.OrdinalIgnoreCase)
                            ? t.DialogTile?.Nodes
                                .OrderBy(n => n.SortOrder)
                                .Select(n => n.Translations.FirstOrDefault(nt => nt.LanguageCode == lang)?.Text)
                                .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text))
                            : tileTranslation?.Text,
                        ImageUrl = t.ImageUrl ?? string.Empty,
                        // Audio and Video are now language-specific (from translation)
                        AudioUrl = tileTranslation?.AudioUrl ?? string.Empty,
                        VideoUrl = tileTranslation?.VideoUrl ?? string.Empty,
                        Question = tileTranslation?.Question,
                        DialogRootNodeId = t.DialogTile?.RootNodeId,
                        DialogNodes = t.DialogTile?.Nodes
                            .OrderBy(n => n.SortOrder)
                            .Select(n => new EditableDialogNodeDto
                            {
                                NodeId = n.NodeId,
                                SpeakerType = n.SpeakerType,
                                SpeakerHeroId = n.SpeakerHeroId,
                                Text = n.Translations.FirstOrDefault(nt => nt.LanguageCode == lang)?.Text ?? string.Empty,
                                AudioUrl = n.Translations.FirstOrDefault(nt => nt.LanguageCode == lang)?.AudioUrl,
                                Options = n.OutgoingEdges
                                    .OrderBy(e => e.OptionOrder)
                                    .Select(e => new EditableDialogOptionDto
                                    {
                                        Id = e.EdgeId,
                                        NextNodeId = e.ToNodeId,
                                        Text = e.Translations.FirstOrDefault(et => et.LanguageCode == lang)?.OptionText ?? string.Empty,
                                        JumpToTileId = e.JumpToTileId,
                                        SetBranchId = e.SetBranchId,
                                        Tokens = (e.Tokens ?? new()).Select(tok => new EditableTokenDto
                                        {
                                            Type = tok.Type,
                                            Value = tok.Value,
                                            Quantity = tok.Quantity
                                        }).ToList()
                                    })
                                    .ToList()
                            })
                            .ToList() ?? new List<EditableDialogNodeDto>(),
                        Answers = t.Answers.OrderBy(a => a.SortOrder).Select(a =>
                        {
                            var answerTranslation = a.Translations.FirstOrDefault(at => at.LanguageCode == lang);
                            
                            var tokens = a.Tokens.Select(tok => new EditableTokenDto
                            {
                                Type = tok.Type,
                                Value = tok.Value,
                                Quantity = tok.Quantity
                            }).ToList();
                            
                            return new EditableAnswerDto
                            {
                                Id = a.AnswerId,
                                Text = answerTranslation?.Text ?? string.Empty,
                                IsCorrect = a.IsCorrect,
                                Tokens = tokens
                            };
                        }).ToList()
                    };
                }).ToList()
            };
        }

        // 2) Fallback to StoryDefinition for initial bootstrapping
        var story = await _repository.GetStoryDefinitionByIdAsync(storyId);
        if (story == null) return null;

        // Get translation for the requested locale
        var storyTranslation = story.Translations.FirstOrDefault(t => t.LanguageCode == locale) 
            ?? story.Translations.FirstOrDefault(t => t.LanguageCode == "ro-ro")
            ?? story.Translations.FirstOrDefault();

        // Load UnlockedStoryHeroes from published definition (DB)
        var unlockedHeroesFromJson = story.UnlockedHeroes?.Select(h => h.HeroId).ToList() ?? new List<string>();

        // Get owner email from CreatedBy (for published stories)
        string? ownerEmailFromDefinition = null;
        if (story.CreatedBy.HasValue)
        {
            ownerEmailFromDefinition = await _context.Set<AlchimaliaUser>()
                .Where(u => u.Id == story.CreatedBy.Value)
                .Select(u => u.Email)
                .FirstOrDefaultAsync();
        }

        var coAuthorsFromDef = (story.CoAuthors ?? Enumerable.Empty<StoryDefinitionCoAuthor>())
            .OrderBy(c => c.SortOrder)
            .Select(c => new StoryCoAuthorDto
            {
                UserId = c.UserId,
                DisplayName = c.UserId.HasValue && c.User != null ? c.User.Name : (c.DisplayName ?? string.Empty)
            })
            .ToList();

        return new EditableStoryDto
        {
            Id = story.StoryId,
            Title = storyTranslation?.Title ?? story.Title,
            CoverImageUrl = story.CoverImageUrl ?? string.Empty,
            Summary = story.Summary ?? string.Empty,
            StoryTopic = story.StoryTopic, // Keep for backward compatibility
            AuthorName = story.AuthorName,
            ClassicAuthorId = story.ClassicAuthorId,
            TopicIds = story.Topics?.Select(t => t.StoryTopic.TopicId).ToList() ?? new List<string>(),
            AgeGroupIds = story.AgeGroups?.Select(ag => ag.StoryAgeGroup.AgeGroupId).ToList() ?? new List<string>(),
            CoAuthors = coAuthorsFromDef,
            PriceInCredits = story.PriceInCredits,
            StoryType = (int)story.StoryType,
            IsEvaluative = story.IsEvaluative,
            IsPartOfEpic = story.IsPartOfEpic,
            UnlockedStoryHeroes = unlockedHeroesFromJson,
            DialogParticipants = story.DialogParticipants
                .OrderBy(p => p.SortOrder)
                .Select(p => p.HeroId)
                .ToList(),
            Status = MapStatusForFrontend(story.Status), // story.Status is already StoryStatus enum
            AvailableLanguages = availableLangs,
            AudioLanguages = story.AudioLanguages ?? new List<string>(),
            OwnerEmail = ownerEmailFromDefinition,
            Tiles = story.Tiles.OrderBy(t => t.SortOrder).Select(t =>
            {
                var tileTranslation = t.Translations.FirstOrDefault(tr => tr.LanguageCode == locale)
                    ?? t.Translations.FirstOrDefault(tr => tr.LanguageCode == "ro-ro")
                    ?? t.Translations.FirstOrDefault();

                return new EditableTileDto
                {
                    Type = t.Type,
                    Id = t.TileId,
                    BranchId = t.BranchId,
                    Caption = tileTranslation?.Caption ?? t.Caption ?? string.Empty,
                    Text = string.Equals(t.Type, "dialog", StringComparison.OrdinalIgnoreCase)
                        ? t.DialogTile?.Nodes
                            .OrderBy(n => n.SortOrder)
                            .Select(n => n.Translations.FirstOrDefault(nt => nt.LanguageCode == locale)?.Text)
                            .FirstOrDefault(text => !string.IsNullOrWhiteSpace(text))
                            ?? string.Empty
                        : tileTranslation?.Text ?? t.Text ?? string.Empty,
                    ImageUrl = t.ImageUrl ?? string.Empty,
                    // Audio and Video are now language-specific (from translation)
                    AudioUrl = tileTranslation?.AudioUrl ?? string.Empty,
                    VideoUrl = tileTranslation?.VideoUrl ?? string.Empty,
                    Question = tileTranslation?.Question ?? t.Question ?? string.Empty,
                    DialogRootNodeId = t.DialogTile?.RootNodeId,
                    DialogNodes = t.DialogTile?.Nodes
                        .OrderBy(n => n.SortOrder)
                        .Select(n => new EditableDialogNodeDto
                        {
                            NodeId = n.NodeId,
                            SpeakerType = n.SpeakerType,
                            SpeakerHeroId = n.SpeakerHeroId,
                            Text = n.Translations.FirstOrDefault(nt => nt.LanguageCode == locale)?.Text ?? string.Empty,
                            AudioUrl = n.Translations.FirstOrDefault(nt => nt.LanguageCode == locale)?.AudioUrl,
                            Options = n.OutgoingEdges
                                .OrderBy(e => e.OptionOrder)
                                .Select(e => new EditableDialogOptionDto
                                {
                                    Id = e.EdgeId,
                                    NextNodeId = e.ToNodeId,
                                    Text = e.Translations.FirstOrDefault(et => et.LanguageCode == locale)?.OptionText ?? string.Empty,
                                    JumpToTileId = e.JumpToTileId,
                                    SetBranchId = e.SetBranchId,
                                    Tokens = (e.Tokens ?? new()).Select(tok => new EditableTokenDto
                                    {
                                        Type = tok.Type ?? string.Empty,
                                        Value = tok.Value ?? string.Empty,
                                        Quantity = tok.Quantity
                                    }).ToList()
                                })
                                .ToList()
                        })
                        .ToList() ?? new List<EditableDialogNodeDto>(),
                    Answers = (t.Answers ?? new()).OrderBy(a => a.SortOrder).Select(a =>
                    {
                        var answerTranslation = a.Translations.FirstOrDefault(at => at.LanguageCode == locale)
                            ?? a.Translations.FirstOrDefault(at => at.LanguageCode == "ro-ro")
                            ?? a.Translations.FirstOrDefault();

                        return new EditableAnswerDto
                        {
                            Id = a.AnswerId,
                            Text = answerTranslation?.Text ?? a.Text ?? string.Empty,
                            IsCorrect = a.IsCorrect,
                            Tokens = (a.Tokens ?? new()).Select(tok => new EditableTokenDto
                            {
                                Type = tok.Type ?? string.Empty, 
                                Value = tok.Value ?? string.Empty,
                                Quantity = tok.Quantity
                            }).ToList()
                        };
                    }).ToList()
                };
            }).ToList()
        };
    }

    internal static string MapStatusForFrontendForExternal(StoryStatus status) => MapStatusForFrontend(status);

    private static string MapStatusForFrontend(StoryStatus status)
    {
        return status switch
        {
            StoryStatus.Approved => "approved",
            StoryStatus.Published => "published",
            StoryStatus.InReview => "in-review",
            StoryStatus.SentForApproval => "sent-for-approval",
            StoryStatus.ChangesRequested => "changes-requested",
            _ => "draft"
        };
    }

    // JSON-based unlocked heroes removed (DB is source of truth).

}


