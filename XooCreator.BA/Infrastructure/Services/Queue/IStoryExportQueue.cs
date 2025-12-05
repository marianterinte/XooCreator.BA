using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryExportQueue
{
    Task EnqueueAsync(StoryExportJob job, CancellationToken ct = default);
}
