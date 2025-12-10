using System;
using System.Threading;
using System.Threading.Tasks;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Services;

public interface IStoryEpicPublishingService
{
    Task<DateTime?> PublishAsync(Guid ownerUserId, string epicId, CancellationToken ct = default);
}
