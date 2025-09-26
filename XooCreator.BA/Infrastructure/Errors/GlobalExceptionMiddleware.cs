using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Infrastructure.Errors;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
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

        // Log with structured context
        _logger.LogError(ex, "{Title} | TraceId={TraceId} | Layer={Layer} | Operation={Operation}", title, traceId, layer, operation);

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
