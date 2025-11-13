using Microsoft.EntityFrameworkCore;
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

    public StoryPublishingService(XooDbContext db)
    {
        _db = db;
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
            var fileName = ComputePublishedFileName(craft.CoverImageUrl);
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
        foreach (var ctile in tilesBySort)
        {
            var tile = new StoryTile
            {
                StoryDefinition = def,
                TileId = ctile.TileId,
                Type = ctile.Type,
                SortOrder = ctile.SortOrder,
                Caption = null,
                Text = null,
                Question = null
            };

            // Map non-translatable asset paths to published structure
            if (!string.IsNullOrWhiteSpace(ctile.ImageUrl))
            {
                var fileName = ComputePublishedFileName(ctile.ImageUrl);
                tile.ImageUrl = $"images/tales-of-alchimalia/stories/{SanitizeEmailForFolder(ownerEmail)}/{storyId}/{fileName}";
            }
            if (!string.IsNullOrWhiteSpace(ctile.VideoUrl))
            {
                var fileName = ComputePublishedFileName(ctile.VideoUrl);
                tile.VideoUrl = $"video/tales-of-alchimalia/stories/{SanitizeEmailForFolder(ownerEmail)}/{storyId}/{fileName}";
            }
            if (!string.IsNullOrWhiteSpace(ctile.AudioUrl))
            {
                var fileName = ComputePublishedFileName(ctile.AudioUrl);
                tile.AudioUrl = $"audio/tales-of-alchimalia/stories/{SanitizeEmailForFolder(ownerEmail)}/{storyId}/{langTag}/{fileName}";
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

    private static string ComputePublishedFileName(string relPath)
    {
        // Examples of incoming relPath (from craft):
        // - cover/0.cover.png            -> cover.png
        // - tiles/p1/bg.webp             -> p1.webp (legacy structure with tileId folder)
        // - tiles/p1.webp                -> p1.webp (new structure, filename already contains tileId)
        // - audio/4.cave.wav             -> 4.cave.wav (filename is in parts[1], use as-is)
        // - audio/p3/intro.m4a           -> p3.m4a (legacy structure with tileId folder)
        var parts = relPath.Split('/', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0) return Path.GetFileName(relPath);
        
        if (parts[0].Equals("cover", StringComparison.OrdinalIgnoreCase))
        {
            var ext = Path.GetExtension(parts[^1]);
            return string.IsNullOrWhiteSpace(ext) ? "cover" : $"cover{ext}";
        }
        
        if (parts.Length >= 2 && (parts[0].Equals("tiles", StringComparison.OrdinalIgnoreCase)
            || parts[0].Equals("audio", StringComparison.OrdinalIgnoreCase)
            || parts[0].Equals("video", StringComparison.OrdinalIgnoreCase)))
        {
            // For audio/tiles/video, parts[1] can be either:
            // 1. A filename directly (e.g., "4.cave.wav", "p1.webp") - use as-is
            // 2. A tileId folder (e.g., "p1") - then parts[2] is the filename
            // Check if parts[1] looks like a filename (has extension) or a folder (no extension)
            var hasExtension = Path.HasExtension(parts[1]);
            
            if (hasExtension)
            {
                // parts[1] is already the filename (e.g., "4.cave.wav")
                return parts[1];
            }
            else
            {
                // parts[1] is a tileId folder, parts[2] is the filename (legacy structure)
                // Extract extension from filename and combine with tileId
                if (parts.Length >= 3)
                {
                    var ext = Path.GetExtension(parts[2]);
                    var tileId = parts[1];
                    return string.IsNullOrWhiteSpace(ext) ? tileId : $"{tileId}{ext}";
                }
                else
                {
                    // Fallback: use parts[1] as-is
                    return parts[1];
                }
            }
        }
        
        // Default to last segment
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


