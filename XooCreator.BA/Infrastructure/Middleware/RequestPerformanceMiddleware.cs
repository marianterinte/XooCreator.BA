using System.Diagnostics;
using Microsoft.ApplicationInsights;

namespace XooCreator.BA.Infrastructure.Middleware;

public class RequestPerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPerformanceMiddleware> _logger;
    private readonly TelemetryClient? _telemetryClient;

    public RequestPerformanceMiddleware(
        RequestDelegate next,
        ILogger<RequestPerformanceMiddleware> logger,
        TelemetryClient? telemetryClient = null)
    {
        _next = next;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Log separator before request
        _logger.LogInformation("---------------------- ---------------------- ---------------------- ----------------------");
        _logger.LogInformation("Request started | Path={Path} | Method={Method}",
            context.Request.Path,
            context.Request.Method);

        var stopwatch = Stopwatch.StartNew();
        var requestStartTime = DateTimeOffset.UtcNow;
        
        // Capture initial memory and GC stats
        var initialMemory = GC.GetTotalMemory(false);
        var initialGen0 = GC.CollectionCount(0);
        var initialGen1 = GC.CollectionCount(1);
        var initialGen2 = GC.CollectionCount(2);
        var initialWorkingSet = Environment.WorkingSet;

        try
        {
            await _next(context);
        }
        finally
        {
            stopwatch.Stop();
            var requestDuration = stopwatch.ElapsedMilliseconds;
            
            // Capture final memory and GC stats
            var finalMemory = GC.GetTotalMemory(false);
            var finalGen0 = GC.CollectionCount(0);
            var finalGen1 = GC.CollectionCount(1);
            var finalGen2 = GC.CollectionCount(2);
            var finalWorkingSet = Environment.WorkingSet;
            
            var memoryDelta = finalMemory - initialMemory;
            var gen0Collections = finalGen0 - initialGen0;
            var gen1Collections = finalGen1 - initialGen1;
            var gen2Collections = finalGen2 - initialGen2;
            var workingSetDelta = finalWorkingSet - initialWorkingSet;

            // Calculate server response time (time from request start to response start)
            // This is approximate - actual server response time would be measured differently
            var serverResponseTime = requestDuration; // For now, use total duration

            // Log to ILogger
            _logger.LogInformation(
                "Request completed | Path={Path} | Method={Method} | StatusCode={StatusCode} | " +
                "RequestDuration={RequestDuration}ms | ServerResponseTime={ServerResponseTime}ms | " +
                "MemoryDelta={MemoryDelta}bytes | WorkingSetDelta={WorkingSetDelta}bytes | " +
                "GC_Gen0={Gen0Collections} | GC_Gen1={Gen1Collections} | GC_Gen2={Gen2Collections}",
                context.Request.Path,
                context.Request.Method,
                context.Response.StatusCode,
                requestDuration,
                serverResponseTime,
                memoryDelta,
                workingSetDelta,
                gen0Collections,
                gen1Collections,
                gen2Collections);

            // Log separator after request
            _logger.LogInformation("---------------------- ---------------------- ---------------------- ----------------------");

            // Track in Application Insights
            if (_telemetryClient != null)
            {
                // Track request duration as custom metric
                _telemetryClient.TrackMetric("RequestDuration", requestDuration, new Dictionary<string, string>
                {
                    ["Path"] = context.Request.Path,
                    ["Method"] = context.Request.Method,
                    ["StatusCode"] = context.Response.StatusCode.ToString()
                });

                // Track server response time
                _telemetryClient.TrackMetric("ServerResponseTime", serverResponseTime, new Dictionary<string, string>
                {
                    ["Path"] = context.Request.Path,
                    ["Method"] = context.Request.Method,
                    ["StatusCode"] = context.Response.StatusCode.ToString()
                });

                // Track working set
                _telemetryClient.TrackMetric("WorkingSet", finalWorkingSet, new Dictionary<string, string>
                {
                    ["Path"] = context.Request.Path,
                    ["Method"] = context.Request.Method
                });

                // Track working set delta
                _telemetryClient.TrackMetric("WorkingSetDelta", workingSetDelta, new Dictionary<string, string>
                {
                    ["Path"] = context.Request.Path,
                    ["Method"] = context.Request.Method
                });

                // Track memory delta
                _telemetryClient.TrackMetric("MemoryDelta", memoryDelta, new Dictionary<string, string>
                {
                    ["Path"] = context.Request.Path,
                    ["Method"] = context.Request.Method
                });

                // Track GC collections
                if (gen0Collections > 0)
                {
                    _telemetryClient.TrackMetric("GC_Gen0_Collections", gen0Collections, new Dictionary<string, string>
                    {
                        ["Path"] = context.Request.Path,
                        ["Method"] = context.Request.Method
                    });
                }

                if (gen1Collections > 0)
                {
                    _telemetryClient.TrackMetric("GC_Gen1_Collections", gen1Collections, new Dictionary<string, string>
                    {
                        ["Path"] = context.Request.Path,
                        ["Method"] = context.Request.Method
                    });
                }

                if (gen2Collections > 0)
                {
                    _telemetryClient.TrackMetric("GC_Gen2_Collections", gen2Collections, new Dictionary<string, string>
                    {
                        ["Path"] = context.Request.Path,
                        ["Method"] = context.Request.Method
                    });
                }

                // Note: Request telemetry is automatically tracked by Application Insights SDK
                // Custom properties can be added via Activity.Current or through TelemetryInitializer if needed
            }
        }
    }
}

public static class RequestPerformanceMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestPerformanceTracking(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestPerformanceMiddleware>();
    }
}

