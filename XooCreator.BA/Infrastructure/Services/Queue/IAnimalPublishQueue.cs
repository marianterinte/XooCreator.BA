using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IAnimalPublishQueue
{
    Task EnqueueAsync(AnimalPublishJob job, CancellationToken ct = default);
}
