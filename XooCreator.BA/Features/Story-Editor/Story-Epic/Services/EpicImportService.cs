using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

/// <summary>
/// Epic Import Service - implementation pending
/// TODO: Implement all methods for epic import functionality
/// </summary>
public class EpicImportService : IEpicImportService
{
    public Task<EpicImportPreviewResponse> PreviewImportAsync(Stream zipStream, Guid ownerUserId, CancellationToken ct)
    {
        throw new NotImplementedException("EpicImportService.PreviewImportAsync is not yet implemented");
    }

    public Task<EpicImportValidationResult> ValidateZipAsync(Stream zipStream, CancellationToken ct)
    {
        throw new NotImplementedException("EpicImportService.ValidateZipAsync is not yet implemented");
    }

    public Task<EpicExportManifestDto> ExtractManifestAsync(Stream zipStream, CancellationToken ct)
    {
        throw new NotImplementedException("EpicImportService.ExtractManifestAsync is not yet implemented");
    }

    public Task<ConflictCheckResult> CheckForConflictsAsync(EpicExportManifestDto manifest, Guid ownerUserId, CancellationToken ct)
    {
        throw new NotImplementedException("EpicImportService.CheckForConflictsAsync is not yet implemented");
    }

    public Task<EpicIdMappings> GenerateIdMappingsAsync(EpicExportManifestDto manifest, string? idPrefix, CancellationToken ct)
    {
        throw new NotImplementedException("EpicImportService.GenerateIdMappingsAsync is not yet implemented");
    }

    public Task<EpicImportResult> ImportEpicAsync(
        Stream zipStream,
        EpicExportManifestDto manifest,
        EpicImportRequest options,
        EpicIdMappings? idMappings,
        Guid ownerUserId,
        string locale,
        CancellationToken ct)
    {
        throw new NotImplementedException("EpicImportService.ImportEpicAsync is not yet implemented");
    }
}

