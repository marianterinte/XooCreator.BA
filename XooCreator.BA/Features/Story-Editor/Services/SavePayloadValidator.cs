using XooCreator.BA.Features.StoryEditor.Services.Content;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Lightweight validation of EditableStoryDto at save boundary to block payloads that would fail at publish.
/// Does not reject legacy/recoverable data; only clear constraint violations.
/// </summary>
public static class SavePayloadValidator
{
    private const int AnswerTokenTypeMax = 64;
    private const int AnswerTokenValueMax = 128;
    private const int DialogEdgeTokenTypeMax = 100;
    private const int DialogEdgeTokenValueMax = 200;

    /// <summary>
    /// Validates the DTO. Returns empty list if valid; otherwise one or more error messages (copyable).
    /// </summary>
    public static IReadOnlyList<string> Validate(EditableStoryDto dto)
    {
        var errors = new List<string>();
        if (dto.Tiles == null) return errors;

        var tileIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var tile in dto.Tiles)
        {
            var id = tile?.Id?.Trim();
            if (string.IsNullOrEmpty(id)) continue;
            if (!tileIds.Add(id))
                errors.Add($"Duplicate tile ID: '{id}'.");
        }

        foreach (var tile in dto.Tiles)
        {
            if (tile == null) continue;
            ValidateTile(tile, errors);
        }

        return errors;
    }

    private static void ValidateTile(EditableTileDto tile, List<string> errors)
    {
        var tileId = tile.Id ?? "(empty)";

        foreach (var answer in tile.Answers ?? new List<EditableAnswerDto>())
        {
            foreach (var token in answer.Tokens ?? new List<EditableTokenDto>())
            {
                var type = token.Type?.Trim() ?? string.Empty;
                var value = token.Value?.Trim() ?? string.Empty;
                if (type.Length > AnswerTokenTypeMax)
                    errors.Add($"Tile '{tileId}' answer '{answer.Id}': token Type length {type.Length} exceeds {AnswerTokenTypeMax}.");
                if (value.Length > AnswerTokenValueMax)
                    errors.Add($"Tile '{tileId}' answer '{answer.Id}': token Value length {value.Length} exceeds {AnswerTokenValueMax}.");
            }
        }

        var answerIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var answer in tile.Answers ?? new List<EditableAnswerDto>())
        {
            var aid = answer.Id?.Trim();
            if (string.IsNullOrEmpty(aid)) continue;
            if (!answerIds.Add(aid))
                errors.Add($"Tile '{tileId}': duplicate answer ID '{aid}'.");
        }

        if (!string.Equals(tile.Type, "dialog", StringComparison.OrdinalIgnoreCase))
            return;

        var nodeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var node in tile.DialogNodes ?? new List<EditableDialogNodeDto>())
        {
            var nid = node.NodeId?.Trim();
            if (string.IsNullOrEmpty(nid)) continue;
            if (!nodeIds.Add(nid))
                errors.Add($"Tile '{tileId}': duplicate dialog node ID '{nid}'.");

            var edgeIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            foreach (var option in node.Options ?? new List<EditableDialogOptionDto>())
            {
                var eid = option.Id?.Trim();
                if (!string.IsNullOrEmpty(eid) && !edgeIds.Add(eid))
                    errors.Add($"Tile '{tileId}' node '{nid}': duplicate option ID '{eid}'.");

                foreach (var token in option.Tokens ?? new List<EditableTokenDto>())
                {
                    var type = token.Type?.Trim() ?? string.Empty;
                    var value = token.Value?.Trim() ?? string.Empty;
                    if (type.Length > DialogEdgeTokenTypeMax)
                        errors.Add($"Tile '{tileId}' dialog option: token Type length {type.Length} exceeds {DialogEdgeTokenTypeMax}.");
                    if (value.Length > DialogEdgeTokenValueMax)
                        errors.Add($"Tile '{tileId}' dialog option: token Value length {value.Length} exceeds {DialogEdgeTokenValueMax}.");
                }
            }
        }
    }
}
