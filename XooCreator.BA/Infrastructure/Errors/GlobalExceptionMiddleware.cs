using System.Net;
using Microsoft.ApplicationInsights;
using Microsoft.ApplicationInsights.DataContracts;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Infrastructure.Errors;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly TelemetryClient? _telemetryClient;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger, TelemetryClient? telemetryClient = null)
    {
        _next = next;
        _logger = logger;
        _telemetryClient = telemetryClient;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception ex)
    {
        var traceId = context.TraceIdentifier;

        var status = ex switch
        {
            ValidationException => (int)HttpStatusCode.BadRequest,
            NotFoundException => (int)HttpStatusCode.NotFound,
            RepositoryException => (int)HttpStatusCode.InternalServerError,
            ServiceException => (int)HttpStatusCode.BadRequest,
            EndpointException => (int)HttpStatusCode.BadRequest,
            AppException => (int)HttpStatusCode.BadRequest,
            _ => (int)HttpStatusCode.InternalServerError
        };

        var (title, layer, operation) = ex switch
        {
            AppException appEx => ($"{appEx.Layer} error in {appEx.Operation}", appEx.Layer, appEx.Operation),
            _ => ("Unhandled server error", "Unknown", "Unknown")
        };

        // Log with structured context using ILogger (goes to Application Insights if configured)
        _logger.LogError(ex, "{Title} | TraceId={TraceId} | Layer={Layer} | Operation={Operation}", title, traceId, layer, operation);

        // Explicitly track exception in Application Insights using TelemetryClient
        if (_telemetryClient != null)
        {
            var exceptionTelemetry = new ExceptionTelemetry(ex)
            {
                SeverityLevel = status >= 500 ? SeverityLevel.Error : SeverityLevel.Warning,
                Properties =
                {
                    ["TraceId"] = traceId,
                    ["Layer"] = layer,
                    ["Operation"] = operation,
                    ["Title"] = title,
                    ["StatusCode"] = status.ToString(),
                    ["Path"] = context.Request?.Path.Value ?? "Unknown",
                    ["Method"] = context.Request?.Method ?? "Unknown",
                    ["QueryString"] = context.Request?.QueryString.ToString() ?? ""
                }
            };

            // Add custom dimensions for better filtering in Azure
            exceptionTelemetry.Context.Operation.Id = traceId;
            exceptionTelemetry.Context.Operation.Name = $"{context.Request?.Method} {context.Request?.Path}";

            _telemetryClient.TrackException(exceptionTelemetry);
            
            // Flush to ensure data is sent immediately (important for exceptions)
            _telemetryClient.Flush();
        }

        var pd = new ProblemDetails
        {
            Status = status,
            Title = title,
            Detail = ex.Message,
            Instance = context.Request?.Path.Value,
            Type = ex.GetType().FullName
        };
        pd.Extensions["traceId"] = traceId;
        pd.Extensions["layer"] = layer;
        pd.Extensions["operation"] = operation;

        if (context.Response.HasStarted)
        {
            _logger.LogWarning("Response already started; skipping error payload. TraceId={TraceId}", traceId);
            return;
        }

        context.Response.ContentType = "application/problem+json";
        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(pd);
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandling(this IApplicationBuilder app)
    {
        return app.UseMiddleware<GlobalExceptionMiddleware>();
    }
}
