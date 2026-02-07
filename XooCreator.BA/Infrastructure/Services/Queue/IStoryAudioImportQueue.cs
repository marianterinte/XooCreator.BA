using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryAudioImportQueue
{
    Task EnqueueAsync(StoryAudioImportJob job, CancellationToken ct = default);
}
