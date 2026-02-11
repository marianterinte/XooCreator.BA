using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services.Content;

public interface IStoryTileUpdater
{
    Task UpdateTilesAsync(StoryCraft craft, List<EditableTileDto> tiles, string languageCode, IReadOnlySet<string>? allowedHeroIds = null, CancellationToken ct = default);
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

    public async Task UpdateTilesAsync(StoryCraft craft, List<EditableTileDto> tiles, string languageCode, IReadOnlySet<string>? allowedHeroIds = null, CancellationToken ct = default)
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
                    BranchId = string.IsNullOrWhiteSpace(tileDto.BranchId) ? null : tileDto.BranchId.Trim(),
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
                tile.BranchId = string.IsNullOrWhiteSpace(tileDto.BranchId) ? null : tileDto.BranchId.Trim();
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

            // Update dialog data when tile is dialog
            await UpdateDialogDataAsync(craft, tile, tileDto, languageCode, allowedHeroIds, ct);
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

    private async Task UpdateDialogDataAsync(
        StoryCraft craft,
        StoryCraftTile tile,
        EditableTileDto tileDto,
        string languageCode,
        IReadOnlySet<string>? allowedHeroIds,
        CancellationToken ct)
    {
        var existingDialogTile = tile.DialogTile;
        var isDialog = string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase);

        if (!isDialog)
        {
            if (existingDialogTile != null)
            {
                _context.StoryCraftDialogTiles.Remove(existingDialogTile);
            }

            return;
        }

        var dialogTile = existingDialogTile;
        if (dialogTile == null)
        {
            dialogTile = new StoryCraftDialogTile
            {
                Id = Guid.NewGuid(),
                StoryCraftId = craft.Id,
                StoryCraftTileId = tile.Id,
                RootNodeId = tileDto.DialogRootNodeId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.StoryCraftDialogTiles.Add(dialogTile);
            tile.DialogTile = dialogTile;
        }
        else
        {
            dialogTile.RootNodeId = tileDto.DialogRootNodeId;
            dialogTile.UpdatedAt = DateTime.UtcNow;
        }

        var dtoNodes = tileDto.DialogNodes ?? new List<EditableDialogNodeDto>();
        var existingNodes = await _context.StoryCraftDialogNodes
            .Include(n => n.Translations)
            .Include(n => n.OutgoingEdges)
                .ThenInclude(e => e.Translations)
            .Include(n => n.OutgoingEdges)
                .ThenInclude(e => e.Tokens)
            .Where(n => n.StoryCraftDialogTileId == dialogTile.Id)
            .ToListAsync(ct);

        var nodeDict = existingNodes.ToDictionary(n => n.NodeId, StringComparer.OrdinalIgnoreCase);
        var dtoNodeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        for (var i = 0; i < dtoNodes.Count; i++)
        {
            var nodeDto = dtoNodes[i];
            if (string.IsNullOrWhiteSpace(nodeDto.NodeId))
            {
                continue;
            }

            dtoNodeIds.Add(nodeDto.NodeId);
            if (!nodeDict.TryGetValue(nodeDto.NodeId, out var node))
            {
                node = new StoryCraftDialogNode
                {
                    Id = Guid.NewGuid(),
                    StoryCraftDialogTileId = dialogTile.Id,
                    NodeId = nodeDto.NodeId
                };
                _context.StoryCraftDialogNodes.Add(node);
                nodeDict[nodeDto.NodeId] = node;
            }

            node.SpeakerType = string.IsNullOrWhiteSpace(nodeDto.SpeakerType) ? "reader" : nodeDto.SpeakerType.Trim().ToLowerInvariant();
            node.SpeakerHeroId = string.IsNullOrWhiteSpace(nodeDto.SpeakerHeroId) ? null : nodeDto.SpeakerHeroId.Trim();
            node.SortOrder = i;

            if (string.Equals(node.SpeakerType, "hero", StringComparison.OrdinalIgnoreCase))
            {
                if (string.IsNullOrWhiteSpace(node.SpeakerHeroId))
                {
                    throw new InvalidOperationException($"Dialog node '{node.NodeId}' requires speakerHeroId when speakerType is hero.");
                }

                if (allowedHeroIds != null && allowedHeroIds.Count > 0 && !allowedHeroIds.Contains(node.SpeakerHeroId))
                {
                    throw new InvalidOperationException($"Dialog node '{node.NodeId}' references hero '{node.SpeakerHeroId}' not registered in dialog participants.");
                }
            }

            var nodeTranslation = node.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
            if (nodeTranslation == null)
            {
                nodeTranslation = new StoryCraftDialogNodeTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftDialogNodeId = node.Id,
                    LanguageCode = languageCode
                };
                _context.StoryCraftDialogNodeTranslations.Add(nodeTranslation);
                node.Translations.Add(nodeTranslation);
            }
            nodeTranslation.Text = nodeDto.Text ?? string.Empty;

            var options = nodeDto.Options ?? new List<EditableDialogOptionDto>();
            var edgeDict = node.OutgoingEdges.ToDictionary(e => e.EdgeId, StringComparer.OrdinalIgnoreCase);
            var optionIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            for (var optionIndex = 0; optionIndex < options.Count; optionIndex++)
            {
                var optionDto = options[optionIndex];
                var edgeId = string.IsNullOrWhiteSpace(optionDto.Id) ? $"opt_{optionIndex + 1}" : optionDto.Id.Trim();
                optionIds.Add(edgeId);

                if (!edgeDict.TryGetValue(edgeId, out var edge))
                {
                    edge = new StoryCraftDialogEdge
                    {
                        Id = Guid.NewGuid(),
                        StoryCraftDialogNodeId = node.Id,
                        EdgeId = edgeId
                    };
                    _context.StoryCraftDialogEdges.Add(edge);
                    node.OutgoingEdges.Add(edge);
                }

                edge.ToNodeId = optionDto.NextNodeId?.Trim() ?? string.Empty;
                edge.JumpToTileId = string.IsNullOrWhiteSpace(optionDto.JumpToTileId) ? null : optionDto.JumpToTileId.Trim();
                edge.SetBranchId = string.IsNullOrWhiteSpace(optionDto.SetBranchId) ? null : optionDto.SetBranchId.Trim();
                edge.OptionOrder = optionIndex;

                var edgeTranslation = edge.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
                if (edgeTranslation == null)
                {
                    edgeTranslation = new StoryCraftDialogEdgeTranslation
                    {
                        Id = Guid.NewGuid(),
                        StoryCraftDialogEdgeId = edge.Id,
                        LanguageCode = languageCode
                    };
                    _context.StoryCraftDialogEdgeTranslations.Add(edgeTranslation);
                    edge.Translations.Add(edgeTranslation);
                }

                edgeTranslation.OptionText = optionDto.Text ?? string.Empty;

                var optionTokens = optionDto.Tokens ?? new List<EditableTokenDto>();
                edge.Tokens ??= new List<StoryCraftDialogEdgeToken>();
                var existingTokens = edge.Tokens;
                var tokensByIndex = existingTokens
                    .OrderBy(t => t.Id)
                    .ToList();

                for (var tokenIndex = 0; tokenIndex < optionTokens.Count; tokenIndex++)
                {
                    var tokenDto = optionTokens[tokenIndex];
                    StoryCraftDialogEdgeToken token;
                    if (tokenIndex < tokensByIndex.Count)
                    {
                        token = tokensByIndex[tokenIndex];
                    }
                    else
                    {
                        token = new StoryCraftDialogEdgeToken
                        {
                            Id = Guid.NewGuid(),
                            StoryCraftDialogEdgeId = edge.Id
                        };
                        _context.StoryCraftDialogEdgeTokens.Add(token);
                        edge.Tokens.Add(token);
                        tokensByIndex.Add(token);
                    }

                    token.Type = tokenDto.Type?.Trim() ?? string.Empty;
                    token.Value = tokenDto.Value?.Trim() ?? string.Empty;
                    token.Quantity = Math.Max(1, tokenDto.Quantity);
                }

                for (var tokenIndex = tokensByIndex.Count - 1; tokenIndex >= optionTokens.Count; tokenIndex--)
                {
                    var tokenToRemove = tokensByIndex[tokenIndex];
                    edge.Tokens.Remove(tokenToRemove);
                    _context.StoryCraftDialogEdgeTokens.Remove(tokenToRemove);
                }
            }

            foreach (var edgeToRemove in node.OutgoingEdges.Where(e => !optionIds.Contains(e.EdgeId)).ToList())
            {
                _context.StoryCraftDialogEdges.Remove(edgeToRemove);
            }
        }

        foreach (var nodeToRemove in existingNodes.Where(n => !dtoNodeIds.Contains(n.NodeId)).ToList())
        {
            _context.StoryCraftDialogNodes.Remove(nodeToRemove);
        }
    }
}
