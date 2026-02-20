namespace XooCreator.BA.Features.StoryEditor.Extensions;

/// <summary>
/// Common helpers for asset paths. Use a single place so draft/published/import/export logic stays consistent.
/// </summary>
public static class AssetPathExtensions
{
    /// <summary>
    /// Returns only the filename part (last segment) of a path.
    /// Used so draft lookup, publish, import and export all use the same format (filename-only).
    /// </summary>
    /// <param name="path">Full path or filename (e.g. "media/images/tiles/1.png" or "1.png")</param>
    /// <returns>Null if input is null; empty string if whitespace; otherwise the last segment after / or \.</returns>
    public static string? GetFilenameOnly(this string? path)
    {
        if (path == null) return null;
        if (string.IsNullOrWhiteSpace(path)) return string.Empty;
        var normalized = path.Trim().Replace('\\', '/');
        var lastSlash = normalized.LastIndexOf('/');
        return lastSlash >= 0 ? normalized.Substring(lastSlash + 1) : normalized;
    }

    /// <summary>
    /// For export/import: normalise asset URL to filename-only, preserving null.
    /// </summary>
    public static string? ToExportAssetUrl(this string? url) =>
        string.IsNullOrWhiteSpace(url) ? url : url.GetFilenameOnly();
}
