using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryPublishingService
{
    // Returns the new global version after publish
    Task<int> UpsertFromCraftAsync(StoryCraft craft, string ownerEmail, string langTag, CancellationToken ct);
}

public class StoryPublishingService : IStoryPublishingService
{
    private readonly XooDbContext _db;
    private readonly ILogger<StoryPublishingService> _logger;

    public StoryPublishingService(XooDbContext db, ILogger<StoryPublishingService> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<int> UpsertFromCraftAsync(StoryCraft craft, string ownerEmail, string langTag, CancellationToken ct)
    {
        if (craft == null) throw new ArgumentNullException(nameof(craft));
        var storyId = craft.StoryId;

        var def = await _db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);

        var isNew = def == null;
        if (isNew)
        {
            def = new StoryDefinition
            {
                StoryId = storyId,
                Title = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Title ?? storyId,
                StoryType = craft.StoryType,
                Status = StoryStatus.Published,
                IsActive = true,
                SortOrder = 0,
                Version = 1
            };
            _db.StoryDefinitions.Add(def);
        }

        // Update header
        def.Title = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Title
            ?? def.Title;
        def.Summary = craft.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Summary ?? def.Summary;
        def.StoryType = craft.StoryType;
        def.Status = StoryStatus.Published;
        def.UpdatedAt = DateTime.UtcNow;
        // Version bump for existing definitions
        if (!isNew)
        {
            def.Version = def.Version <= 0 ? 1 : def.Version + 1;
        }

        // Map cover to published structure (language-agnostic)
        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            var fileName = craft.CoverImageUrl;
            def.CoverImageUrl = $"images/tales-of-alchimalia/stories/{SanitizeEmailForFolder(ownerEmail)}/{storyId}/{fileName}";
        }

        // Clear existing tiles/translations to avoid stale data
        if (def.Id != Guid.Empty)
        {
            var existingTiles = await _db.StoryTiles.Where(t => t.StoryDefinitionId == def.Id).ToListAsync(ct);
            if (existingTiles.Count > 0)
            {
                _db.StoryTiles.RemoveRange(existingTiles);
            }

            var existingDefTr = await _db.StoryDefinitionTranslations.Where(t => t.StoryDefinitionId == def.Id).ToListAsync(ct);
            if (existingDefTr.Count > 0)
            {
                _db.StoryDefinitionTranslations.RemoveRange(existingDefTr);
            }
        }

        // Add definition translations from craft
        foreach (var tr in craft.Translations)
        {
            _db.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
            {
                StoryDefinition = def,
                LanguageCode = tr.LanguageCode.ToLowerInvariant(),
                Title = tr.Title ?? string.Empty
            });
        }

        // Tiles
        var tilesBySort = craft.Tiles.OrderBy(t => t.SortOrder).ToList();
        _logger.LogInformation("Publishing story: storyId={StoryId} tilesCount={Count}", storyId, tilesBySort.Count);
        
        foreach (var ctile in tilesBySort)
        {
            _logger.LogInformation("Processing tile: tileId={TileId} imageUrl={ImageUrl} audioUrl={AudioUrl} videoUrl={VideoUrl}", 
                ctile.TileId, ctile.ImageUrl ?? "(null)", ctile.AudioUrl ?? "(null)", ctile.VideoUrl ?? "(null)");
            
            var tile = new StoryTile
            {
                StoryDefinition = def,
                TileId = ctile.TileId,
                Type = ctile.Type,
                SortOrder = ctile.SortOrder,
                Caption = null,
                Text = null,
                Question = null,
                ImageUrl = ctile.ImageUrl,
                AudioUrl = ctile.AudioUrl,
                VideoUrl = ctile.VideoUrl
            };

            // Map non-translatable asset paths to published structure
            if (!string.IsNullOrWhiteSpace(ctile.ImageUrl))
            {
                var fileName = ComputePublishedFileName(ctile.ImageUrl, "tiles");
                tile.ImageUrl = $"images/tales-of-alchimalia/stories/{SanitizeEmailForFolder(ownerEmail)}/{storyId}/{fileName}";
                _logger.LogInformation("Mapped ImageUrl: {ImageUrl} -> {PublishedUrl}", ctile.ImageUrl, tile.ImageUrl);
            }
            if (!string.IsNullOrWhiteSpace(ctile.VideoUrl))
            {
                var fileName = ComputePublishedFileName(ctile.VideoUrl, "video");
                tile.VideoUrl = $"video/tales-of-alchimalia/stories/{SanitizeEmailForFolder(ownerEmail)}/{storyId}/{fileName}";
                _logger.LogInformation("Mapped VideoUrl: {VideoUrl} -> {PublishedUrl}", ctile.VideoUrl, tile.VideoUrl);
            }
            if (!string.IsNullOrWhiteSpace(ctile.AudioUrl))
            {
                var fileName = ComputePublishedFileName(ctile.AudioUrl, "audio");
                tile.AudioUrl = $"audio/tales-of-alchimalia/stories/{SanitizeEmailForFolder(ownerEmail)}/{storyId}/{langTag}/{fileName}";
                _logger.LogInformation("Mapped AudioUrl: {AudioUrl} -> {PublishedUrl}", ctile.AudioUrl, tile.AudioUrl);
            }

            _db.StoryTiles.Add(tile);

            // Translations
            foreach (var tr in ctile.Translations)
            {
                _db.StoryTileTranslations.Add(new StoryTileTranslation
                {
                    StoryTile = tile,
                    LanguageCode = tr.LanguageCode.ToLowerInvariant(),
                    Caption = tr.Caption,
                    Text = tr.Text,
                    Question = tr.Question
                });
            }

            // Answers
            var answers = ctile.Answers.OrderBy(a => a.SortOrder).ToList();
            foreach (var ca in answers)
            {
                var ans = new StoryAnswer
                {
                    StoryTile = tile,
                    AnswerId = ca.AnswerId,
                    Text = ca.Translations.FirstOrDefault(t => t.LanguageCode == langTag)?.Text ?? string.Empty,
                    SortOrder = ca.SortOrder
                };
                _db.StoryAnswers.Add(ans);

                foreach (var tok in ca.Tokens)
                {
                    _db.StoryAnswerTokens.Add(new StoryAnswerToken
                    {
                        StoryAnswer = ans,
                        Type = tok.Type,
                        Value = tok.Value,
                        Quantity = tok.Quantity
                    });
                }

                foreach (var atr in ca.Translations)
                {
                    _db.StoryAnswerTranslations.Add(new StoryAnswerTranslation
                    {
                        StoryAnswer = ans,
                        LanguageCode = atr.LanguageCode.ToLowerInvariant(),
                        Text = atr.Text ?? string.Empty
                    });
                }
            }
        }

        await _db.SaveChangesAsync(ct);
        return def.Version;
    }

    private static string ComputePublishedFileName(string relPath, string category)
    {
        var parts = relPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0) return Path.GetFileName(relPath);
        
        return category.ToLowerInvariant() switch
        {
            "cover" => ExtractCoverFileName(parts),
            "tiles" => ExtractImageFileName(parts),
            "audio" => ExtractAudioFileName(parts),
            "video" => ExtractVideoFileName(parts),
            _ => parts[^1] // Default to last segment
        };
    }

    private static string ExtractCoverFileName(string[] parts)
    {
        // cover/0.cover.png -> cover.png
        if (parts.Length == 0) return "cover";
        var ext = Path.GetExtension(parts[^1]);
        return string.IsNullOrWhiteSpace(ext) ? "cover" : $"cover{ext}";
    }

    private static string ExtractImageFileName(string[] parts)
    {
        // tiles/p1.webp -> p1.webp (new structure: filename directly)
        // tiles/p1/bg.webp -> bg.webp (legacy structure: filename in parts[2])
        if (parts.Length < 2) return Path.GetFileName(string.Join("/", parts));
        
        // Check if parts[1] is a filename (has extension) or a folder (no extension)
        if (Path.HasExtension(parts[1]))
        {
            // New structure: filename is directly in parts[1]
            return parts[1];
        }
        
        // Legacy structure: parts[1] is tileId folder, parts[2] is the actual filename
        if (parts.Length >= 3)
        {
            return parts[2];
        }
        
        // Fallback
        return parts[^1];
    }

    private static string ExtractAudioFileName(string[] parts)
    {
        // audio/4.cave.wav -> 4.cave.wav (new structure: filename directly)
        // audio/p1/4.cave.wav -> 4.cave.wav (legacy structure: filename in parts[2])
        // 4.cave.wav -> 4.cave.wav (just filename, no prefix)
        
        // If only one part, it's just the filename
        if (parts.Length == 1)
        {
            return parts[0];
        }
        
        // If parts[0] is "audio", skip it and process the rest
        if (parts.Length >= 2 && parts[0].Equals("audio", StringComparison.OrdinalIgnoreCase))
        {
            // Check if parts[1] is a filename (has extension) or a folder (no extension)
            if (Path.HasExtension(parts[1]))
            {
                // New structure: filename is directly in parts[1]
                return parts[1];
            }
            
            // Legacy structure: parts[1] is tileId folder, parts[2] is the actual filename
            if (parts.Length >= 3)
            {
                return parts[2];
            }
        }
        
        // If no "audio" prefix, find the last part that looks like a filename
        for (int i = parts.Length - 1; i >= 0; i--)
        {
            if (Path.HasExtension(parts[i]))
            {
                return parts[i];
            }
        }
        
        // Fallback: return last part
        return parts[^1];
    }

    private static string ExtractVideoFileName(string[] parts)
    {
        // video/p1.mp4 -> p1.mp4 (new structure: filename directly)
        // video/p1/intro.mp4 -> intro.mp4 (legacy structure: filename in parts[2])
        if (parts.Length < 2) return Path.GetFileName(string.Join("/", parts));
        
        // Check if parts[1] is a filename (has extension) or a folder (no extension)
        if (Path.HasExtension(parts[1]))
        {
            // New structure: filename is directly in parts[1]
            return parts[1];
        }
        
        // Legacy structure: parts[1] is tileId folder, parts[2] is the actual filename
        if (parts.Length >= 3)
        {
            return parts[2];
        }
        
        // Fallback
        return parts[^1];
    }

    private static string SanitizeEmailForFolder(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return "unknown";
        var trimmed = email.Trim().ToLowerInvariant();
        var chars = trimmed.Select(ch =>
            char.IsLetterOrDigit(ch) || ch == '.' || ch == '_' || ch == '-' || ch == '@' ? ch : '-'
        ).ToArray();
        var cleaned = new string(chars);
        while (cleaned.Contains("--")) cleaned = cleaned.Replace("--", "-");
        return cleaned.Trim('-');
    }
}


