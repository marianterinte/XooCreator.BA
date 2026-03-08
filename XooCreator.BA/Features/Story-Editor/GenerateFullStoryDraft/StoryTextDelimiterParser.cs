using System.Text.RegularExpressions;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

/// <summary>
/// Parses AI-generated or imported story text in ###T / ###S / ###P1..###PN format.
/// Aligned with frontend StoryEditorTextImportService.importFromText().
/// No modifications to existing files; used only by Generate Full Story Draft.
/// </summary>
public static class StoryTextDelimiterParser
{
    private static readonly Regex DelimiterRegex = new(@"###(T|S|P(\d+))", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));

    /// <summary>
    /// Parses raw text containing ###T (title), ###S (summary), ###P1, ###P2, ... (pages).
    /// If no valid delimiters found, attempts fallback: "Page 1:", "Page 2:" pattern or single block as title + one page.
    /// </summary>
    public static StoryTextParseResult Parse(string rawText)
    {
        if (string.IsNullOrWhiteSpace(rawText))
            return FallbackEmpty();

        var delimiters = CollectDelimiters(rawText);
        if (delimiters.Count == 0)
            return ParsePageFallback(rawText);

        return ExtractSections(rawText, delimiters);
    }

    private static List<DelimiterInfo> CollectDelimiters(string content)
    {
        var list = new List<DelimiterInfo>();
        var match = DelimiterRegex.Match(content);
        while (match.Success)
        {
            var typeChar = match.Groups[1].Value;
            var position = match.Index;
            var length = match.Length;

            if (typeChar == "T")
                list.Add(new DelimiterInfo(DelimiterType.Title, position, length, null));
            else if (typeChar == "S")
                list.Add(new DelimiterInfo(DelimiterType.Summary, position, length, null));
            else if (match.Groups[2].Success && int.TryParse(match.Groups[2].Value, out var num) && num > 0)
                list.Add(new DelimiterInfo(DelimiterType.Page, position, length, num));

            match = match.NextMatch();
        }

        list.Sort((a, b) => a.Position.CompareTo(b.Position));
        return list;
    }

    private static StoryTextParseResult ExtractSections(string content, List<DelimiterInfo> delimiters)
    {
        string title = string.Empty;
        string summary = string.Empty;
        var pages = new List<StoryTextPagePart>();

        for (var i = 0; i < delimiters.Count; i++)
        {
            var d = delimiters[i];
            var contentStart = ContentStartAfterDelimiterLine(content, d.Position, d.Length);
            var contentEnd = i < delimiters.Count - 1
                ? LineStartOf(content, delimiters[i + 1].Position)
                : content.Length;
            var sectionContent = content.Substring(contentStart, contentEnd - contentStart);
            sectionContent = sectionContent.Trim();
            sectionContent = Regex.Replace(sectionContent, @"^[\s\n\r]+", string.Empty);
            sectionContent = Regex.Replace(sectionContent, @"[\s\n\r]+$", string.Empty);

            switch (d.Type)
            {
                case DelimiterType.Title:
                    title = Regex.Replace(sectionContent, @"\s+", " ").Trim();
                    break;
                case DelimiterType.Summary:
                    summary = Regex.Replace(sectionContent, @"\s+", " ").Trim();
                    break;
                case DelimiterType.Page when d.PageNumber.HasValue:
                    var pageText = sectionContent.Replace("\r\n", "\n").Replace("\r", "\n");
                    pages.Add(new StoryTextPagePart { PageId = $"p{d.PageNumber.Value}", Text = pageText });
                    break;
            }
        }

        if (string.IsNullOrWhiteSpace(title) && pages.Count > 0)
        {
            var firstPageText = pages[0].Text ?? string.Empty;
            title = Regex.Replace(firstPageText, @"\s+", " ").Trim();
            if (title.Length > 50) title = title.Substring(0, 50);
            if (string.IsNullOrWhiteSpace(title)) title = "AI Story";
        }

        if (string.IsNullOrWhiteSpace(title))
            title = "AI Story";

        if (pages.Count == 0)
            pages.Add(new StoryTextPagePart { PageId = "p1", Text = string.Empty });

        return new StoryTextParseResult
        {
            Title = title,
            Summary = summary ?? string.Empty,
            Pages = pages
        };
    }

    /// <summary>Fallback when no ###T/###S/###P found: try "Page 1:", "Page 2:" pattern (OpenAI style).</summary>
    private static StoryTextParseResult ParsePageFallback(string rawText)
    {
        var pageRegex = new Regex(@"Page\s+(\d+)\s*:\s*", RegexOptions.IgnoreCase | RegexOptions.Compiled, TimeSpan.FromMilliseconds(100));
        var matches = pageRegex.Matches(rawText);
        if (matches.Count > 0)
        {
            var pages = new List<StoryTextPagePart>();
            for (var i = 0; i < matches.Count; i++)
            {
                var start = matches[i].Index + matches[i].Length;
                var end = i < matches.Count - 1 ? matches[i + 1].Index : rawText.Length;
                var text = rawText.Substring(start, end - start).Trim().Replace("\r\n", "\n").Replace("\r", "\n");
                var num = int.Parse(matches[i].Groups[1].Value);
                pages.Add(new StoryTextPagePart { PageId = $"p{num}", Text = text });
            }

            var title = pages.Count > 0 && !string.IsNullOrWhiteSpace(pages[0].Text)
                ? Regex.Replace(pages[0].Text, @"\s+", " ").Trim()
                : "AI Story";
            if (title.Length > 50) title = title.Substring(0, 50);

            return new StoryTextParseResult
            {
                Title = title,
                Summary = string.Empty,
                Pages = pages
            };
        }

        var singleLine = Regex.Replace(rawText.Trim(), @"\s+", " ").Trim();
        var fallbackTitle = singleLine.Length > 50 ? singleLine.Substring(0, 50) : (singleLine.Length > 0 ? singleLine : "AI Story");
        return new StoryTextParseResult
        {
            Title = fallbackTitle,
            Summary = string.Empty,
            Pages = new List<StoryTextPagePart> { new StoryTextPagePart { PageId = "p1", Text = rawText.Trim() } }
        };
    }

    private static int ContentStartAfterDelimiterLine(string content, int delimiterPosition, int delimiterLength)
    {
        var lineEnd = content.IndexOf('\n', delimiterPosition);
        if (lineEnd < 0) lineEnd = content.Length;
        return Math.Min(lineEnd + 1, content.Length);
    }

    private static int LineStartOf(string content, int position)
    {
        var lastNewLine = content.LastIndexOf('\n', position);
        return lastNewLine < 0 ? 0 : lastNewLine + 1;
    }

    private static StoryTextParseResult FallbackEmpty()
    {
        return new StoryTextParseResult
        {
            Title = "AI Story",
            Summary = string.Empty,
            Pages = new List<StoryTextPagePart> { new StoryTextPagePart { PageId = "p1", Text = string.Empty } }
        };
    }

    private enum DelimiterType { Title, Summary, Page }

    private sealed record DelimiterInfo(DelimiterType Type, int Position, int Length, int? PageNumber);
}
