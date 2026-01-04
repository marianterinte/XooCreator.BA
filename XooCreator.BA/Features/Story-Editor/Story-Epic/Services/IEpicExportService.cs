using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IEpicExportService
{
    /// <summary>
    /// Build the complete export manifest for an epic, including all related data
    /// </summary>
    Task<EpicExportManifestDto> BuildManifestAsync(string epicId, EpicExportRequest options, bool isDraft, CancellationToken ct);

    /// <summary>
    /// Create a ZIP archive from the export manifest and related assets
    /// </summary>
    Task<Stream> CreateZipArchiveAsync(EpicExportManifestDto manifest, string epicId, bool isDraft, CancellationToken ct);

    /// <summary>
    /// Upload the export ZIP to blob storage and return the download URL
    /// </summary>
    Task<EpicExportUploadResult> UploadExportAsync(Stream zipStream, string epicId, string fileName, CancellationToken ct);
}

public record EpicExportUploadResult
{
    public required string BlobPath { get; init; }
    public required string DownloadUrl { get; init; }
    public required long SizeBytes { get; init; }
}