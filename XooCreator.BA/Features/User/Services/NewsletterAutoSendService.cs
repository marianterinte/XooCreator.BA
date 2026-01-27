using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Data;
using XooCreator.BA.Features.User.Services;

namespace XooCreator.BA.Features.User.Services;

public interface INewsletterAutoSendService
{
    Task<bool> TrySendNewsletterForNewStoryAsync(string storyId, string locale, CancellationToken ct = default);
    Task<bool> TrySendNewsletterForNewEpicAsync(string epicId, string locale, CancellationToken ct = default);
}

public class NewsletterAutoSendService : INewsletterAutoSendService
{
    private readonly XooDbContext _db;
    private readonly INewsletterSendingService _newsletterSendingService;
    private readonly INewsletterTemplateService _templateService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<NewsletterAutoSendService> _logger;

    public NewsletterAutoSendService(
        XooDbContext db,
        INewsletterSendingService newsletterSendingService,
        INewsletterTemplateService templateService,
        IConfiguration configuration,
        ILogger<NewsletterAutoSendService> logger)
    {
        _db = db;
        _newsletterSendingService = newsletterSendingService;
        _templateService = templateService;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<bool> TrySendNewsletterForNewStoryAsync(string storyId, string locale, CancellationToken ct = default)
    {
        try
        {
            // Check if newsletter auto-send is enabled
            var autoSendEnabled = _configuration.GetValue<bool>("Newsletter:AutoSendEnabled", true);
            if (!autoSendEnabled)
            {
                _logger.LogDebug("Newsletter auto-send is disabled. Skipping newsletter for story {StoryId}", storyId);
                return false;
            }

            // Get the published story
            var story = await _db.StoryDefinitions
                .Include(s => s.Translations)
                .FirstOrDefaultAsync(s => s.StoryId == storyId && s.Status == Data.Enums.StoryStatus.Published && s.IsActive, ct);

            if (story == null)
            {
                _logger.LogWarning("Story not found or not published: {StoryId}", storyId);
                return false;
            }

            // Only send newsletter for independent stories (not part of epic)
            if (story.IsPartOfEpic)
            {
                _logger.LogDebug("Story {StoryId} is part of an epic, skipping newsletter", storyId);
                return false;
            }

            // Check if this is a new story (Version == 1 means first publish)
            // We'll send newsletter only for new stories, not updates
            if (story.Version > 1)
            {
                _logger.LogDebug("Story {StoryId} is an update (version {Version}), skipping newsletter", storyId, story.Version);
                return false;
            }

            // Check if story was created recently (within last hour) to avoid duplicates
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            if (story.CreatedAt < oneHourAgo)
            {
                _logger.LogDebug("Story {StoryId} was created more than 1 hour ago, skipping newsletter", storyId);
                return false;
            }

            // Get story title and summary in the requested locale
            var translation = story.Translations.FirstOrDefault(t => t.LanguageCode == locale) 
                ?? story.Translations.FirstOrDefault();
            
            var title = translation?.Title ?? story.Title;
            var summary = story.Summary ?? "";

            // Build story URL (assuming frontend URL structure)
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "https://alchimalia.com";
            var storyUrl = $"{frontendUrl}/tales-of-alchimalia/story-details/{storyId}";

            // Create newsletter content
            var stories = new List<NewStoryItem>
            {
                new()
                {
                    Title = title,
                    Description = summary,
                    Url = storyUrl,
                    CoverImageUrl = story.CoverImageUrl
                }
            };

            var htmlContent = _templateService.GenerateNewStoriesNewsletterHtml(stories, locale);
            var textContent = _templateService.GenerateNewStoriesNewsletterText(stories, locale);

            // Replace unsubscribe URL placeholder
            var unsubscribeUrl = $"{frontendUrl}/user-menu?unsubscribe=true";
            htmlContent = htmlContent.Replace("{unsubscribe_url}", unsubscribeUrl);
            textContent = textContent.Replace("{unsubscribe_url}", unsubscribeUrl);

            // Send newsletter
            var subject = locale.StartsWith("ro", StringComparison.OrdinalIgnoreCase)
                ? "Noutăți Alchimalia - Poveste nouă!"
                : "Alchimalia News - New Story!";

            var result = await _newsletterSendingService.SendNewsletterToSubscribersAsync(
                subject,
                htmlContent,
                textContent,
                ct);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Newsletter sent successfully for new story {StoryId}: {SentCount} recipients",
                    storyId, result.SentCount);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to send newsletter for story {StoryId}: {ErrorMessage}",
                    storyId, result.ErrorMessage);
            }

            return result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending newsletter for new story {StoryId}: {Error}", storyId, ex.Message);
            return false;
        }
    }

    public async Task<bool> TrySendNewsletterForNewEpicAsync(string epicId, string locale, CancellationToken ct = default)
    {
        try
        {
            // Check if newsletter auto-send is enabled
            var autoSendEnabled = _configuration.GetValue<bool>("Newsletter:AutoSendEnabled", true);
            if (!autoSendEnabled)
            {
                _logger.LogDebug("Newsletter auto-send is disabled. Skipping newsletter for epic {EpicId}", epicId);
                return false;
            }

            // Get the published epic
            var epic = await _db.StoryEpicDefinitions
                .Include(e => e.Translations)
                .FirstOrDefaultAsync(e => e.Id == epicId && e.Status == "published" && e.IsActive, ct);

            if (epic == null)
            {
                _logger.LogWarning("Epic not found or not published: {EpicId}", epicId);
                return false;
            }

            // Check if this is a new epic (Version == 1 means first publish)
            if (epic.Version > 1)
            {
                _logger.LogDebug("Epic {EpicId} is an update (version {Version}), skipping newsletter", epicId, epic.Version);
                return false;
            }

            // Check if epic was created recently (within last hour) to avoid duplicates
            var oneHourAgo = DateTime.UtcNow.AddHours(-1);
            if (epic.CreatedAt < oneHourAgo)
            {
                _logger.LogDebug("Epic {EpicId} was created more than 1 hour ago, skipping newsletter", epicId);
                return false;
            }

            // Get epic name and description in the requested locale
            var translation = epic.Translations.FirstOrDefault(t => t.LanguageCode == locale)
                ?? epic.Translations.FirstOrDefault();

            var name = translation?.Name ?? epic.Name;
            var description = translation?.Description ?? epic.Description ?? "";

            // Build epic URL
            var frontendUrl = _configuration["Frontend:BaseUrl"] ?? "https://alchimalia.com";
            var epicUrl = $"{frontendUrl}/tales-of-alchimalia/epic-details/{epicId}";

            // Create newsletter content (treat epic as a story item)
            var stories = new List<NewStoryItem>
            {
                new()
                {
                    Title = name,
                    Description = description,
                    Url = epicUrl,
                    CoverImageUrl = epic.CoverImageUrl
                }
            };

            var htmlContent = _templateService.GenerateNewStoriesNewsletterHtml(stories, locale);
            var textContent = _templateService.GenerateNewStoriesNewsletterText(stories, locale);

            // Replace unsubscribe URL placeholder
            var unsubscribeUrl = $"{frontendUrl}/user-menu?unsubscribe=true";
            htmlContent = htmlContent.Replace("{unsubscribe_url}", unsubscribeUrl);
            textContent = textContent.Replace("{unsubscribe_url}", unsubscribeUrl);

            // Send newsletter
            var subject = locale.StartsWith("ro", StringComparison.OrdinalIgnoreCase)
                ? "Noutăți Alchimalia - Epic nou!"
                : "Alchimalia News - New Epic!";

            var result = await _newsletterSendingService.SendNewsletterToSubscribersAsync(
                subject,
                htmlContent,
                textContent,
                ct);

            if (result.Success)
            {
                _logger.LogInformation(
                    "Newsletter sent successfully for new epic {EpicId}: {SentCount} recipients",
                    epicId, result.SentCount);
            }
            else
            {
                _logger.LogWarning(
                    "Failed to send newsletter for epic {EpicId}: {ErrorMessage}",
                    epicId, result.ErrorMessage);
            }

            return result.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error sending newsletter for new epic {EpicId}: {Error}", epicId, ex.Message);
            return false;
        }
    }
}
