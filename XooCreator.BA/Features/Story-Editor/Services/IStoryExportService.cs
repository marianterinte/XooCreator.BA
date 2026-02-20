using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>Options for which media types to include in the export ZIP. When all are false, only JSON (story + dialogs) is included.</summary>
public record ExportOptions(bool IncludeVideo = true, bool IncludeAudio = true, bool IncludeImages = true);

public interface IStoryExportService
{
    Task<ExportResult> ExportPublishedStoryAsync(StoryDefinition def, string locale, ExportOptions options, CancellationToken ct);
    Task<ExportResult> ExportDraftStoryAsync(StoryCraft craft, string locale, string ownerEmail, ExportOptions options, CancellationToken ct);
}

public record ExportResult
{
    public required byte[] ZipBytes { get; init; }
    public required string FileName { get; init; }
    public int MediaCount { get; init; }
    public int LanguageCount { get; init; }
    public long ZipSizeBytes { get; init; }
}
