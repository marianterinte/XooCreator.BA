using System.Collections.Concurrent;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Infrastructure.Services.Jobs;

/// <summary>
/// In-memory, single-instance job events hub (Variant B).
/// Workers/endpoints publish job status payloads; SSE endpoint subscribes and streams them.
/// </summary>
public sealed class InMemoryJobEventsHub : IJobEventsHub
{
    private static readonly JsonSerializerOptions Json = new(JsonSerializerDefaults.Web);

    private readonly ConcurrentDictionary<string, ConcurrentDictionary<Guid, Channel<string>>> _topics = new();
    private readonly ILogger<InMemoryJobEventsHub> _logger;

    public InMemoryJobEventsHub(ILogger<InMemoryJobEventsHub> logger)
    {
        _logger = logger;
    }

    public JobEventStream Subscribe(string jobType, Guid jobId)
    {
        var key = GetKey(jobType, jobId);
        var subscriptionId = Guid.NewGuid();

        var channel = Channel.CreateUnbounded<string>(new UnboundedChannelOptions
        {
            SingleReader = true,
            SingleWriter = false,
            AllowSynchronousContinuations = true
        });

        var subs = _topics.GetOrAdd(key, _ => new ConcurrentDictionary<Guid, Channel<string>>());
        subs[subscriptionId] = channel;

        return new JobEventStream(
            channel.Reader,
            dispose: () =>
            {
                try
                {
                    if (_topics.TryGetValue(key, out var current))
                    {
                        current.TryRemove(subscriptionId, out var removed);
                        removed?.Writer.TryComplete();

                        if (current.IsEmpty)
                        {
                            _topics.TryRemove(key, out _);
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to dispose job subscription: key={Key} subscriptionId={SubscriptionId}", key, subscriptionId);
                }

                return ValueTask.CompletedTask;
            });
    }

    public void Publish(string jobType, Guid jobId, object payload)
    {
        var key = GetKey(jobType, jobId);
        if (!_topics.TryGetValue(key, out var subs) || subs.IsEmpty)
        {
            return;
        }

        string json;
        try
        {
            json = JsonSerializer.Serialize(payload, Json);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to serialize job payload: key={Key} payloadType={PayloadType}", key, payload.GetType().FullName);
            return;
        }

        foreach (var (subscriptionId, ch) in subs)
        {
            // If the subscriber is gone, clean up.
            if (!ch.Writer.TryWrite(json))
            {
                subs.TryRemove(subscriptionId, out var removed);
                removed?.Writer.TryComplete();
            }
        }

        if (subs.IsEmpty)
        {
            _topics.TryRemove(key, out _);
        }
    }

    private static string GetKey(string jobType, Guid jobId) => $"{jobType}:{jobId:D}";
}


