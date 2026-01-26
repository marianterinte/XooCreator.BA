using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Features.User.Services;

public interface INewsletterTemplateService
{
    string GenerateNewStoriesNewsletterHtml(List<NewStoryItem> stories, string locale = "ro-ro");
    string GenerateNewStoriesNewsletterText(List<NewStoryItem> stories, string locale = "ro-ro");
    string GenerateNewsletterHtml(List<NewStoryItem> stories, List<NewEpicItem> epics, string locale = "ro-ro");
    string GenerateNewsletterText(List<NewStoryItem> stories, List<NewEpicItem> epics, string locale = "ro-ro");
}

public class NewsletterTemplateService : INewsletterTemplateService
{
    private readonly ILogger<NewsletterTemplateService> _logger;

    public NewsletterTemplateService(ILogger<NewsletterTemplateService> logger)
    {
        _logger = logger;
    }

    public string GenerateNewStoriesNewsletterHtml(List<NewStoryItem> stories, string locale = "ro-ro")
    {
        if (stories == null || stories.Count == 0)
        {
            return GetEmptyNewsletterHtml(locale);
        }

        var storiesHtml = string.Join("\n", stories.Select(story => GenerateStoryCardHtml(story, locale)));

        return $@"
<!DOCTYPE html>
<html lang=""{locale}"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Noutăți Alchimalia</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 8px;
            padding: 30px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
            border-bottom: 3px solid #059669;
            padding-bottom: 20px;
        }}
        .header h1 {{
            color: #0b4f33;
            margin: 0;
            font-size: 28px;
        }}
        .intro {{
            margin-bottom: 30px;
            color: #555;
            font-size: 16px;
        }}
        .story-card, .epic-card {{
            background-color: #f9fafb;
            border-left: 4px solid #059669;
            padding: 20px;
            margin-bottom: 20px;
            border-radius: 4px;
            display: flex;
            gap: 20px;
        }}
        .story-card-image, .epic-card-image {{
            flex-shrink: 0;
            width: 150px;
            height: 150px;
            object-fit: cover;
            border-radius: 8px;
        }}
        .story-card-content, .epic-card-content {{
            flex: 1;
        }}
        .story-card h3, .epic-card h3 {{
            color: #0b4f33;
            margin-top: 0;
            font-size: 20px;
        }}
        .story-card p, .epic-card p {{
            color: #666;
            margin: 10px 0;
        }}
        .story-link, .epic-link {{
            display: inline-block;
            background-color: #059669;
            color: #ffffff;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 6px;
            margin-top: 10px;
            font-weight: 600;
        }}
        .story-link:hover, .epic-link:hover {{
            background-color: #047857;
        }}
        .section-title {{
            font-size: 24px;
            color: #0b4f33;
            margin: 30px 0 20px 0;
            padding-bottom: 10px;
            border-bottom: 2px solid #059669;
        }}
        .footer {{
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #e5e7eb;
            text-align: center;
            color: #888;
            font-size: 14px;
        }}
        .unsubscribe {{
            margin-top: 20px;
            font-size: 12px;
            color: #999;
        }}
        .unsubscribe a {{
            color: #059669;
            text-decoration: none;
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🌟 Alchimalia</h1>
        </div>
        
        <div class=""intro"">
            <p>Salut!</p>
            <p>Avem noutăți pentru tine! Am adăugat {(stories.Count == 1 ? "o poveste nouă" : $"{stories.Count} povești noi")} în Alchimalia.</p>
        </div>

        {storiesHtml}

        <div class=""footer"">
            <p>Mulțumim că ești alături de noi în această aventură! 🎭</p>
            <p>Echipa Alchimalia</p>
            <div class=""unsubscribe"">
                <p>Nu mai dorești să primești aceste email-uri? <a href=""{{unsubscribe_url}}"">Anulează abonarea</a></p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateNewStoriesNewsletterText(List<NewStoryItem> stories, string locale = "ro-ro")
    {
        if (stories == null || stories.Count == 0)
        {
            return GetEmptyNewsletterText(locale);
        }

        var storiesText = string.Join("\n\n", stories.Select(story => GenerateStoryCardText(story, locale)));

        return $@"Noutăți Alchimalia

Salut!

Avem noutăți pentru tine! Am adăugat {(stories.Count == 1 ? "o poveste nouă" : $"{stories.Count} povești noi")} în Alchimalia.

{storiesText}

---

Mulțumim că ești alături de noi în această aventură!

Echipa Alchimalia

Nu mai dorești să primești aceste email-uri? Vizitează {{unsubscribe_url}} pentru a anula abonarea.";
    }

    public string GenerateNewsletterHtml(List<NewStoryItem> stories, List<NewEpicItem> epics, string locale = "ro-ro")
    {
        var storiesHtml = stories != null && stories.Count > 0
            ? string.Join("\n", stories.Select(story => GenerateStoryCardHtml(story, locale)))
            : "";
        
        var epicsHtml = epics != null && epics.Count > 0
            ? string.Join("\n", epics.Select(epic => GenerateEpicCardHtml(epic, locale)))
            : "";

        var totalItems = (stories?.Count ?? 0) + (epics?.Count ?? 0);
        if (totalItems == 0)
        {
            return GetEmptyNewsletterHtml(locale);
        }

        var introText = locale.StartsWith("ro", StringComparison.OrdinalIgnoreCase)
            ? GetRomanianIntroText(stories?.Count ?? 0, epics?.Count ?? 0)
            : GetEnglishIntroText(stories?.Count ?? 0, epics?.Count ?? 0);

        return $@"
<!DOCTYPE html>
<html lang=""{locale}"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Noutăți Alchimalia</title>
    <style>
        body {{
            font-family: -apple-system, BlinkMacSystemFont, 'Segoe UI', Roboto, 'Helvetica Neue', Arial, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 8px;
            padding: 30px;
            box-shadow: 0 2px 4px rgba(0,0,0,0.1);
        }}
        .header {{
            text-align: center;
            margin-bottom: 30px;
            border-bottom: 3px solid #059669;
            padding-bottom: 20px;
        }}
        .header h1 {{
            color: #0b4f33;
            margin: 0;
            font-size: 28px;
        }}
        .intro {{
            margin-bottom: 30px;
            color: #555;
            font-size: 16px;
        }}
        .story-card, .epic-card {{
            background-color: #f9fafb;
            border-left: 4px solid #059669;
            padding: 20px;
            margin-bottom: 20px;
            border-radius: 4px;
            display: flex;
            gap: 20px;
        }}
        .story-card-image, .epic-card-image {{
            flex-shrink: 0;
            width: 150px;
            height: 150px;
            object-fit: cover;
            border-radius: 8px;
        }}
        .story-card-content, .epic-card-content {{
            flex: 1;
        }}
        .story-card h3, .epic-card h3 {{
            color: #0b4f33;
            margin-top: 0;
            font-size: 20px;
        }}
        .story-card p, .epic-card p {{
            color: #666;
            margin: 10px 0;
        }}
        .story-link, .epic-link {{
            display: inline-block;
            background-color: #059669;
            color: #ffffff;
            padding: 12px 24px;
            text-decoration: none;
            border-radius: 6px;
            margin-top: 10px;
            font-weight: 600;
        }}
        .story-link:hover, .epic-link:hover {{
            background-color: #047857;
        }}
        .section-title {{
            font-size: 24px;
            color: #0b4f33;
            margin: 30px 0 20px 0;
            padding-bottom: 10px;
            border-bottom: 2px solid #059669;
        }}
        .footer {{
            margin-top: 40px;
            padding-top: 20px;
            border-top: 1px solid #e5e7eb;
            text-align: center;
            color: #888;
            font-size: 14px;
        }}
        .unsubscribe {{
            margin-top: 20px;
            font-size: 12px;
            color: #999;
        }}
        .unsubscribe a {{
            color: #059669;
            text-decoration: none;
        }}
        @media (max-width: 600px) {{
            .story-card, .epic-card {{
                flex-direction: column;
            }}
            .story-card-image, .epic-card-image {{
                width: 100%;
                height: 200px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🌟 Alchimalia</h1>
        </div>
        
        <div class=""intro"">
            {introText}
        </div>

        {(string.IsNullOrEmpty(storiesHtml) ? "" : $@"<h2 class=""section-title"">📚 Povești</h2>{storiesHtml}")}
        {(string.IsNullOrEmpty(epicsHtml) ? "" : $@"<h2 class=""section-title"">🎭 Epic-uri</h2>{epicsHtml}")}

        <div class=""footer"">
            <p>Mulțumim că ești alături de noi în această aventură! 🎭</p>
            <p>Echipa Alchimalia</p>
            <div class=""unsubscribe"">
                <p>Nu mai dorești să primești aceste email-uri? <a href=""{{unsubscribe_url}}"">Anulează abonarea</a></p>
            </div>
        </div>
    </div>
</body>
</html>";
    }

    public string GenerateNewsletterText(List<NewStoryItem> stories, List<NewEpicItem> epics, string locale = "ro-ro")
    {
        var storiesText = stories != null && stories.Count > 0
            ? string.Join("\n\n", stories.Select(story => GenerateStoryCardText(story, locale)))
            : "";
        
        var epicsText = epics != null && epics.Count > 0
            ? string.Join("\n\n", epics.Select(epic => GenerateEpicCardText(epic, locale)))
            : "";

        var totalItems = (stories?.Count ?? 0) + (epics?.Count ?? 0);
        if (totalItems == 0)
        {
            return GetEmptyNewsletterText(locale);
        }

        var introText = locale.StartsWith("ro", StringComparison.OrdinalIgnoreCase)
            ? GetRomanianIntroText(stories?.Count ?? 0, epics?.Count ?? 0)
            : GetEnglishIntroText(stories?.Count ?? 0, epics?.Count ?? 0);

        var content = new System.Text.StringBuilder();
        content.AppendLine("Noutăți Alchimalia");
        content.AppendLine();
        content.AppendLine(introText);
        content.AppendLine();

        if (!string.IsNullOrEmpty(storiesText))
        {
            content.AppendLine("--- Povești ---");
            content.AppendLine(storiesText);
            content.AppendLine();
        }

        if (!string.IsNullOrEmpty(epicsText))
        {
            content.AppendLine("--- Epic-uri ---");
            content.AppendLine(epicsText);
            content.AppendLine();
        }

        content.AppendLine("---");
        content.AppendLine();
        content.AppendLine("Mulțumim că ești alături de noi în această aventură!");
        content.AppendLine();
        content.AppendLine("Echipa Alchimalia");
        content.AppendLine();
        content.AppendLine("Nu mai dorești să primești aceste email-uri? Vizitează {unsubscribe_url} pentru a anula abonarea.");

        return content.ToString();
    }

    private string GenerateStoryCardHtml(NewStoryItem story, string locale)
    {
        var title = story.Title ?? "Poveste nouă";
        var description = story.Description ?? "";
        var link = story.Url ?? "#";
        var imageHtml = !string.IsNullOrEmpty(story.CoverImageUrl)
            ? $@"<img src=""{EscapeHtml(story.CoverImageUrl)}"" alt=""{EscapeHtml(title)}"" class=""story-card-image"" />"
            : "";

        return $@"
        <div class=""story-card"">
            {imageHtml}
            <div class=""story-card-content"">
                <h3>{EscapeHtml(title)}</h3>
                {(string.IsNullOrEmpty(description) ? "" : $"<p>{EscapeHtml(description)}</p>")}
                <a href=""{EscapeHtml(link)}"" class=""story-link"">Citește povestea →</a>
            </div>
        </div>";
    }

    private string GenerateEpicCardHtml(NewEpicItem epic, string locale)
    {
        var name = epic.Name ?? "Epic nou";
        var description = epic.Description ?? "";
        var link = epic.Url ?? "#";
        var imageHtml = !string.IsNullOrEmpty(epic.CoverImageUrl)
            ? $@"<img src=""{EscapeHtml(epic.CoverImageUrl)}"" alt=""{EscapeHtml(name)}"" class=""epic-card-image"" />"
            : "";

        return $@"
        <div class=""epic-card"">
            {imageHtml}
            <div class=""epic-card-content"">
                <h3>{EscapeHtml(name)}</h3>
                {(string.IsNullOrEmpty(description) ? "" : $"<p>{EscapeHtml(description)}</p>")}
                <a href=""{EscapeHtml(link)}"" class=""epic-link"">Explorează epic-ul →</a>
            </div>
        </div>";
    }

    private string GenerateEpicCardText(NewEpicItem epic, string locale)
    {
        var name = epic.Name ?? "Epic nou";
        var description = epic.Description ?? "";
        var link = epic.Url ?? "#";

        return $@"{name}
{(string.IsNullOrEmpty(description) ? "" : $"{description}\n")}
Explorează aici: {link}";
    }

    private string GetRomanianIntroText(int storiesCount, int epicsCount)
    {
        if (storiesCount > 0 && epicsCount > 0)
        {
            return $@"<p>Salut!</p>
            <p>Avem noutăți pentru tine! Am adăugat {GetRomanianCount(storiesCount, "poveste", "povești")} și {GetRomanianCount(epicsCount, "epic", "epic-uri")} în Alchimalia.</p>";
        }
        else if (storiesCount > 0)
        {
            return $@"<p>Salut!</p>
            <p>Avem noutăți pentru tine! Am adăugat {GetRomanianCount(storiesCount, "poveste nouă", "povești noi")} în Alchimalia.</p>";
        }
        else if (epicsCount > 0)
        {
            return $@"<p>Salut!</p>
            <p>Avem noutăți pentru tine! Am adăugat {GetRomanianCount(epicsCount, "epic nou", "epic-uri noi")} în Alchimalia.</p>";
        }
        return "<p>Salut!</p><p>Avem noutăți pentru tine!</p>";
    }

    private string GetEnglishIntroText(int storiesCount, int epicsCount)
    {
        if (storiesCount > 0 && epicsCount > 0)
        {
            return $@"<p>Hello!</p>
            <p>We have news for you! We've added {storiesCount} {(storiesCount == 1 ? "story" : "stories")} and {epicsCount} {(epicsCount == 1 ? "epic" : "epics")} to Alchimalia.</p>";
        }
        else if (storiesCount > 0)
        {
            return $@"<p>Hello!</p>
            <p>We have news for you! We've added {storiesCount} {(storiesCount == 1 ? "new story" : "new stories")} to Alchimalia.</p>";
        }
        else if (epicsCount > 0)
        {
            return $@"<p>Hello!</p>
            <p>We have news for you! We've added {epicsCount} {(epicsCount == 1 ? "new epic" : "new epics")} to Alchimalia.</p>";
        }
        return "<p>Hello!</p><p>We have news for you!</p>";
    }

    private string GetRomanianCount(int count, string singular, string plural)
    {
        return count == 1 ? $"o {singular}" : $"{count} {plural}";
    }

    private string GenerateStoryCardText(NewStoryItem story, string locale)
    {
        var title = story.Title ?? "Poveste nouă";
        var description = story.Description ?? "";
        var link = story.Url ?? "#";

        return $@"{title}
{(string.IsNullOrEmpty(description) ? "" : $"{description}\n")}
Citește aici: {link}";
    }

    private string GetEmptyNewsletterHtml(string locale)
    {
        return $@"
<!DOCTYPE html>
<html lang=""{locale}"">
<head>
    <meta charset=""UTF-8"">
    <title>Alchimalia Newsletter</title>
</head>
<body>
    <div style=""font-family: Arial, sans-serif; padding: 20px;"">
        <h1>Alchimalia</h1>
        <p>Nu avem noutăți în acest moment, dar te vom anunța când apar povești noi!</p>
    </div>
</body>
</html>";
    }

    private string GetEmptyNewsletterText(string locale)
    {
        return @"Alchimalia Newsletter

Nu avem noutăți în acest moment, dar te vom anunța când apar povești noi!";
    }

    private static string EscapeHtml(string text)
    {
        return text
            .Replace("&", "&amp;")
            .Replace("<", "&lt;")
            .Replace(">", "&gt;")
            .Replace("\"", "&quot;")
            .Replace("'", "&#39;");
    }
}

public class NewStoryItem
{
    public string Title { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? CoverImageUrl { get; set; }
}

public class NewEpicItem
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Url { get; set; }
    public string? CoverImageUrl { get; set; }
}
