using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Logging.Console;
using Microsoft.Extensions.Options;
using System.Text;

namespace XooCreator.BA.Infrastructure.Logging;

/// <summary>
/// Custom console formatter that colors the entire error message in red, not just the prefix.
/// </summary>
public class RedErrorConsoleFormatter : ConsoleFormatter
{
    private const string RedColorCode = "\x1B[31m";
    private const string ResetColorCode = "\x1B[0m";

    public RedErrorConsoleFormatter(IOptionsMonitor<SimpleConsoleFormatterOptions> options)
        : base("RedError")
    {
        Options = options.CurrentValue;
    }

    private SimpleConsoleFormatterOptions Options { get; }

    public override void Write<TState>(
        in LogEntry<TState> logEntry,
        IExternalScopeProvider? scopeProvider,
        TextWriter textWriter)
    {
        var message = logEntry.Formatter(logEntry.State, logEntry.Exception);
        if (string.IsNullOrEmpty(message) && logEntry.Exception == null)
        {
            return;
        }

        // Format timestamp
        var timestamp = Options.TimestampFormat != null
            ? DateTimeOffset.Now.ToString(Options.TimestampFormat)
            : string.Empty;

        // Format log level
        var logLevelString = GetLogLevelString(logEntry.LogLevel);

        // Format category
        var category = logEntry.Category;

        // Build the log line
        var logLine = new StringBuilder();

        // For errors and critical, color the entire message in red
        if (logEntry.LogLevel == LogLevel.Error || logEntry.LogLevel == LogLevel.Critical)
        {
            logLine.Append(RedColorCode);
        }

        // Add timestamp if configured
        if (!string.IsNullOrEmpty(timestamp))
        {
            logLine.Append(timestamp);
        }

        // Add log level
        logLine.Append(logLevelString);

        // Add category
        logLine.Append($"{category}");

        // Add message
        if (!string.IsNullOrEmpty(message))
        {
            logLine.Append($" {message}");
        }

        // Add exception if present
        if (logEntry.Exception != null)
        {
            logLine.AppendLine();
            logLine.Append(logEntry.Exception.ToString());
        }

        // Reset color for errors
        if (logEntry.LogLevel == LogLevel.Error || logEntry.LogLevel == LogLevel.Critical)
        {
            logLine.Append(ResetColorCode);
        }

        textWriter.WriteLine(logLine.ToString());
    }

    private static string GetLogLevelString(LogLevel logLevel)
    {
        return logLevel switch
        {
            LogLevel.Trace => "trce: ",
            LogLevel.Debug => "dbug: ",
            LogLevel.Information => "info: ",
            LogLevel.Warning => "warn: ",
            LogLevel.Error => "fail: ",
            LogLevel.Critical => "crit: ",
            LogLevel.None => string.Empty,
            _ => throw new ArgumentOutOfRangeException(nameof(logLevel))
        };
    }
}

