using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IEpicAggregatesQueue
{
    Task EnqueueHeroVersionAsync(HeroVersionJob job, CancellationToken ct = default);
    Task EnqueueRegionVersionAsync(RegionVersionJob job, CancellationToken ct = default);
}

