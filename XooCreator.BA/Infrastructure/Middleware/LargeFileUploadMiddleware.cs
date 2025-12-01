using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;

namespace XooCreator.BA.Infrastructure.Middleware;

/// <summary>
/// Middleware to disable request size limits for large file upload endpoints
/// Must be registered BEFORE UseCors and other middleware
/// </summary>
public class LargeFileUploadMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<LargeFileUploadMiddleware> _logger;
    private const long MaxRequestSize = 600 * 1024 * 1024; // 600MB

    public LargeFileUploadMiddleware(RequestDelegate next, ILogger<LargeFileUploadMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Check if this is the import-full endpoint
        var path = context.Request.Path.Value ?? string.Empty;
        if (path.Contains("/stories/import-full", StringComparison.OrdinalIgnoreCase) && 
            context.Request.Method.Equals("POST", StringComparison.OrdinalIgnoreCase))
        {
            _logger.LogDebug("Large file upload detected for path: {Path}, setting unlimited request size", path);

            // Disable request size limit for this specific request
            var feature = context.Features.Get<IHttpMaxRequestBodySizeFeature>();
            if (feature != null && !feature.IsReadOnly)
            {
                feature.MaxRequestBodySize = null; // null = unlimited
                _logger.LogDebug("Request size limit disabled for import-full endpoint");
            }
            else if (feature?.IsReadOnly == true)
            {
                _logger.LogWarning("Cannot disable request size limit - feature is read-only. This may cause 413 errors for large files.");
            }

            // Also configure form options for this request
            var formFeature = context.Features.Get<IFormFeature>();
            if (formFeature == null || formFeature.Form == null)
            {
                context.Features.Set<IFormFeature>(new FormFeature(context.Request, new FormOptions
                {
                    MultipartBodyLengthLimit = MaxRequestSize,
                    ValueLengthLimit = int.MaxValue,
                    KeyLengthLimit = int.MaxValue
                }));
            }
        }

        await _next(context);
    }
}

