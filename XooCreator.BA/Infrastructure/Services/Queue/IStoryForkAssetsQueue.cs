using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryForkAssetsQueue
{
    Task EnqueueAsync(StoryForkAssetJob job, CancellationToken ct = default);
}

