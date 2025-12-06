using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.DTOs;

public record ExportResponse
{
    public Guid JobId { get; init; }
    public string Status { get; init; } = StoryExportJobStatus.Queued;
}
