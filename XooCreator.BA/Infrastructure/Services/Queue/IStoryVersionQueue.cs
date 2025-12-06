using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryVersionQueue
{
    Task EnqueueAsync(StoryVersionJob job, CancellationToken ct = default);
}

