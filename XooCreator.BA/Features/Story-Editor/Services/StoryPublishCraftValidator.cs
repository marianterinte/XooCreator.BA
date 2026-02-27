using System.Collections.Frozen;
using System.Linq;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Pre-publish validation against DB constraints and uniqueness. Produces copyable diagnostics.
/// </summary>
public class StoryPublishCraftValidator : IStoryPublishCraftValidator
{
    // Published table limits (craft may allow same or different; we validate for publish)
    private const int AnswerTokenTypeMaxLength = 64;
    private const int AnswerTokenValueMaxLength = 128;
    private const int DialogEdgeTokenTypeMaxLength = 100;
    private const int DialogEdgeTokenValueMaxLength = 200;
    private const int BranchIdMaxLength = 100;
    private const int LanguageCodeMaxLength = 10;

    public PublishCraftValidationResult Validate(StoryCraft craft)
    {
        var result = PublishCraftValidationResult.Create();

        if (craft.Tiles == null) return result;

        var tileIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tile in craft.Tiles)
        {
            if (string.IsNullOrEmpty(tile.TileId)) continue;
            if (!tileIds.Add(tile.TileId))
                result.Add("Tile", tile.TileId, "UniqueViolation", "Duplicate TileId in story.");
        }

        var allTileIds = craft.Tiles
            .Where(t => !string.IsNullOrEmpty(t.TileId))
            .Select(t => t.TileId!)
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        foreach (var tile in craft.Tiles)
        {
            ValidateTile(tile, craft, allTileIds, result);
        }

        return result;
    }

    private void ValidateTile(
        StoryCraftTile tile,
        StoryCraft craft,
        FrozenSet<string> allTileIds,
        PublishCraftValidationResult result)
    {
        var tileKey = tile.TileId ?? "(empty)";

        if (tile.BranchId != null && tile.BranchId.Length > BranchIdMaxLength)
            result.Add("Tile", tileKey, "MaxLength", $"BranchId length {tile.BranchId.Length} > {BranchIdMaxLength}.");

        foreach (var tr in tile.Translations ?? new List<StoryCraftTileTranslation>())
        {
            if (tr.LanguageCode != null && tr.LanguageCode.Length > LanguageCodeMaxLength)
                result.Add("TileTranslation", $"{tileKey}|{tr.LanguageCode}", "MaxLength", $"LanguageCode length > {LanguageCodeMaxLength}.");
        }

        var answerIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var answer in tile.Answers ?? new List<StoryCraftAnswer>())
        {
            if (!string.IsNullOrEmpty(answer.AnswerId) && !answerIds.Add(answer.AnswerId))
                result.Add("Answer", $"{tileKey}|{answer.AnswerId}", "UniqueViolation", "Duplicate AnswerId in tile.");
            ValidateAnswerTokens(tileKey, answer, result);
        }

        if (string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase) && tile.DialogTile != null)
            ValidateDialogTile(tile.TileId, tile.DialogTile, craft, allTileIds, result);
    }

    private void ValidateAnswerTokens(string tileKey, StoryCraftAnswer answer, PublishCraftValidationResult result)
    {
        foreach (var token in answer.Tokens ?? new List<StoryCraftAnswerToken>())
        {
            var type = token.Type;
            var value = token.Value;
            if (string.IsNullOrEmpty(type))
                result.Add("AnswerToken", $"{tileKey}|{answer.AnswerId}", "Required", "Token Type is null or empty.");
            else if (type.Length > AnswerTokenTypeMaxLength)
                result.Add("AnswerToken", $"{tileKey}|{answer.AnswerId}|Type", "MaxLength", $"Type length {type.Length} > {AnswerTokenTypeMaxLength}.");
            if (value == null)
                result.Add("AnswerToken", $"{tileKey}|{answer.AnswerId}", "Required", "Token Value is null.");
            else if (value.Length > AnswerTokenValueMaxLength)
                result.Add("AnswerToken", $"{tileKey}|{answer.AnswerId}|Value", "MaxLength", $"Value length {value.Length} > {AnswerTokenValueMaxLength}.");
        }
    }

    private void ValidateDialogTile(
        string tileId,
        StoryCraftDialogTile dialogTile,
        StoryCraft craft,
        FrozenSet<string> allTileIds,
        PublishCraftValidationResult result)
    {
        var participantHeroIds = (craft.DialogParticipants ?? new List<StoryCraftDialogParticipant>())
            .Select(p => p.HeroId?.Trim())
            .Where(h => !string.IsNullOrEmpty(h))
            .ToFrozenSet(StringComparer.OrdinalIgnoreCase);

        var nodeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var nodes = dialogTile.Nodes ?? new List<StoryCraftDialogNode>();

        foreach (var node in nodes)
        {
            if (string.IsNullOrEmpty(node.NodeId)) continue;
            if (!nodeIds.Add(node.NodeId))
                result.Add("DialogNode", $"{tileId}|{node.NodeId}", "UniqueViolation", "Duplicate NodeId in dialog tile.");

            if (string.Equals(node.SpeakerType, "hero", StringComparison.OrdinalIgnoreCase)
                && !string.IsNullOrWhiteSpace(node.SpeakerHeroId)
                && participantHeroIds.Count > 0
                && !participantHeroIds.Contains(node.SpeakerHeroId.Trim()))
                result.Add("DialogNode", $"{tileId}|{node.NodeId}", "InvalidReference", $"SpeakerHeroId '{node.SpeakerHeroId}' not in DialogParticipants.");

            foreach (var tr in node.Translations ?? new List<StoryCraftDialogNodeTranslation>())
            {
                if (tr.LanguageCode != null && tr.LanguageCode.Length > LanguageCodeMaxLength)
                    result.Add("DialogNodeTranslation", $"{node.NodeId}|{tr.LanguageCode}", "MaxLength", $"LanguageCode length > {LanguageCodeMaxLength}.");
            }

            var edgeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var edge in node.OutgoingEdges ?? new List<StoryCraftDialogEdge>())
            {
                if (!string.IsNullOrEmpty(edge.EdgeId) && !edgeIds.Add(edge.EdgeId))
                    result.Add("DialogEdge", $"{tileId}|{node.NodeId}|{edge.EdgeId}", "UniqueViolation", "Duplicate EdgeId in node.");

                if (edge.JumpToTileId != null && edge.JumpToTileId.Length > BranchIdMaxLength)
                    result.Add("DialogEdge", $"{tileId}|{edge.EdgeId}", "MaxLength", $"JumpToTileId length > {BranchIdMaxLength}.");
                if (edge.SetBranchId != null && edge.SetBranchId.Length > BranchIdMaxLength)
                    result.Add("DialogEdge", $"{tileId}|{edge.EdgeId}", "MaxLength", $"SetBranchId length > {BranchIdMaxLength}.");
                if (edge.HideIfBranchSet != null && edge.HideIfBranchSet.Length > BranchIdMaxLength)
                    result.Add("DialogEdge", $"{tileId}|{edge.EdgeId}", "MaxLength", $"HideIfBranchSet length > {BranchIdMaxLength}.");

                if (!string.IsNullOrWhiteSpace(edge.JumpToTileId) && allTileIds.Count > 0 && !allTileIds.Contains(edge.JumpToTileId.Trim()))
                    result.Add("DialogEdge", $"{tileId}|{edge.EdgeId}", "InvalidReference", $"JumpToTileId '{edge.JumpToTileId}' does not match any TileId in story.");

                foreach (var token in edge.Tokens ?? new List<StoryCraftDialogEdgeToken>())
                {
                    var type = token.Type ?? string.Empty;
                    var value = token.Value ?? string.Empty;
                    if (type.Length > DialogEdgeTokenTypeMaxLength)
                        result.Add("DialogEdgeToken", $"{tileId}|{edge.EdgeId}|Type", "MaxLength", $"Type length {type.Length} > {DialogEdgeTokenTypeMaxLength}.");
                    if (value.Length > DialogEdgeTokenValueMaxLength)
                        result.Add("DialogEdgeToken", $"{tileId}|{edge.EdgeId}|Value", "MaxLength", $"Value length {value.Length} > {DialogEdgeTokenValueMaxLength}.");
                }

                foreach (var tr in edge.Translations ?? new List<StoryCraftDialogEdgeTranslation>())
                {
                    if (tr.LanguageCode != null && tr.LanguageCode.Length > LanguageCodeMaxLength)
                        result.Add("DialogEdgeTranslation", $"{edge.EdgeId}|{tr.LanguageCode}", "MaxLength", $"LanguageCode length > {LanguageCodeMaxLength}.");
                }
            }
        }
    }
}
