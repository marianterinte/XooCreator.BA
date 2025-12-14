using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IEpicPublishQueue
{
    Task EnqueueAsync(EpicPublishJob job, CancellationToken ct = default);
}

