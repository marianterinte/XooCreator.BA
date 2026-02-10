using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IStoryImageExportQueue
{
    Task EnqueueAsync(StoryImageExportJob job, CancellationToken ct = default);
}
