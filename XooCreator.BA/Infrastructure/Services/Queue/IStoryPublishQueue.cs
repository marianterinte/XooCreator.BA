using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryPublishQueue
{
    Task EnqueueAsync(StoryPublishJob job, CancellationToken ct = default);
}
