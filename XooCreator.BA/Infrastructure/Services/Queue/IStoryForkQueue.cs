using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryForkQueue
{
    Task EnqueueAsync(StoryForkJob job, CancellationToken ct = default);
}

