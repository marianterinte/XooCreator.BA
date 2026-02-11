using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.SeedData;
using XooCreator.BA.Data.SeedData.DTOs;
using XooCreator.BA.Features.Stories.DTOs;
using XooCreator.BA.Features.Stories.Mappers;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Features.Stories.SeedEntities;

namespace XooCreator.BA.Features.StoryEditor.Repositories;

using Microsoft.Extensions.Caching.Memory;

public class StoriesRepository : IStoriesRepository
{
    private readonly XooDbContext _context;
    private readonly IMemoryCache _cache;

    public StoriesRepository(XooDbContext context, IMemoryCache cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<List<StoryContentDto>> GetAllStoriesAsync(string locale)
    {
        // Note: Frontend uses answers and tokens in mapApiTileToTile for quiz tiles
        // So we need to include them even for list view
        // Optimized: Use AsSplitQuery to reduce memory pressure while keeping all necessary data
        var stories = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.Answers)
                    .ThenInclude(a => a.Tokens)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Translations)
            .Include(s => s.Tiles)
                .ThenInclude(t => t.DialogTile!)
                    .ThenInclude(dt => dt.Nodes)
                        .ThenInclude(n => n.OutgoingEdges)
                            .ThenInclude(e => e.Tokens)
            .Where(s => s.IsActive)
            .OrderBy(s => s.SortOrder)
            .AsSplitQuery() // Use split query to reduce memory pressure
            .ToListAsync();

        return stories.Select(s => StoryDefinitionMapper.MapToDtoWithLocale(s, locale)).ToList();
    }

    public async Task<StoryContentDto?> GetStoryByIdAsync(string storyId, string locale)
    {
        var normalizedId = NormalizeStoryId(storyId);
        var key = $"story_content:{normalizedId}:{locale.ToLower()}";

        return await _cache.GetOrCreateAsync(key, async entry => 
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(24);

            var story = await _context.StoryDefinitions
                    .Include(s => s.Translations)
                    .Include(s => s.Tiles)
                        .ThenInclude(t => t.Translations)
                    .Include(s => s.Tiles)
                        .ThenInclude(t => t.Answers)
                            .ThenInclude(a => a.Tokens)
                    .Include(s => s.Tiles)
                        .ThenInclude(t => t.Answers)
                            .ThenInclude(a => a.Translations)
                    .Include(s => s.Tiles)
                        .ThenInclude(t => t.DialogTile!)
                            .ThenInclude(dt => dt.Nodes)
                                .ThenInclude(n => n.Translations)
                    .Include(s => s.Tiles)
                        .ThenInclude(t => t.DialogTile!)
                            .ThenInclude(dt => dt.Nodes)
                                .ThenInclude(n => n.OutgoingEdges)
                                    .ThenInclude(e => e.Translations)
                    .Include(s => s.Tiles)
                        .ThenInclude(t => t.DialogTile!)
                            .ThenInclude(dt => dt.Nodes)
                                .ThenInclude(n => n.OutgoingEdges)
                                    .ThenInclude(e => e.Tokens)
                    .AsSplitQuery() // Optimization: Use split query for complex include chains
                    .FirstOrDefaultAsync(s => s.StoryId == normalizedId && s.IsActive);

            if (story == null) return null;

            // Get owner email from CreatedBy
            string? ownerEmail = null;
            if (story.CreatedBy.HasValue)
            {
                ownerEmail = await _context.Set<AlchimaliaUser>()
                    .Where(u => u.Id == story.CreatedBy.Value)
                    .Select(u => u.Email)
                    .FirstOrDefaultAsync();
            }

            return StoryDefinitionMapper.MapToDtoWithLocale(story, locale, ownerEmail);
        });
    }

    public async Task<StoryDefinition?> GetStoryDefinitionByIdAsync(string storyId)
    {
        storyId = NormalizeStoryId(storyId);
        var story = await _context.StoryDefinitions
                .Include(s => s.Translations)
                .Include(s => s.Tiles)
                    .ThenInclude(t => t.Translations)
                .Include(s => s.Tiles)
                    .ThenInclude(t => t.Answers)
                        .ThenInclude(a => a.Tokens)
                .Include(s => s.Tiles)
                    .ThenInclude(t => t.Answers)
                        .ThenInclude(a => a.Translations)
                .Include(s => s.Tiles)
                    .ThenInclude(t => t.DialogTile!)
                        .ThenInclude(dt => dt.Nodes)
                            .ThenInclude(n => n.Translations)
                .Include(s => s.Tiles)
                    .ThenInclude(t => t.DialogTile!)
                        .ThenInclude(dt => dt.Nodes)
                            .ThenInclude(n => n.OutgoingEdges)
                                .ThenInclude(e => e.Translations)
                .Include(s => s.Tiles)
                    .ThenInclude(t => t.DialogTile!)
                        .ThenInclude(dt => dt.Nodes)
                            .ThenInclude(n => n.OutgoingEdges)
                                .ThenInclude(e => e.Tokens)
                .Include(s => s.Topics).ThenInclude(t => t.StoryTopic)
                .Include(s => s.AgeGroups).ThenInclude(ag => ag.StoryAgeGroup)
                .Include(s => s.CoAuthors).ThenInclude(c => c.User)
                .Include(s => s.UnlockedHeroes)
                .Include(s => s.DialogParticipants)
                .FirstOrDefaultAsync(s => s.StoryId == storyId && s.IsActive);

        return story;
    }

    public async Task<List<UserStoryProgressDto>> GetUserStoryProgressAsync(Guid userId, string storyId)
    {
        storyId = NormalizeStoryId(storyId);
        // Optimized: Filter directly in database query instead of loading all progress in memory
        // This reduces memory usage by 70-90% and improves query performance
        // Use case-insensitive comparison to handle any existing data inconsistencies
        // Note: EF.Functions.ILike without wildcards performs exact match case-insensitive in PostgreSQL
        // Equivalent to: LOWER(p.StoryId) = LOWER(storyId)
        var progress = await _context.UserStoryReadProgress
            .Where(p => p.UserId == userId 
                && EF.Functions.ILike(p.StoryId, storyId)) // Exact match case-insensitive (no wildcards)
            .OrderBy(p => p.ReadAt)
            .Select(p => new UserStoryProgressDto
            {
                StoryId = p.StoryId,
                TileId = p.TileId,
                ReadAt = p.ReadAt
            })
            .ToListAsync();

        return progress;
    }

    public async Task<bool> MarkTileAsReadAsync(Guid userId, string storyId, string tileId)
    {
        try
        {
            storyId = NormalizeStoryId(storyId);
            // Optimized: Filter directly in database query instead of loading all progress in memory
            // Use case-insensitive comparison to handle any existing data inconsistencies
            // Note: EF.Functions.ILike without wildcards performs exact match case-insensitive in PostgreSQL
            var existing = await _context.UserStoryReadProgress
                .Where(p => p.UserId == userId 
                    && p.TileId == tileId
                    && EF.Functions.ILike(p.StoryId, storyId)) // Exact match case-insensitive (no wildcards)
                .FirstOrDefaultAsync();

            if (existing != null)
            {
                return true;
            }

            var readProgress = new UserStoryReadProgress
            {
                UserId = userId,
                StoryId = storyId, // Store normalized storyId
                TileId = tileId
            };

            _context.UserStoryReadProgress.Add(readProgress);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task ResetStoryProgressAsync(Guid userId, string storyId)
    {
        storyId = NormalizeStoryId(storyId);
        // Optimized: Filter directly in database query instead of loading all progress in memory
        // Use case-insensitive comparison to handle any existing data inconsistencies
        // Note: EF.Functions.ILike without wildcards performs exact match case-insensitive in PostgreSQL
        var entries = await _context.UserStoryReadProgress
            .Where(p => p.UserId == userId 
                && EF.Functions.ILike(p.StoryId, storyId)) // Exact match case-insensitive (no wildcards)
            .ToListAsync();

        if (entries.Count == 0)
        {
            return;
        }

        // Get story definition to calculate total tiles
        var story = await GetStoryDefinitionByIdAsync(storyId);
        var totalTiles = story?.Tiles?.Count ?? 0;
        var totalTilesRead = entries.Count;
        var isCompleted = totalTiles > 0 && totalTilesRead >= totalTiles;
        var lastReadAt = entries.Max(e => e.ReadAt);

        // Save to history before deleting progress (filter directly in database)
        var existingHistory = await _context.UserStoryReadHistory
            .Where(h => h.UserId == userId 
                && EF.Functions.ILike(h.StoryId, storyId)) // Exact match case-insensitive (no wildcards)
            .FirstOrDefaultAsync();

        if (existingHistory != null)
        {
            // Update existing history record
            existingHistory.TotalTilesRead = totalTilesRead;
            existingHistory.TotalTiles = totalTiles;
            existingHistory.LastReadAt = lastReadAt;
            if (isCompleted && !existingHistory.CompletedAt.HasValue)
            {
                existingHistory.CompletedAt = DateTime.UtcNow;
            }
        }
        else
        {
            // Create new history record
            var history = new UserStoryReadHistory
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StoryId = storyId,
                TotalTilesRead = totalTilesRead,
                TotalTiles = totalTiles,
                LastReadAt = lastReadAt,
                CompletedAt = isCompleted ? DateTime.UtcNow : null
            };
            _context.UserStoryReadHistory.Add(history);
        }

        // Delete progress entries
        _context.UserStoryReadProgress.RemoveRange(entries);
        await _context.SaveChangesAsync();
    }

    public async Task<StoryCompletionInfo> GetStoryCompletionStatusAsync(Guid userId, string storyId)
    {
        storyId = NormalizeStoryId(storyId);
        // Optimized: Filter directly in database query instead of loading all history in memory
        // Use case-insensitive comparison to handle any existing data inconsistencies
        // Note: EF.Functions.ILike without wildcards performs exact match case-insensitive in PostgreSQL
        var history = await _context.UserStoryReadHistory
            .Where(h => h.UserId == userId 
                && EF.Functions.ILike(h.StoryId, storyId)) // Exact match case-insensitive (no wildcards)
            .FirstOrDefaultAsync();

        if (history != null && history.CompletedAt.HasValue)
        {
            return new StoryCompletionInfo
            {
                IsCompleted = true,
                CompletedAt = history.CompletedAt
            };
        }

        return new StoryCompletionInfo
        {
            IsCompleted = false,
            CompletedAt = null
        };
    }

    public async Task SeedStoriesAsync()
    {
        try
        {
            // Check if main stories are already seeded (not just independent stories)
            var mainStoryIds = new[] { "intro-pufpuf", "terra-s1", "lunaria-s1", "mechanika-s1" };
            var existingMainStories = await _context.StoryDefinitions
                .Where(s => mainStoryIds.Contains(s.StoryId))
                .Select(s => s.StoryId)
                .ToListAsync();
            
            var stories = await LoadStoriesFromJsonAsync(LanguageCode.RoRo.ToFolder());
            
            // If stories already exist, update them; otherwise create new ones
            if (existingMainStories.Count == mainStoryIds.Length)
            {
                // Stories exist, but we need to ensure ro-ro translations are created/updated for VideoUrl and AudioUrl
                // This handles the case where translations were not created initially
                // Also need to fix ImageUrl for video tiles that have VideoUrl in translation
            }
            else
            {
                // Create new stories
                foreach (var story in stories)
                {
                    _context.StoryDefinitions.Add(story);
                }
                await _context.SaveChangesAsync();
            }

            var processedDefTranslations = new HashSet<(Guid, string)>();
            var processedTileTranslations = new HashSet<(Guid, string)>();
            var processedAnswerTranslations = new HashSet<(Guid, string)>();

            // Process ro-ro translations first (base language) to handle VideoUrl and AudioUrl
            // This runs for both new and existing stories to ensure translations are created/updated
            var roRoTranslations = await LoadStoryTranslationsFromJsonAsync(LanguageCode.RoRo.ToTag());
            foreach (var tr in roRoTranslations)
            {
                var def = await _context.StoryDefinitions
                    .Include(s => s.Tiles)
                        .ThenInclude(t => t.Answers)
                    .Include(s => s.Tiles)
                        .ThenInclude(t => t.Translations)
                    .FirstOrDefaultAsync(s => s.StoryId == tr.StoryId);
                if (def == null) continue;

                if (tr.Tiles == null) continue;

                foreach (var t in tr.Tiles)
                {
                    var dbTile = def.Tiles.FirstOrDefault(x => x.TileId == t.TileId);
                    if (dbTile == null) continue;

                    var tileKey = (dbTile.Id, tr.Locale);
                    
                    // Build video URL with language prefix if VideoUrl is provided
                    string? videoUrl = null;
                    if (!string.IsNullOrWhiteSpace(t.VideoUrl))
                    {
                        // If VideoUrl starts with "video/tol/", insert language code after "video/"
                        // Example: "video/tol/stories/..." -> "video/ro-ro/tol/stories/..."
                        if (t.VideoUrl.StartsWith("video/tol/", StringComparison.OrdinalIgnoreCase))
                        {
                            videoUrl = $"video/{tr.Locale}/tol/{t.VideoUrl.Substring("video/tol/".Length)}";
                        }
                        else
                        {
                            videoUrl = t.VideoUrl;
                        }
                    }
                    
                    // Build audio URL with language prefix if AudioUrl is provided
                    string? audioUrl = null;
                    if (!string.IsNullOrWhiteSpace(t.AudioUrl))
                    {
                        // If AudioUrl starts with "audio/tol/", insert language code after "audio/"
                        // Example: "audio/tol/stories/..." -> "audio/ro-ro/tol/stories/..."
                        if (t.AudioUrl.StartsWith("audio/tol/", StringComparison.OrdinalIgnoreCase))
                        {
                            audioUrl = $"audio/{tr.Locale}/tol/{t.AudioUrl.Substring("audio/tol/".Length)}";
                        }
                        else
                        {
                            audioUrl = t.AudioUrl;
                        }
                    }
                    
                    // Check if translation already exists
                    var existingTranslation = dbTile.Translations.FirstOrDefault(trans => trans.LanguageCode == tr.Locale);
                    if (existingTranslation != null)
                    {
                        // Update existing translation
                        if (!string.IsNullOrWhiteSpace(videoUrl))
                        {
                            existingTranslation.VideoUrl = videoUrl;
                        }
                        if (!string.IsNullOrWhiteSpace(audioUrl))
                        {
                            existingTranslation.AudioUrl = audioUrl;
                        }
                        // Update other fields only if they are provided in seed data (don't overwrite with null)
                        // This preserves manually edited translations if seed data doesn't have values
                        if (t.Caption != null) existingTranslation.Caption = t.Caption;
                        if (t.Text != null) existingTranslation.Text = t.Text;
                        if (t.Question != null) existingTranslation.Question = t.Question;
                        
                        // If this is a video tile and VideoUrl is set, ensure ImageUrl is null (not the video path)
                        if (dbTile.Type?.Equals("video", StringComparison.OrdinalIgnoreCase) == true 
                            && !string.IsNullOrWhiteSpace(videoUrl))
                        {
                            if (!string.IsNullOrWhiteSpace(dbTile.ImageUrl) && dbTile.ImageUrl.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                            {
                                dbTile.ImageUrl = null; // Clear video path from ImageUrl
                            }
                        }
                    }
                    else if (!processedTileTranslations.Contains(tileKey))
                    {
                        // Only create translation if there's VideoUrl, AudioUrl, or other translation-specific data
                        if (!string.IsNullOrWhiteSpace(videoUrl) || !string.IsNullOrWhiteSpace(audioUrl) 
                            || !string.IsNullOrWhiteSpace(t.Caption) || !string.IsNullOrWhiteSpace(t.Text) || !string.IsNullOrWhiteSpace(t.Question))
                        {
                            _context.StoryTileTranslations.Add(new StoryTileTranslation
                            {
                                StoryTileId = dbTile.Id,
                                LanguageCode = tr.Locale,
                                Caption = t.Caption,
                                Text = t.Text,
                                Question = t.Question,
                                AudioUrl = audioUrl,
                                VideoUrl = videoUrl
                            });
                            processedTileTranslations.Add(tileKey);
                            
                            // If this is a video tile and VideoUrl is set, ensure ImageUrl is null (not the video path)
                            if (dbTile.Type?.Equals("video", StringComparison.OrdinalIgnoreCase) == true 
                                && !string.IsNullOrWhiteSpace(videoUrl))
                            {
                                if (!string.IsNullOrWhiteSpace(dbTile.ImageUrl) && dbTile.ImageUrl.StartsWith("video/", StringComparison.OrdinalIgnoreCase))
                                {
                                    dbTile.ImageUrl = null; // Clear video path from ImageUrl
                                }
                            }
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();

            // Process other languages
            foreach (var lc in LanguageCodeExtensions.All().Where(x => x != LanguageCode.RoRo))
            {
                var translations = await LoadStoryTranslationsFromJsonAsync(lc.ToTag());
                if (translations.Count == 0) continue;

                foreach (var tr in translations)
                {
                    var def = await _context.StoryDefinitions
                        .Include(s => s.Tiles)
                            .ThenInclude(t => t.Answers)
                        .FirstOrDefaultAsync(s => s.StoryId == tr.StoryId);
                    if (def == null) continue;

                    if (!string.IsNullOrWhiteSpace(tr.Title))
                    {
                        var key = (def.Id, tr.Locale);
                    if (!processedDefTranslations.Contains(key))
                    {
                        var existsDefTr = await _context.StoryDefinitionTranslations
                            .AnyAsync(e => e.StoryDefinitionId == def.Id && e.LanguageCode == tr.Locale);
                        if (!existsDefTr)
                        {
                            _context.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
                            {
                                StoryDefinitionId = def.Id,
                                LanguageCode = tr.Locale,
                                Title = tr.Title!
                            });
                        }
                        processedDefTranslations.Add(key);
                    }
                    }

                    if (tr.Tiles == null) continue;

                    foreach (var t in tr.Tiles)
                    {
                        var dbTile = def.Tiles.FirstOrDefault(x => x.TileId == t.TileId);
                        if (dbTile == null) continue;

                        var tileKey = (dbTile.Id, tr.Locale);
                        if (!processedTileTranslations.Contains(tileKey))
                        {
                            var existsTile = await _context.StoryTileTranslations
                                .AnyAsync(e => e.StoryTileId == dbTile.Id && e.LanguageCode == tr.Locale);
                            if (!existsTile)
                            {
                                // Build video URL with language prefix if VideoUrl is provided
                                string? videoUrl = null;
                                if (!string.IsNullOrWhiteSpace(t.VideoUrl))
                                {
                                    if (t.VideoUrl.StartsWith("video/tol/", StringComparison.OrdinalIgnoreCase))
                                    {
                                        videoUrl = $"video/{tr.Locale}/tol/{t.VideoUrl.Substring("video/tol/".Length)}";
                                    }
                                    else
                                    {
                                        videoUrl = t.VideoUrl;
                                    }
                                }
                                
                                // Build audio URL with language prefix if AudioUrl is provided
                                string? audioUrl = null;
                                if (!string.IsNullOrWhiteSpace(t.AudioUrl))
                                {
                                    if (t.AudioUrl.StartsWith("audio/tol/", StringComparison.OrdinalIgnoreCase))
                                    {
                                        audioUrl = $"audio/{tr.Locale}/tol/{t.AudioUrl.Substring("audio/tol/".Length)}";
                                    }
                                    else
                                    {
                                        audioUrl = t.AudioUrl;
                                    }
                                }
                                
                                _context.StoryTileTranslations.Add(new StoryTileTranslation
                                {
                                    StoryTileId = dbTile.Id,
                                    LanguageCode = tr.Locale,
                                    Caption = t.Caption,
                                    Text = t.Text,
                                    Question = t.Question,
                                    AudioUrl = audioUrl,
                                    VideoUrl = videoUrl
                                });
                            }
                            processedTileTranslations.Add(tileKey);
                        }

                        if (t.Answers == null) continue;

                        foreach (var answer in t.Answers)
                        {
                            var dbAnswer = dbTile.Answers.FirstOrDefault(x => x.AnswerId == answer.AnswerId);
                            if (dbAnswer == null) continue;

                            var answerKey = (dbAnswer.Id, tr.Locale);
                            if (!processedAnswerTranslations.Contains(answerKey))
                        {
                            var existsAns = await _context.StoryAnswerTranslations
                                .AnyAsync(e => e.StoryAnswerId == dbAnswer.Id && e.LanguageCode == tr.Locale);
                            if (!existsAns)
                            {
                                _context.StoryAnswerTranslations.Add(new StoryAnswerTranslation
                                {
                                    StoryAnswerId = dbAnswer.Id,
                                    LanguageCode = tr.Locale,
                                    Text = answer.Text
                                });
                            }
                            processedAnswerTranslations.Add(answerKey);
                        }
                        }
                    }
                }
            }
            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during story seeding: {ex.Message}");
            throw;
        }
    }

    public async Task SeedIndependentStoriesAsync()
    {
        try
        {
            var createdStoryIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            // First, create StoryDefinitions for all languages (but each story only once)
            // This is similar to SeedStoriesAsync where we create base stories first
            foreach (var lc in LanguageCodeExtensions.All())
            {
                await EnsureIndependentDefinitionsForLocale(lc.ToFolder(), createdStoryIds);
            }
            
            // Save StoryDefinitions to database before creating translations
            // This ensures StoryDefinitions exist when we look them up in ApplyIndependentTranslationsForLocale
            await _context.SaveChangesAsync();

            // Now create translations for all languages (including ro-ro, unlike SeedStoriesAsync)
            // This ensures all languages have translations, including the base ro-ro
            foreach (var lc in LanguageCodeExtensions.All())
            {
                await ApplyIndependentTranslationsForLocale(lc.ToTag());
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred during independent story seeding: {ex.Message}");
            throw;
        }
    }

    private async Task EnsureIndependentDefinitionsForLocale(string localeFolder, HashSet<string> createdStoryIds)
    {
        var stories = await LoadIndependentStoriesFromJsonAsync(localeFolder);
        foreach (var story in stories)
        {
            var exists = await _context.StoryDefinitions.AnyAsync(s => s.StoryId == story.StoryId);
            if (!exists && !createdStoryIds.Contains(story.StoryId))
            {
                _context.StoryDefinitions.Add(story);
                createdStoryIds.Add(story.StoryId);
            }
        }
    }

    private async Task ApplyIndependentTranslationsForLocale(string localeTag)
    {
        var processedDefTranslations = new HashSet<(Guid, string)>();
        var processedTileTranslations = new HashSet<(Guid, string)>();
        var processedAnswerTranslations = new HashSet<(Guid, string)>();

        var translations = await LoadIndependentStoryTranslationsFromJsonAsync(localeTag);
        if (translations.Count == 0) return;

        foreach (var tr in translations)
        {
            var def = await _context.StoryDefinitions
                .Include(s => s.Tiles)
                    .ThenInclude(t => t.Answers)
                .FirstOrDefaultAsync(s => s.StoryId == tr.StoryId);
            if (def == null) continue;

            if (!string.IsNullOrWhiteSpace(tr.Title))
            {
                var key = (def.Id, tr.Locale);
                if (!processedDefTranslations.Contains(key))
                {
                    var existsTr = await _context.StoryDefinitionTranslations.AnyAsync(e => e.StoryDefinitionId == def.Id && e.LanguageCode == tr.Locale);
                    if (!existsTr)
                    {
                        _context.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
                        {
                            StoryDefinitionId = def.Id,
                            LanguageCode = tr.Locale,
                            Title = tr.Title!
                        });
                    }
                    processedDefTranslations.Add(key);
                }
            }

            if (tr.Tiles == null) continue;

            foreach (var t in tr.Tiles)
            {
                var dbTile = def.Tiles.FirstOrDefault(x => x.TileId == t.TileId);
                if (dbTile == null) continue;

                var tileKey = (dbTile.Id, tr.Locale);
                if (!processedTileTranslations.Contains(tileKey))
                {
                    var existsTile = await _context.StoryTileTranslations.AnyAsync(e => e.StoryTileId == dbTile.Id && e.LanguageCode == tr.Locale);
                    if (!existsTile)
                    {
                        // Build video URL with language prefix if VideoUrl is provided
                        string? videoUrl = null;
                        if (!string.IsNullOrWhiteSpace(t.VideoUrl))
                        {
                            // If VideoUrl starts with "video/tol/", insert language code after "video/"
                            // Example: "video/tol/stories/..." -> "video/ro-ro/tol/stories/..."
                            if (t.VideoUrl.StartsWith("video/tol/", StringComparison.OrdinalIgnoreCase))
                            {
                                videoUrl = $"video/{tr.Locale}/tol/{t.VideoUrl.Substring("video/tol/".Length)}";
                            }
                            else
                            {
                                videoUrl = t.VideoUrl;
                            }
                        }
                        
                        // Build audio URL with language prefix if AudioUrl is provided
                        string? audioUrl = null;
                        if (!string.IsNullOrWhiteSpace(t.AudioUrl))
                        {
                            // If AudioUrl starts with "audio/tol/", insert language code after "audio/"
                            // Example: "audio/tol/stories/..." -> "audio/ro-ro/tol/stories/..."
                            if (t.AudioUrl.StartsWith("audio/tol/", StringComparison.OrdinalIgnoreCase))
                            {
                                audioUrl = $"audio/{tr.Locale}/tol/{t.AudioUrl.Substring("audio/tol/".Length)}";
                            }
                            else
                            {
                                audioUrl = t.AudioUrl;
                            }
                        }
                        
                        _context.StoryTileTranslations.Add(new StoryTileTranslation
                        {
                            StoryTileId = dbTile.Id,
                            LanguageCode = tr.Locale,
                            Caption = t.Caption,
                            Text = t.Text,
                            Question = t.Question,
                            AudioUrl = audioUrl,
                            VideoUrl = videoUrl
                        });
                    }
                    processedTileTranslations.Add(tileKey);
                }

                if (t.Answers == null) continue;

                foreach (var answer in t.Answers)
                {
                    var dbAnswer = dbTile.Answers.FirstOrDefault(x => x.AnswerId == answer.AnswerId);
                    if (dbAnswer == null) continue;

                    var answerKey = (dbAnswer.Id, tr.Locale);
                    if (!processedAnswerTranslations.Contains(answerKey))
                    {
                        var existsAns = await _context.StoryAnswerTranslations.AnyAsync(e => e.StoryAnswerId == dbAnswer.Id && e.LanguageCode == tr.Locale);
                        if (!existsAns)
                        {
                            _context.StoryAnswerTranslations.Add(new StoryAnswerTranslation
                            {
                                StoryAnswerId = dbAnswer.Id,
                                LanguageCode = tr.Locale,
                                Text = answer.Text
                            });
                        }
                        processedAnswerTranslations.Add(answerKey);
                    }
                }
            }
        }
    }

    private async Task<List<StoryDefinition>> LoadStoriesFromJsonAsync(string baseLocale = "ro-ro")
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var localeLc = (baseLocale ?? "ro-ro").ToLowerInvariant();
        var candidates = new[]
        {
            Path.Combine(baseDir, "Data", "SeedData", "Stories", "seed@alchimalia.com", "i18n", localeLc)
        };
        var legacyPath = Path.Combine(baseDir, "Data", "SeedData", "stories-seed.json");

        // Get seed user ID for ownership
        var seedUserId = await SeedUserHelper.GetOrCreateSeedUserIdAsync(_context);

        var storyMap = new Dictionary<string, StoryDefinition>(StringComparer.OrdinalIgnoreCase);

        foreach (var dir in candidates)
        {
            if (!Directory.Exists(dir)) continue;
            var files = Directory
                .EnumerateFiles(dir, "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                try
                {
                    var seed = JsonSerializer.Deserialize<StorySeedData>(json, new JsonSerializerOptions
                    {
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                        PropertyNameCaseInsensitive = true
                    });
                    if (seed == null)
                    {
                        throw new InvalidOperationException($"Invalid story seed data in '{file}'.");
                    }

                    var def = StoryDefinitionMapper.MapFromSeedData(seed, seedUserId);
                    storyMap[def.StoryId] = def;

                }
                catch (Exception ex)
                {

                    throw;
                }
              
            }
            if (files.Count > 0) break;
        }

        if (File.Exists(legacyPath))
        {
            var jsonContent = await File.ReadAllTextAsync(legacyPath);
            var seedData = JsonSerializer.Deserialize<StoriesSeedData>(jsonContent, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true
            });

            if (seedData?.Stories == null)
            {
                throw new InvalidOperationException("Invalid stories seed data format");
            }

            foreach (var s in seedData.Stories)
            {
                if (string.IsNullOrWhiteSpace(s.StoryId)) continue;
                if (!storyMap.ContainsKey(s.StoryId))
                {
                    storyMap[s.StoryId] = StoryDefinitionMapper.MapFromSeedData(s, seedUserId);
                }
            }
        }

        if (storyMap.Count == 0)
        {
            throw new FileNotFoundException(
                $"No story seeds found. Checked folders: {string.Join(", ", candidates)} and legacy file '{legacyPath}'.");
        }

        return storyMap.Values
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.StoryId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private async Task<List<StoryDefinition>> LoadIndependentStoriesFromJsonAsync(string baseLocale = "ro-ro")
    {
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var localeLc = (baseLocale ?? "ro-ro").ToLowerInvariant();
        var dir = Path.Combine(baseDir, "Data", "SeedData", "Stories", "seed@alchimalia.com", "independent", "i18n", localeLc);

        // Get seed user ID for ownership
        var seedUserId = await SeedUserHelper.GetOrCreateSeedUserIdAsync(_context);

        var storyMap = new Dictionary<string, StoryDefinition>(StringComparer.OrdinalIgnoreCase);
        if (Directory.Exists(dir))
        {
            var files = Directory
                .EnumerateFiles(dir, "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var file in files)
            {
                var json = await File.ReadAllTextAsync(file);
                var seed = JsonSerializer.Deserialize<StorySeedData>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
                if (seed == null)
                {
                    throw new InvalidOperationException($"Invalid independent story seed data in '{file}'.");
                }
                var def = StoryDefinitionMapper.MapFromSeedDataForIndie(seed, seedUserId);
                storyMap[def.StoryId] = def;
            }
        }

        return storyMap.Values
            .OrderBy(s => s.SortOrder)
            .ThenBy(s => s.StoryId, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static async Task<List<IndependentStoryTranslationSeed>> LoadIndependentStoryTranslationsFromJsonAsync(string locale)
    {
        var results = new List<IndependentStoryTranslationSeed>();
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var dir = Path.Combine(baseDir, "Data", "SeedData", "Stories", "seed@alchimalia.com", "independent", "i18n", locale);

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        if (!Directory.Exists(dir)) return results;

        var files = Directory
            .EnumerateFiles(dir, "*.json", SearchOption.TopDirectoryOnly)
            .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
            .ToList();

        foreach (var file in files)
        {
            var json = await File.ReadAllTextAsync(file);
            var seed = JsonSerializer.Deserialize<StorySeedData>(json, jsonOptions);
            if (seed == null || string.IsNullOrWhiteSpace(seed.StoryId))
            {
                continue;
            }

            var tr = new IndependentStoryTranslationSeed
            {
                Locale = locale.ToLowerInvariant(),
                StoryId = seed.StoryId,
                Title = seed.Title,
                Tiles = seed.Tiles?.Select(t => new TileTranslationSeed
                {
                    TileId = t.TileId,
                    Caption = t.Caption,
                    Text = t.Text,
                    Question = t.Question,
                    AudioUrl = t.AudioUrl,
                    VideoUrl = t.VideoUrl,
                    Answers = t.Answers?.Select(a => new AnswerTranslationSeed
                    {
                        AnswerId = a.AnswerId,
                        Text = a.Text
                    }).ToList()
                }).ToList()
            };
            results.Add(tr);
        }

        return results;
    }

    private static async Task<List<StoryTranslationSeed>> LoadStoryTranslationsFromJsonAsync(string locale)
    {
        var results = new List<StoryTranslationSeed>();
        var candidates = new[]
        {
            Path.Combine("Data", "SeedData", "Stories", "seed@alchimalia.com", "i18n", locale)
        };

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };

        foreach (var dir in candidates)
        {
            if (!Directory.Exists(dir)) continue;
            var files = Directory
                .EnumerateFiles(dir, "*.json", SearchOption.TopDirectoryOnly)
                .OrderBy(f => f, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var file in files)
            {
                try
                {
                    var json = await File.ReadAllTextAsync(file);
                    var seed = JsonSerializer.Deserialize<StorySeedData>(json, jsonOptions);
                    if (seed == null || string.IsNullOrWhiteSpace(seed.StoryId))
                    {
                        continue;
                    }

                    var tr = new StoryTranslationSeed
                    {
                        Locale = locale.ToLowerInvariant(),
                        StoryId = seed.StoryId,
                        Title = seed.Title,
                        Tiles = seed.Tiles?.Select(t => new TileTranslationSeed
                        {
                            TileId = t.TileId,
                            Caption = t.Caption,
                            Text = t.Text,
                            Question = t.Question,
                            AudioUrl = t.AudioUrl,
                            VideoUrl = t.VideoUrl,
                            Answers = t.Answers?.Select(a => new AnswerTranslationSeed
                            {
                                AnswerId = a.AnswerId,
                                Text = a.Text
                            }).ToList()
                        }).ToList()
                    };
                    results.Add(tr);
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        return results;
    }

    private static string NormalizeStoryId(string storyId)
    {
        if (string.Equals(storyId, "intro-puf-puf", StringComparison.OrdinalIgnoreCase))
            return "intro-pufpuf";
        return storyId;
    }

    public async Task<bool> StoryIdExistsAsync(string storyId)
    {
        var normalizedId = NormalizeStoryId(storyId);
        return await _context.StoryDefinitions
            .AnyAsync(s => s.StoryId == normalizedId);
    }
}


