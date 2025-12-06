using Azure.Storage.Queues;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Infrastructure.Services.Queue;

public class AzureQueueClientFactory : IAzureQueueClientFactory
{
    private readonly string _connectionString;
    private readonly ILogger<AzureQueueClientFactory> _logger;

    public AzureQueueClientFactory(IConfiguration configuration, ILogger<AzureQueueClientFactory> logger)
    {
        _logger = logger;

        var section = configuration.GetSection("AzureStorage");
        _connectionString = ResolveConfiguredValue(section["ConnectionString"])
                            ?? Environment.GetEnvironmentVariable("AzureStorage__ConnectionString")
                            ?? throw new InvalidOperationException("AzureStorage:ConnectionString is not configured.");
    }

    public QueueClient CreateClient(string? configuredQueueName, string defaultQueueName)
    {
        var queueName = ResolveConfiguredValue(configuredQueueName) ?? defaultQueueName;

        if (string.IsNullOrWhiteSpace(queueName))
        {
            throw new InvalidOperationException("Queue name cannot be null or empty.");
        }

        _logger.LogDebug("Creating QueueClient for queueName={QueueName}", queueName);

        return new QueueClient(_connectionString, queueName, new QueueClientOptions
        {
            MessageEncoding = QueueMessageEncoding.Base64
        });
    }

    private static string? ResolveConfiguredValue(string? configured)
    {
        if (string.IsNullOrWhiteSpace(configured))
        {
            return null;
        }

        if (configured.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
        {
            var envKey = configured[4..].Trim();
            return string.IsNullOrWhiteSpace(envKey) ? null : Environment.GetEnvironmentVariable(envKey);
        }

        return configured;
    }
}

