using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IEpicVersionQueue
{
    Task EnqueueAsync(EpicVersionJob job, CancellationToken ct = default);
}

