using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryTranslationQueue
{
    Task EnqueueAsync(StoryTranslationJob job, CancellationToken ct = default);
}
