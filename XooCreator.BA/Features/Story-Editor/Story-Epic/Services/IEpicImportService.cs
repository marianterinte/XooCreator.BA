using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IEpicImportService
{
    /// <summary>
    /// Preview an epic import without actually importing anything
    /// </summary>
    Task<EpicImportPreviewResponse> PreviewImportAsync(Stream zipStream, Guid ownerUserId, CancellationToken ct);

    /// <summary>
    /// Validate the ZIP structure and manifest
    /// </summary>
    Task<EpicImportValidationResult> ValidateZipAsync(Stream zipStream, CancellationToken ct);

    /// <summary>
    /// Extract the manifest from the ZIP file
    /// </summary>
    Task<EpicExportManifestDto> ExtractManifestAsync(Stream zipStream, CancellationToken ct);

    /// <summary>
    /// Check for conflicts with existing data
    /// </summary>
    Task<ConflictCheckResult> CheckForConflictsAsync(EpicExportManifestDto manifest, Guid ownerUserId, CancellationToken ct);

    /// <summary>
    /// Generate new IDs for all entities if needed (for conflict resolution)
    /// </summary>
    Task<EpicIdMappings> GenerateIdMappingsAsync(EpicExportManifestDto manifest, string? idPrefix, CancellationToken ct);

    /// <summary>
    /// Import the complete epic from the manifest
    /// </summary>
    Task<EpicImportResult> ImportEpicAsync(
        Stream zipStream,
        EpicExportManifestDto manifest,
        EpicImportRequest options,
        EpicIdMappings? idMappings,
        Guid ownerUserId,
        string locale,
        CancellationToken ct);
}

public record EpicImportResult
{
    public required string ImportedEpicId { get; init; }
    public required EpicIdMappings IdMappings { get; init; }
    public List<string> Warnings { get; init; } = new();
    public List<string> Errors { get; init; } = new();
}