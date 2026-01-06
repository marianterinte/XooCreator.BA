using System.Threading.Channels;

namespace XooCreator.BA.Infrastructure.Services.Jobs;

public sealed class JobEventStream : IAsyncDisposable
{
    private readonly Func<ValueTask> _dispose;

    internal JobEventStream(ChannelReader<string> reader, Func<ValueTask> dispose)
    {
        Reader = reader;
        _dispose = dispose;
    }

    public ChannelReader<string> Reader { get; }

    public ValueTask DisposeAsync() => _dispose();
}


