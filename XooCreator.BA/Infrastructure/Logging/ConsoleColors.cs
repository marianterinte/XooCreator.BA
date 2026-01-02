namespace XooCreator.BA.Infrastructure.Logging;

/// <summary>
/// Helper class for ANSI color codes used in console logging
/// </summary>
public static class ConsoleColors
{
    // Reset
    public const string Reset = "\u001b[0m";
    
    // Text colors
    public const string Black = "\u001b[30m";
    public const string Red = "\u001b[31m";
    public const string Green = "\u001b[32m";
    public const string Yellow = "\u001b[33m";
    public const string Blue = "\u001b[34m";
    public const string Magenta = "\u001b[35m";
    public const string Cyan = "\u001b[36m";
    public const string White = "\u001b[37m";
    public const string Gray = "\u001b[90m";
    
    // Bright colors
    public const string BrightRed = "\u001b[91m";
    public const string BrightGreen = "\u001b[92m";
    public const string BrightYellow = "\u001b[93m";
    public const string BrightBlue = "\u001b[94m";
    public const string BrightMagenta = "\u001b[95m";
    public const string BrightCyan = "\u001b[96m";
    
    // Text styles
    public const string Bold = "\u001b[1m";
    public const string Dim = "\u001b[2m";
    
    /// <summary>
    /// Gets color for HTTP method
    /// </summary>
    public static string GetMethodColor(string method)
    {
        return method.ToUpperInvariant() switch
        {
            "GET" => Cyan,
            "POST" => Blue,
            "PUT" => Yellow,
            "DELETE" => Red,
            "PATCH" => White,
            _ => Gray
        };
    }
    
    /// <summary>
    /// Gets color for HTTP status code
    /// </summary>
    public static string GetStatusCodeColor(int statusCode)
    {
        return statusCode switch
        {
            >= 200 and < 300 => Green,      // 2xx - Success
            >= 300 and < 400 => Yellow,     // 3xx - Redirect
            >= 400 and < 500 => Red,        // 4xx - Client Error
            >= 500 => BrightRed,             // 5xx - Server Error
            _ => Gray
        };
    }
    
    /// <summary>
    /// Gets color for request duration (performance indicator)
    /// </summary>
    public static string GetDurationColor(long durationMs)
    {
        return durationMs switch
        {
            < 100 => Green,        // Fast (< 100ms)
            < 500 => Yellow,       // Medium (100-500ms)
            < 2000 => BrightYellow, // Slow (500-2000ms)
            _ => Red               // Very slow (> 2000ms)
        };
    }
}

