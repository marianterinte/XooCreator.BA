using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services;

public record DocumentExportResult(byte[] Bytes, string FileName)
{
    public long SizeBytes => Bytes?.LongLength ?? 0;
}

public interface IStoryDocumentExportService
{
    Task<DocumentExportResult> ExportAsync(StoryDocumentExportJob job, CancellationToken ct = default);
}


