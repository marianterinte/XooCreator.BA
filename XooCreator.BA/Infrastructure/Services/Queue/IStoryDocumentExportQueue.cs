using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryDocumentExportQueue
{
    Task EnqueueAsync(StoryDocumentExportJob job, CancellationToken ct = default);
}

