using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryAudioExportQueue
{
    Task EnqueueAsync(StoryAudioExportJob job, CancellationToken ct = default);
}
