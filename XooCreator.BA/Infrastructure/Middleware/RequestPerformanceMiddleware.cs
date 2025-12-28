using System.Diagnostics;
using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Http.Features;
using XooCreator.BA.Infrastructure.Logging;

namespace XooCreator.BA.Infrastructure.Middleware;

public class RequestPerformanceMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestPerformanceMiddleware> _logger;
    private readonly TelemetryClient? _telemetryClient;

    // ANSI color constants for console output
    private const string Separator = "━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━━";

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
        var method = context.Request.Method;
        var path = context.Request.Path.Value ?? "/";
        var queryString = context.Request.QueryString.HasValue ? context.Request.QueryString.Value : "";
        var methodColor = ConsoleColors.GetMethodColor(method);
        var timestamp = DateTime.Now.ToString("HH:mm:ss.fff");
        
        // Log colored request start with separator
        Console.WriteLine($"{ConsoleColors.Gray}{Separator}{ConsoleColors.Reset}");
        Console.WriteLine($"{ConsoleColors.Gray}{timestamp}{ConsoleColors.Reset} {ConsoleColors.BrightCyan}▶{ConsoleColors.Reset} {methodColor}{ConsoleColors.Bold}{method,-7}{ConsoleColors.Reset} {ConsoleColors.Yellow}{path}{ConsoleColors.Reset}{ConsoleColors.Gray}{queryString}{ConsoleColors.Reset}");
        
        // Also log to ILogger for Application Insights (structured logging)
        _logger.LogInformation("Request started | Path={Path} | Method={Method}",
            path, method);

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
            var statusCode = context.Response.StatusCode;
            var endTimestamp = DateTime.Now.ToString("HH:mm:ss.fff");
            
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

            // Get endpoint information (available after routing)
            var endpoint = context.GetEndpoint();
            var endpointHandler = ExtractEndpointHandler(endpoint);

            var serverResponseTime = requestDuration;
            var statusColor = ConsoleColors.GetStatusCodeColor(statusCode);
            var durationColor = ConsoleColors.GetDurationColor(requestDuration);
            var statusIcon = statusCode >= 200 && statusCode < 400 ? "✓" : statusCode >= 400 && statusCode < 500 ? "⚠" : "✗";
            var statusIconColor = statusCode >= 200 && statusCode < 400 ? ConsoleColors.Green : statusCode >= 400 && statusCode < 500 ? ConsoleColors.Yellow : ConsoleColors.Red;
            
            // Build memory info string (only show if significant)
            var memoryInfo = "";
            if (Math.Abs(memoryDelta) > 1024 * 10) // > 10KB
            {
                var memoryDeltaKb = memoryDelta / 1024.0;
                var memoryDeltaColor = memoryDelta > 0 ? ConsoleColors.Yellow : ConsoleColors.Green;
                memoryInfo = $" {ConsoleColors.Gray}│{ConsoleColors.Reset} {memoryDeltaColor}Δmem:{memoryDeltaKb:+0;-0}KB{ConsoleColors.Reset}";
            }
            
            // Build GC info string (only show if collections happened)
            var gcInfo = "";
            if (gen0Collections > 0 || gen1Collections > 0 || gen2Collections > 0)
            {
                gcInfo = $" {ConsoleColors.Gray}│{ConsoleColors.Reset} {ConsoleColors.Magenta}GC:{gen0Collections}/{gen1Collections}/{gen2Collections}{ConsoleColors.Reset}";
            }

            // Log colored request completion
            Console.WriteLine($"{ConsoleColors.Gray}{endTimestamp}{ConsoleColors.Reset} {statusIconColor}{statusIcon}{ConsoleColors.Reset} {methodColor}{ConsoleColors.Bold}{method,-7}{ConsoleColors.Reset} {ConsoleColors.Yellow}{path}{ConsoleColors.Reset} {ConsoleColors.Gray}│{ConsoleColors.Reset} {statusColor}{statusCode}{ConsoleColors.Reset} {ConsoleColors.Gray}│{ConsoleColors.Reset} {durationColor}{requestDuration}ms{ConsoleColors.Reset}{memoryInfo}{gcInfo}");
            
            // Log endpoint handler if available
            if (!string.IsNullOrEmpty(endpointHandler) && endpointHandler != "Unknown")
            {
                Console.WriteLine($"           {ConsoleColors.Gray}└─>{ConsoleColors.Reset} {ConsoleColors.Magenta}{endpointHandler}{ConsoleColors.Reset}");
            }
            
            Console.WriteLine($"{ConsoleColors.Gray}{Separator}{ConsoleColors.Reset}");

            // Log to ILogger for Application Insights
            _logger.LogInformation(
                "Request completed | Path={Path} | Method={Method} | StatusCode={StatusCode} | " +
                "RequestDuration={RequestDuration}ms | ServerResponseTime={ServerResponseTime}ms | " +
                "MemoryDelta={MemoryDelta}bytes | WorkingSetDelta={WorkingSetDelta}bytes | " +
                "GC_Gen0={Gen0Collections} | GC_Gen1={Gen1Collections} | GC_Gen2={Gen2Collections} | Endpoint={Endpoint}",
                path,
                method,
                statusCode,
                requestDuration,
                serverResponseTime,
                memoryDelta,
                workingSetDelta,
                gen0Collections,
                gen1Collections,
                gen2Collections,
                endpointHandler);

            // Track in Application Insights
            if (_telemetryClient != null)
            {
                _telemetryClient.TrackMetric("RequestDuration", requestDuration, new Dictionary<string, string>
                {
                    ["Path"] = path,
                    ["Method"] = method,
                    ["StatusCode"] = statusCode.ToString(),
                    ["Endpoint"] = endpointHandler
                });

                _telemetryClient.TrackMetric("ServerResponseTime", serverResponseTime, new Dictionary<string, string>
                {
                    ["Path"] = path,
                    ["Method"] = method,
                    ["StatusCode"] = statusCode.ToString()
                });

                _telemetryClient.TrackMetric("WorkingSet", finalWorkingSet, new Dictionary<string, string>
                {
                    ["Path"] = path,
                    ["Method"] = method
                });

                _telemetryClient.TrackMetric("WorkingSetDelta", workingSetDelta, new Dictionary<string, string>
                {
                    ["Path"] = path,
                    ["Method"] = method
                });

                _telemetryClient.TrackMetric("MemoryDelta", memoryDelta, new Dictionary<string, string>
                {
                    ["Path"] = path,
                    ["Method"] = method
                });

                if (gen0Collections > 0)
                {
                    _telemetryClient.TrackMetric("GC_Gen0_Collections", gen0Collections, new Dictionary<string, string>
                    {
                        ["Path"] = path,
                        ["Method"] = method
                    });
                }

                if (gen1Collections > 0)
                {
                    _telemetryClient.TrackMetric("GC_Gen1_Collections", gen1Collections, new Dictionary<string, string>
                    {
                        ["Path"] = path,
                        ["Method"] = method
                    });
                }

                if (gen2Collections > 0)
                {
                    _telemetryClient.TrackMetric("GC_Gen2_Collections", gen2Collections, new Dictionary<string, string>
                    {
                        ["Path"] = path,
                        ["Method"] = method
                    });
                }
            }
        }
    }

    /// <summary>
    /// Extracts the endpoint handler name from the endpoint metadata
    /// </summary>
    private static string ExtractEndpointHandler(Endpoint? endpoint)
    {
        if (endpoint == null) return "Unknown";
        
        // Try to get the display name which usually contains the handler info
        var displayName = endpoint.DisplayName;
        if (string.IsNullOrEmpty(displayName)) return "Unknown";
        
        // For minimal APIs and our custom endpoints, the display name usually contains the method info
        // Format is usually "HTTP: GET /api/path" or "EndpointName.HandleGet"
        
        // Try to extract just the handler name (last part after arrow or the method name)
        if (displayName.Contains("->"))
        {
            var parts = displayName.Split("->");
            return parts.Last().Trim();
        }
        
        // If it contains the full delegate signature, try to simplify it
        if (displayName.Contains("("))
        {
            var methodPart = displayName.Split('(').First().Trim();
            // Get just the class.method part
            var dotParts = methodPart.Split('.');
            if (dotParts.Length >= 2)
            {
                return $"{dotParts[^2]}.{dotParts[^1]}";
            }
            return methodPart;
        }
        
        return displayName;
    }
}

public static class RequestPerformanceMiddlewareExtensions
{
    public static IApplicationBuilder UseRequestPerformanceTracking(this IApplicationBuilder app)
    {
        return app.UseMiddleware<RequestPerformanceMiddleware>();
    }
}
