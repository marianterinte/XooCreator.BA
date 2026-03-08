using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryAIGenerateQueue
{
    Task EnqueueAsync(StoryAIGenerateJob job, CancellationToken ct = default);
}
