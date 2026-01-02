using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Infrastructure.Services.Images;

public interface IImageCompressionService
{
    /// <summary>
    /// Ensures s/m variants exist for an image in the PUBLISHED container.
    /// Rules:
    /// - If the image is not close to an allowed aspect ratio (tolerance), it will be skipped (no crop/pad).
    /// - If overwriteExisting is false, existing variants are preserved.
    /// </summary>
    Task<ImageCompressionResult> EnsureStorySizeVariantsAsync(
        string sourceBlobPath,
        string targetBasePath,
        string filename,
        bool overwriteExisting,
        CancellationToken ct);
}

public sealed record ImageCompressionResult
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public bool SkippedBecauseNotFourByFive { get; init; }
    public long OriginalSizeBytes { get; init; }
    public long SmallSizeBytes { get; init; }
    public long MediumSizeBytes { get; init; }
    public string? SmallPath { get; init; }
    public string? MediumPath { get; init; }
}


