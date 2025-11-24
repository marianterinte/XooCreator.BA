using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services.Content;

public interface IStoryTileUpdater
{
    Task UpdateTilesAsync(StoryCraft craft, List<EditableTileDto> tiles, string languageCode, CancellationToken ct = default);
}

public class StoryTileUpdater : IStoryTileUpdater
{
    private readonly XooDbContext _context;
    private readonly IStoryAnswerUpdater _answerUpdater;

    public StoryTileUpdater(XooDbContext context, IStoryAnswerUpdater answerUpdater)
    {
        _context = context;
        _answerUpdater = answerUpdater;
    }

    public async Task UpdateTilesAsync(StoryCraft craft, List<EditableTileDto> tiles, string languageCode, CancellationToken ct = default)
    {
        var existingTiles = craft.Tiles.ToList();
        var tileDict = existingTiles.ToDictionary(t => t.TileId);
        
        // Process each tile from DTO
        for (int i = 0; i < tiles.Count; i++)
        {
            var tileDto = tiles[i];
            var tile = tileDict.GetValueOrDefault(tileDto.Id);
            
            if (tile == null)
            {
                // Create new tile
                tile = new StoryCraftTile
                {
                    Id = Guid.NewGuid(),
                    StoryCraftId = craft.Id,
                    TileId = tileDto.Id,
                    Type = tileDto.Type ?? "page",
                    SortOrder = i,
                    // Image is common for all languages
                    ImageUrl = ExtractFileName(tileDto.ImageUrl),
                    // Audio and Video are now language-specific (stored in translation)
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                };
                _context.StoryCraftTiles.Add(tile);
                craft.Tiles.Add(tile);
            }
            else
            {
                // Update existing tile
                tile.Type = tileDto.Type ?? tile.Type;
                tile.SortOrder = i;
                // Image is common for all languages
                tile.ImageUrl = ExtractFileName(tileDto.ImageUrl);
                // Audio and Video are now language-specific (stored in translation)
                tile.UpdatedAt = DateTime.UtcNow;
            }
            
            // Update or create tile translation
            var tileTranslation = tile.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
            if (tileTranslation == null)
            {
                tileTranslation = new StoryCraftTileTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftTileId = tile.Id,
                    LanguageCode = languageCode,
                    Caption = tileDto.Caption,
                    Text = tileDto.Text,
                    Question = tileDto.Question,
                    // Audio and Video are language-specific
                    AudioUrl = ExtractFileName(tileDto.AudioUrl),
                    VideoUrl = ExtractFileName(tileDto.VideoUrl)
                };
                _context.StoryCraftTileTranslations.Add(tileTranslation);
            }
            else
            {
                tileTranslation.Caption = tileDto.Caption;
                tileTranslation.Text = tileDto.Text;
                tileTranslation.Question = tileDto.Question;
                // Audio and Video are language-specific
                tileTranslation.AudioUrl = ExtractFileName(tileDto.AudioUrl);
                tileTranslation.VideoUrl = ExtractFileName(tileDto.VideoUrl);
            }
            
            // Update answers
            await _answerUpdater.UpdateAnswersAsync(tile, tileDto.Answers ?? new(), languageCode, ct);
        }
        
        // Remove tiles that are no longer in DTO
        var dtoTileIds = new HashSet<string>(tiles.Select(t => t.Id));
        var tilesToRemove = existingTiles.Where(t => !dtoTileIds.Contains(t.TileId)).ToList();
        foreach (var tileToRemove in tilesToRemove)
        {
            _context.StoryCraftTiles.Remove(tileToRemove);
        }
    }

    /// <summary>
    /// Extracts filename from a path. If input is already just a filename (no '/'), returns it as-is.
    /// If input contains '/', extracts the filename using Path.GetFileName().
    /// </summary>
    private static string? ExtractFileName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path)) return null;
        
        // If already just filename (no path separator), return as-is
        if (!path.Contains('/')) return path;
        
        // Extract filename from path
        return Path.GetFileName(path);
    }
}
