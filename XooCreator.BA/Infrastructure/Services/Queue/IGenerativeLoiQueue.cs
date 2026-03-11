using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IGenerativeLoiQueue
{
    Task EnqueueAsync(GenerativeLoiJob job, CancellationToken ct = default);
}
