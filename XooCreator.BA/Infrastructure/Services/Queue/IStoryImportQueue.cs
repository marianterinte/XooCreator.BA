using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryImportQueue
{
    Task EnqueueAsync(StoryImportJob job, CancellationToken ct = default);
}

