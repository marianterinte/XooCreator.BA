using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IHeroPublishQueue
{
    Task EnqueueAsync(HeroPublishJob job, CancellationToken ct = default);
}
