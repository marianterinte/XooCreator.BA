using Azure.Storage.Queues;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public interface IAzureQueueClientFactory
{
    QueueClient CreateClient(string? configuredQueueName, string defaultQueueName);
}

