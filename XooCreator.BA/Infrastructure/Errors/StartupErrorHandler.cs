using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace XooCreator.BA.Infrastructure.Errors;

public static class StartupErrorHandler
{
    public static void ConfigureErrorPage(IApplicationBuilder app)
    {
        app.UseExceptionHandler(errorApp =>
        {
            errorApp.Run(async context =>
            {
                var logger = context.RequestServices.GetRequiredService<ILogger<Program>>();
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;

                if (exception != null)
                {
                    // Log to Application Insights
                    logger.LogCritical(exception, "❌ CRITICAL: Application startup failed!");
                    
                    // Log detailed error information
                    logger.LogCritical("Exception Type: {ExceptionType}", exception.GetType().FullName);
                    logger.LogCritical("Exception Message: {ExceptionMessage}", exception.Message);
                    
                    if (exception is Npgsql.PostgresException pgEx)
                    {
                        logger.LogCritical("PostgreSQL Error Code: {SqlState}", pgEx.SqlState);
                        logger.LogCritical("PostgreSQL Error Detail: {Detail}", pgEx.Detail ?? "N/A");
                        logger.LogCritical("PostgreSQL Error Hint: {Hint}", pgEx.Hint ?? "N/A");
                        logger.LogCritical("PostgreSQL Error Position: {Position}", pgEx.Position);
                    }
                    
                    if (exception.InnerException != null)
                    {
                        logger.LogCritical("Inner Exception: {InnerExceptionType} - {InnerExceptionMessage}",
                            exception.InnerException.GetType().FullName, exception.InnerException.Message);
                    }
                    
                    if (exception.StackTrace != null)
                    {
                        logger.LogCritical("Stack Trace: {StackTrace}", exception.StackTrace);
                    }

                    // Generate detailed HTML error page
                    var html = GenerateErrorPageHtml(exception);
                    context.Response.StatusCode = 500;
                    context.Response.ContentType = "text/html; charset=utf-8";
                    await context.Response.WriteAsync(html);
                }
            });
        });
    }

    public static string GenerateErrorPageHtml(Exception ex)
    {
        var pgEx = ex as Npgsql.PostgresException;
        var innerEx = ex.InnerException as Npgsql.PostgresException;
        
        var html = $@"<!DOCTYPE html>
<html>
<head>
    <title>Application Startup Error</title>
    <style>
        body {{ font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif; margin: 0; padding: 20px; background: #f5f5f5; }}
        .container {{ max-width: 1200px; margin: 0 auto; background: white; padding: 30px; border-radius: 8px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
        h1 {{ color: #d32f2f; margin-top: 0; }}
        h2 {{ color: #1976d2; margin-top: 30px; border-bottom: 2px solid #1976d2; padding-bottom: 10px; }}
        .error-box {{ background: #ffebee; border-left: 4px solid #d32f2f; padding: 15px; margin: 20px 0; }}
        .info-box {{ background: #e3f2fd; border-left: 4px solid #1976d2; padding: 15px; margin: 20px 0; }}
        .code {{ background: #f5f5f5; padding: 15px; border-radius: 4px; font-family: 'Consolas', 'Monaco', monospace; font-size: 13px; overflow-x: auto; white-space: pre-wrap; word-wrap: break-word; }}
        table {{ width: 100%; border-collapse: collapse; margin: 20px 0; }}
        th, td {{ padding: 12px; text-align: left; border-bottom: 1px solid #ddd; }}
        th {{ background: #f5f5f5; font-weight: bold; color: #333; }}
        .timestamp {{ color: #666; font-size: 0.9em; }}
    </style>
</head>
<body>
    <div class=""container"">
        <h1>❌ Application Startup Error</h1>
        <p class=""timestamp"">Timestamp: {DateTime.UtcNow:yyyy-MM-dd HH:mm:ss} UTC</p>
        
        <div class=""error-box"">
            <h2>Error Details</h2>
            <table>
                <tr><th>Exception Type</th><td><strong>{HtmlEncode(ex.GetType().FullName)}</strong></td></tr>
                <tr><th>Exception Message</th><td>{HtmlEncode(ex.Message)}</td></tr>
            </table>";

        if (pgEx != null)
        {
            html += $@"
            <h2>PostgreSQL Error Details</h2>
            <table>
                <tr><th>SQL State</th><td><code>{HtmlEncode(pgEx.SqlState)}</code></td></tr>
                <tr><th>Error Detail</th><td>{HtmlEncode(pgEx.Detail ?? "N/A")}</td></tr>
                <tr><th>Error Hint</th><td>{HtmlEncode(pgEx.Hint ?? "N/A")}</td></tr>
                <tr><th>Error Position</th><td>{pgEx.Position}</td></tr>";
            
            if (!string.IsNullOrWhiteSpace(pgEx.InternalQuery))
            {
                html += $@"<tr><th>Internal Query</th><td><div class=""code"">{HtmlEncode(pgEx.InternalQuery)}</div></td></tr>";
            }
            
            html += "</table>";
        }

        if (ex.InnerException != null)
        {
            html += $@"
            <h2>Inner Exception</h2>
            <table>
                <tr><th>Type</th><td>{HtmlEncode(ex.InnerException.GetType().FullName)}</td></tr>
                <tr><th>Message</th><td>{HtmlEncode(ex.InnerException.Message)}</td></tr>
            </table>";
            
            if (innerEx != null)
            {
                html += $@"
                <table>
                    <tr><th>PostgreSQL SQL State</th><td><code>{HtmlEncode(innerEx.SqlState)}</code></td></tr>
                    <tr><th>PostgreSQL Detail</th><td>{HtmlEncode(innerEx.Detail ?? "N/A")}</td></tr>
                </table>";
            }
        }

        if (!string.IsNullOrWhiteSpace(ex.StackTrace))
        {
            html += $@"
            <h2>Stack Trace</h2>
            <div class=""code"">{HtmlEncode(ex.StackTrace)}</div>";
        }

        html += $@"
            <div class=""info-box"">
                <h2>💡 Troubleshooting Steps</h2>
                <ul>
                    <li>Check the Application Insights logs for more details</li>
                    <li>Verify database connection string and permissions</li>
                    <li>Ensure the database schema exists and migrations are up to date</li>
                    <li>Check Azure App Service logs for additional context</li>
                    <li>Review the stack trace above to identify the failing component</li>
                </ul>
            </div>
        </div>
    </div>
</body>
</html>";

        return html;
    }

    private static string HtmlEncode(string? text)
    {
        if (string.IsNullOrEmpty(text)) return "N/A";
        return System.Net.WebUtility.HtmlEncode(text);
    }
}

