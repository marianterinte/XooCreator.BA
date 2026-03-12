namespace XooCreator.BA.Infrastructure.Logging;

/// <summary>
/// ANSI color codes for colorful console logging.
/// </summary>
public static class ColoredLogHelper
{
    private const string Reset = "\u001b[0m";
    private const string Green = "\u001b[32m";
    private const string Yellow = "\u001b[33m";
    private const string Cyan = "\u001b[36m";
    private const string Magenta = "\u001b[35m";
    private const string Blue = "\u001b[34m";
    private const string Red = "\u001b[31m";
    private const string White = "\u001b[37m";
    private const string BrightGreen = "\u001b[92m";
    private const string BrightYellow = "\u001b[93m";
    private const string BrightCyan = "\u001b[96m";
    private const string BrightMagenta = "\u001b[95m";

    public static string FormatImageGeneration(string model, string status, string? details = null)
    {
        var arrow = $"{White}→{Reset}";
        var modelPart = $"{BrightMagenta}{model}{Reset}";
        var statusPart = status.ToUpperInvariant() switch
        {
            "START" => $"{BrightCyan}⚡ IMAGE GEN{Reset}",
            "OK" or "SUCCESS" => $"{BrightGreen}✓ IMAGE OK{Reset}",
            "FAIL" or "ERROR" => $"{Red}✗ IMAGE FAIL{Reset}",
            "RETRY" => $"{BrightYellow}↻ IMAGE RETRY{Reset}",
            _ => $"{Yellow}{status}{Reset}"
        };

        var detailsPart = string.IsNullOrWhiteSpace(details) ? string.Empty : $" {White}({details}){Reset}";
        return $"{statusPart} {arrow} {modelPart}{detailsPart}";
    }

    public static string FormatAudioGeneration(string model, string status, string? details = null)
    {
        var arrow = $"{White}→{Reset}";
        var modelPart = $"{BrightCyan}{model}{Reset}";
        var statusPart = status.ToUpperInvariant() switch
        {
            "START" => $"{BrightMagenta}♫ AUDIO GEN{Reset}",
            "OK" or "SUCCESS" => $"{BrightGreen}✓ AUDIO OK{Reset}",
            "FAIL" or "ERROR" => $"{Red}✗ AUDIO FAIL{Reset}",
            _ => $"{Yellow}{status}{Reset}"
        };

        var detailsPart = string.IsNullOrWhiteSpace(details) ? string.Empty : $" {White}({details}){Reset}";
        return $"{statusPart} {arrow} {modelPart}{detailsPart}";
    }

    public static string FormatStoryBible(string status, string? details = null)
    {
        var statusPart = status.ToUpperInvariant() switch
        {
            "START" => $"{BrightYellow}📖 STORY BIBLE{Reset}",
            "OK" or "SUCCESS" => $"{BrightGreen}✓ BIBLE OK{Reset}",
            "FAIL" or "ERROR" => $"{Red}✗ BIBLE FAIL{Reset}",
            _ => $"{Yellow}{status}{Reset}"
        };

        var detailsPart = string.IsNullOrWhiteSpace(details) ? string.Empty : $" {White}→ {details}{Reset}";
        return $"{statusPart}{detailsPart}";
    }

    public static string FormatScenePlan(string status, string? details = null)
    {
        var statusPart = status.ToUpperInvariant() switch
        {
            "START" => $"{BrightCyan}🎬 SCENE PLAN{Reset}",
            "OK" or "SUCCESS" => $"{BrightGreen}✓ SCENES OK{Reset}",
            "FAIL" or "ERROR" => $"{Red}✗ SCENES FAIL{Reset}",
            "FALLBACK" => $"{BrightYellow}⚠ SCENES FALLBACK{Reset}",
            _ => $"{Yellow}{status}{Reset}"
        };

        var detailsPart = string.IsNullOrWhiteSpace(details) ? string.Empty : $" {White}→ {details}{Reset}";
        return $"{statusPart}{detailsPart}";
    }

    public static string FormatProgress(int current, int total, string item)
    {
        return $"{BrightCyan}[{current}/{total}]{Reset} {White}{item}{Reset}";
    }

    public static string Highlight(string text) => $"{BrightYellow}{text}{Reset}";
    public static string Success(string text) => $"{BrightGreen}{text}{Reset}";
    public static string Error(string text) => $"{Red}{text}{Reset}";
    public static string Info(string text) => $"{BrightCyan}{text}{Reset}";
    public static string Muted(string text) => $"{White}{text}{Reset}";
}
