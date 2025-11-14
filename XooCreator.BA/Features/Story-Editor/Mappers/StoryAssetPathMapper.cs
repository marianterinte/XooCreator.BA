using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Mappers;

/// <summary>
/// Mapper for constructing asset paths for story drafts and published stories.
/// Centralizes all path construction logic to avoid duplication and simplify maintenance.
/// </summary>
public static class StoryAssetPathMapper
{
    /// <summary>
    /// Represents an asset with its type and optional language tag.
    /// </summary>
    public record AssetInfo(string Filename, AssetType Type, string? Lang);

    /// <summary>
    /// Type of asset (Image, Audio, Video).
    /// </summary>
    public enum AssetType
    {
        Image,
        Audio,
        Video
    }

    /// <summary>
    /// Extracts all asset filenames from a StoryCraft.
    /// Returns only the filename (not full paths) as stored in the database.
    /// </summary>
    /// <param name="craft">The story craft to extract assets from</param>
    /// <param name="langTag">Language tag for audio assets (language-specific)</param>
    /// <returns>List of asset information with filename, type, and language</returns>
    public static List<AssetInfo> ExtractAssets(StoryCraft craft, string langTag)
    {
        var results = new List<AssetInfo>();

        // Cover image (language-agnostic)
        if (!string.IsNullOrWhiteSpace(craft.CoverImageUrl))
        {
            results.Add(new AssetInfo(craft.CoverImageUrl, AssetType.Image, null));
        }

        // Tile assets
        foreach (var tile in craft.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(tile.ImageUrl))
            {
                results.Add(new AssetInfo(tile.ImageUrl, AssetType.Image, null));
            }

            if (!string.IsNullOrWhiteSpace(tile.AudioUrl))
            {
                results.Add(new AssetInfo(tile.AudioUrl, AssetType.Audio, langTag));
            }

            if (!string.IsNullOrWhiteSpace(tile.VideoUrl))
            {
                results.Add(new AssetInfo(tile.VideoUrl, AssetType.Video, null));
            }
        }

        return results;
    }

    /// <summary>
    /// Builds the published blob storage path for an asset.
    /// Structure: {category}/tales-of-alchimalia/stories/{userEmail}/{storyId}/{filename}
    /// For audio: {category}/tales-of-alchimalia/stories/{userEmail}/{storyId}/{lang}/{filename}
    /// </summary>
    /// <param name="asset">Asset information</param>
    /// <param name="userEmail">User email (owner of the story)</param>
    /// <param name="storyId">Story identifier</param>
    /// <returns>Full blob path for published container</returns>
    public static string BuildPublishedPath(AssetInfo asset, string userEmail, string storyId)
    {
        var category = asset.Type switch
        {
            AssetType.Image => "images",
            AssetType.Audio => "audio",
            AssetType.Video => "video",
            _ => "images"
        };

        var basePath = $"{category}/tales-of-alchimalia/stories/{userEmail}/{storyId}";

        // Audio assets are language-specific in published structure
        if (asset.Type == AssetType.Audio && !string.IsNullOrWhiteSpace(asset.Lang))
        {
            basePath = $"{basePath}/{asset.Lang}";
        }

        return $"{basePath}/{asset.Filename}";
    }

    /// <summary>
    /// Builds the draft blob storage path for an asset.
    /// Structure: draft/u/{emailEscaped}/stories/{storyId}/{filename} (for images)
    /// Structure: draft/u/{emailEscaped}/stories/{storyId}/{lang}/{filename} (for audio/video)
    /// </summary>
    /// <param name="asset">Asset information</param>
    /// <param name="userEmail">User email (owner of the story)</param>
    /// <param name="storyId">Story identifier</param>
    /// <returns>Full blob path for draft container</returns>
    public static string BuildDraftPath(AssetInfo asset, string userEmail, string storyId)
    {
        var emailEsc = Uri.EscapeDataString(userEmail);
        var basePath = $"draft/u/{emailEsc}/stories/{storyId}";

        // Images are language-agnostic in draft
        if (asset.Type == AssetType.Image)
        {
            return $"{basePath}/{asset.Filename}";
        }

        // Audio and video are language-specific
        if (!string.IsNullOrWhiteSpace(asset.Lang))
        {
            return $"{basePath}/{asset.Lang}/{asset.Filename}";
        }

        // Fallback: without language (shouldn't happen for audio/video, but handle gracefully)
        return $"{basePath}/{asset.Filename}";
    }

    /// <summary>
    /// Sanitizes email for use in folder paths (keeps @, removes special chars).
    /// Used for published paths where we want readable folder names.
    /// </summary>
    /// <param name="email">Email address to sanitize</param>
    /// <returns>Sanitized email suitable for folder names</returns>
    public static string SanitizeEmailForFolder(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return "unknown";

        var trimmed = email.Trim().ToLowerInvariant();
        var chars = trimmed.Select(ch =>
            char.IsLetterOrDigit(ch) || ch == '.' || ch == '_' || ch == '-' || ch == '@' ? ch : '-'
        ).ToArray();

        var cleaned = new string(chars);
        while (cleaned.Contains("--"))
        {
            cleaned = cleaned.Replace("--", "-");
        }

        return cleaned.Trim('-');
    }
}

