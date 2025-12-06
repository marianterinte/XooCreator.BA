using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryExportService
{
    Task<ExportResult> ExportPublishedStoryAsync(StoryDefinition def, string locale, CancellationToken ct);
    Task<ExportResult> ExportDraftStoryAsync(StoryCraft craft, string locale, string ownerEmail, CancellationToken ct);
}

public record ExportResult
{
    public required byte[] ZipBytes { get; init; }
    public required string FileName { get; init; }
    public int MediaCount { get; init; }
    public int LanguageCount { get; init; }
    public long ZipSizeBytes { get; init; }
}
