using System.Threading.Channels;

namespace XooCreator.BA.Infrastructure.Services.Jobs;

public interface IJobEventsHub
{
    JobEventStream Subscribe(string jobType, Guid jobId);
    void Publish(string jobType, Guid jobId, object payload);
}


