using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryImageImportQueue
{
    Task EnqueueAsync(StoryImageImportJob job, CancellationToken ct = default);
}
