using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public record StoryProgressInfo(string StoryId, int ProgressCount);

public interface IStoriesMarketplaceRepository
{
    Task<List<StoryMarketplaceItemDto>> GetMarketplaceStoriesAsync(Guid userId, string locale, SearchStoriesRequest request);
    Task<List<StoryMarketplaceItemDto>> GetFeaturedStoriesAsync(Guid userId, string locale);
    Task<List<string>> GetAvailableRegionsAsync();
    Task<List<string>> GetAvailableAgeRatingsAsync();
    Task<List<string>> GetAvailableCharactersAsync();
    Task<bool> PurchaseStoryAsync(Guid userId, string storyId, int creditsSpent);
    Task<bool> IsStoryPurchasedAsync(Guid userId, string storyId);
    Task<List<StoryMarketplaceItemDto>> GetUserPurchasedStoriesAsync(Guid userId, string locale);
    Task SeedMarketplaceDataAsync();
    Task<StoryDetailsDto?> GetStoryDetailsAsync(string storyId, Guid userId, string locale);
}

public class StoriesMarketplaceRepository : IStoriesMarketplaceRepository
{
    private readonly XooDbContext _context;

    public StoriesMarketplaceRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<StoryMarketplaceItemDto>> GetMarketplaceStoriesAsync(Guid userId, string locale, SearchStoriesRequest request)
    {
        var query = _context.StoryMarketplaceInfos
            .Include(smi => smi.Story)
                .ThenInclude(s => s.Translations)
            .Where(smi => smi.Story.IsActive);

        // Filtre implicite: doar Published + StoryType = Indie (dacă nu s-au cerut categorii specifice)
        query = query.Where(smi => smi.Story.Status == StoryStatus.Published);
        if (!(request.Categories?.Any() ?? false))
        {
            query = query.Where(smi => smi.Story.StoryType == StoryType.Indie);
        }
        
            // Apply filters
        if (!string.IsNullOrEmpty(request.SearchTerm))
        {
            query = query.Where(smi => 
                smi.Story.Title.Contains(request.SearchTerm) ||
                smi.Story.Translations.Any(t => t.LanguageCode == locale && t.Title.Contains(request.SearchTerm)));
        }

        if (request.Regions.Any())
        {
            query = query.Where(smi => request.Regions.Contains(smi.Region));
        }

        if (request.AgeRatings.Any())
        {
            query = query.Where(smi => request.AgeRatings.Contains(smi.AgeRating));
        }

        if (request.Characters.Any())
        {
            query = query.Where(smi => smi.Characters.Any(c => request.Characters.Contains(c)));
        }

        if (request.Categories.Any())
        {
            query = query.Where(smi => smi.Story.StoryTopic != null && request.Categories.Contains(smi.Story.StoryTopic));
        }

        if (request.Difficulties.Any())
        {
            query = query.Where(smi => request.Difficulties.Contains(smi.Difficulty));
        }

        // Apply completion status filter
        switch (request.CompletionStatus)
        {
            case "completed":
                var completedStoryIds = await _context.UserStoryReadProgress
                    .Where(usp => usp.UserId == userId)
                    .GroupBy(usp => usp.StoryId)
                    .Where(g => g.Count() > 0) // Has progress
                    .Select(g => g.Key)
                    .ToListAsync();
                query = query.Where(smi => completedStoryIds.Contains(smi.StoryId));
                break;
            case "in-progress":
                var inProgressStoryIds = await _context.UserStoryReadProgress
                    .Where(usp => usp.UserId == userId)
                    .Select(usp => usp.StoryId)
                    .Distinct()
                    .ToListAsync();
                query = query.Where(smi => inProgressStoryIds.Contains(smi.StoryId));
                break;
            case "not-started":
                var startedStoryIds = await _context.UserStoryReadProgress
                    .Where(usp => usp.UserId == userId)
                    .Select(usp => usp.StoryId)
                    .Distinct()
                    .ToListAsync();
                query = query.Where(smi => !startedStoryIds.Contains(smi.StoryId));
                break;
        }

        // Apply sorting
        query = request.SortBy switch
        {
            "title" => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.Story.Title) : query.OrderBy(smi => smi.Story.Title),
            "date" => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.CreatedAt) : query.OrderBy(smi => smi.CreatedAt),
            "difficulty" => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.Difficulty) : query.OrderBy(smi => smi.Difficulty),
            "price" => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.PriceInCredits) : query.OrderBy(smi => smi.PriceInCredits),
            _ => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.Story.SortOrder) : query.OrderBy(smi => smi.Story.SortOrder)
        };

        // Apply pagination
        var stories = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        return stories.Select(smi => MapToMarketplaceItemDto(smi, locale)).ToList();
    }

    public async Task<List<StoryMarketplaceItemDto>> GetFeaturedStoriesAsync(Guid userId, string locale)
    {
        var featuredStories = await _context.StoryMarketplaceInfos
            .Include(smi => smi.Story)
                .ThenInclude(s => s.Translations)
            .Where(smi => smi.IsFeatured && smi.Story.IsActive)
            .OrderBy(smi => smi.Story.SortOrder)
            .Take(5)
            .ToListAsync();

        return featuredStories.Select(smi => MapToMarketplaceItemDto(smi, locale)).ToList();
    }

    public async Task<List<string>> GetAvailableRegionsAsync()
    {
        return await _context.StoryMarketplaceInfos
            .Where(smi => smi.Story.IsActive)
            .Select(smi => smi.Region)
            .Distinct()
            .OrderBy(r => r)
            .ToListAsync();
    }

    public async Task<List<string>> GetAvailableAgeRatingsAsync()
    {
        return await _context.StoryMarketplaceInfos
            .Where(smi => smi.Story.IsActive)
            .Select(smi => smi.AgeRating)
            .Distinct()
            .OrderBy(r => r)
            .ToListAsync();
    }

    public async Task<List<string>> GetAvailableCharactersAsync()
    {
        var allCharacters = await _context.StoryMarketplaceInfos
            .Where(smi => smi.Story.IsActive)
            .Select(smi => smi.Characters)
            .ToListAsync();

        return allCharacters
            .SelectMany(c => c)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public async Task<bool> PurchaseStoryAsync(Guid userId, string storyId, int creditsSpent)
    {
        using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            // Check if already purchased
            var existingPurchase = await _context.StoryPurchases
                .FirstOrDefaultAsync(sp => sp.UserId == userId && sp.StoryId == storyId);

            if (existingPurchase != null)
            {
                return false; // Already purchased
            }

            // Deduct credits from user's wallet
            var wallet = await _context.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null || wallet.DiscoveryBalance < creditsSpent)
            {
                return false; // Insufficient credits
            }

            wallet.DiscoveryBalance -= creditsSpent;
            wallet.UpdatedAt = DateTime.UtcNow;

            // Create purchase record
            var purchase = new StoryPurchase
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                StoryId = storyId,
                CreditsSpent = creditsSpent,
                PurchasedAt = DateTime.UtcNow
            };
            _context.StoryPurchases.Add(purchase);

            // Create transaction record
            var creditTransaction = new CreditTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -creditsSpent,
                Type = CreditTransactionType.Spend,
                Reference = $"Story Purchase - {storyId}",
                CreatedAt = DateTime.UtcNow
            };
            _context.CreditTransactions.Add(creditTransaction);

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();
            return true;
        }
        catch
        {
            await transaction.RollbackAsync();
            return false;
        }
    }

    public async Task<bool> IsStoryPurchasedAsync(Guid userId, string storyId)
    {
        return await _context.StoryPurchases
            .AnyAsync(sp => sp.UserId == userId && sp.StoryId == storyId);
    }

    public async Task<List<StoryMarketplaceItemDto>> GetUserPurchasedStoriesAsync(Guid userId, string locale)
    {
        var purchasedStories = await _context.StoryPurchases
            .Include(sp => sp.Story)
                .ThenInclude(s => s.Translations)
            .Where(sp => sp.UserId == userId && sp.Story.IsActive)
            .OrderBy(sp => sp.PurchasedAt)
            .ToListAsync();

        var storyProgress = await _context.UserStoryReadProgress
            .Where(usp => usp.UserId == userId)
            .GroupBy(usp => usp.StoryId)
            .Select(g => new StoryProgressInfo(g.Key, g.Count()))
            .ToListAsync();

        var result = new List<StoryMarketplaceItemDto>();
        foreach (var sp in purchasedStories)
        {
            var marketplaceInfo = await _context.StoryMarketplaceInfos
                .Include(smi => smi.Story)
                    .ThenInclude(s => s.Translations)
                .FirstOrDefaultAsync(smi => smi.StoryId == sp.StoryId);
            
            if (marketplaceInfo != null)
            {
                result.Add(MapToMarketplaceItemDto(marketplaceInfo, locale));
            }
        }
        return result;
    }

    public async Task SeedMarketplaceDataAsync()
    {
        // This will be implemented to seed marketplace data for existing stories
        // For now, we'll create a basic implementation
        var existingStories = await _context.StoryDefinitions
            .Where(s => s.IsActive)
            .ToListAsync();

        var existingMarketplaceInfos = await _context.StoryMarketplaceInfos
            .Select(smi => smi.StoryId)
            .ToListAsync();

        var newMarketplaceInfos = new List<StoryMarketplaceInfo>();

        foreach (var story in existingStories)
        {
            if (!existingMarketplaceInfos.Contains(story.StoryId))
            {
                var region = ExtractRegionFromStoryId(story.StoryId);
                var ageRating = DetermineAgeRating(story.StoryId);
                var difficulty = DetermineDifficulty(story.StoryId);
                var characters = ExtractCharactersFromStoryId(story.StoryId);
                var price = DeterminePrice(story.StoryId);

                newMarketplaceInfos.Add(new StoryMarketplaceInfo
                {
                    Id = Guid.NewGuid(),
                    StoryId = story.StoryId,
                    PriceInCredits = price,
                    Region = region,
                    AgeRating = ageRating,
                    Difficulty = difficulty,
                    Characters = characters,
                    Tags = story.StoryTopic != null ? new List<string> { story.StoryTopic } : new List<string>(),
                    IsFeatured = story.StoryId.Contains("intro") || story.StoryId.Contains("loi"),
                    IsNew = false,
                    EstimatedReadingTime = CalculateReadingTime(story.StoryId),
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow
                });
            }
        }

        if (newMarketplaceInfos.Any())
        {
            _context.StoryMarketplaceInfos.AddRange(newMarketplaceInfos);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<StoryDetailsDto?> GetStoryDetailsAsync(string storyId, Guid userId, string locale)
    {
        var marketplaceInfo = await _context.StoryMarketplaceInfos
            .Include(smi => smi.Story)
                .ThenInclude(s => s.Translations)
            .Include(smi => smi.Story)
                .ThenInclude(s => s.Tiles)
            .FirstOrDefaultAsync(smi => smi.StoryId == storyId && smi.Story.IsActive);

        if (marketplaceInfo == null)
            return null;

        // Check if user has purchased this story
        var isPurchased = await _context.StoryPurchases
            .AnyAsync(sp => sp.UserId == userId && sp.StoryId == storyId);

        // Check if user owns this story (UserOwnedStories)
        var ownedRow = await _context.UserOwnedStories
            .AnyAsync(uos => uos.UserId == userId && uos.StoryDefinitionId == marketplaceInfo.Story.Id);
        var isOwned = isPurchased || ownedRow;

        // Get user's story progress
        var storyProgress = await _context.UserStoryReadProgress
            .Where(usp => usp.UserId == userId && usp.StoryId == storyId)
            .CountAsync();

        var totalTiles = marketplaceInfo.Story.Tiles.Count;
        var progressPercentage = totalTiles > 0 ? (int)((double)storyProgress / totalTiles * 100) : 0;
        var isCompleted = progressPercentage >= 100;

        return MapToStoryDetailsDto(marketplaceInfo, locale, isPurchased, isOwned, isCompleted, progressPercentage);
    }

    private StoryDetailsDto MapToStoryDetailsDto(StoryMarketplaceInfo smi, string locale, bool isPurchased, bool isOwned, bool isCompleted, int progressPercentage)
    {
        var translation = smi.Story.Translations.FirstOrDefault(t => t.LanguageCode == locale);
        var title = translation?.Title ?? smi.Story.Title;
        string? authorName = null;
        if (smi.Story.CreatedBy.HasValue)
        {
            authorName = _context.Set<AlchimaliaUser>()
                .Where(u => u.Id == smi.Story.CreatedBy.Value)
                .Select(u => u.Name)
                .FirstOrDefault();
        }

        return new StoryDetailsDto
        {
            Id = smi.StoryId,
            Title = title,
            CoverImageUrl = smi.Story.CoverImageUrl,
            CreatedBy = smi.Story.CreatedBy,
            CreatedByName = authorName,
            Summary = GenerateDetailedSummary(smi.Story, locale),
            PriceInCredits = smi.PriceInCredits,
            Region = smi.Region,
            AgeRating = smi.AgeRating,
            Difficulty = smi.Difficulty,
            Characters = smi.Characters,
            Tags = smi.Tags,
            IsFeatured = smi.IsFeatured,
            IsNew = smi.IsNew,
            EstimatedReadingTime = smi.EstimatedReadingTime,
            IsPurchased = isPurchased,
            IsOwned = isOwned,
            IsCompleted = isCompleted,
            ProgressPercentage = progressPercentage,
            CreatedAt = smi.CreatedAt,
            UnlockedStoryHeroes = new List<string>(), // TODO: Implement if needed
            StoryTopic = smi.Story.StoryTopic,
            StoryType = smi.Story.StoryType.ToString(),
            Status = smi.Story.Status.ToString(),
            SortOrder = smi.Story.SortOrder,
            IsActive = smi.Story.IsActive,
            UpdatedAt = smi.Story.UpdatedAt,
            UpdatedBy = smi.Story.UpdatedBy
        };
    }

    private string GenerateDetailedSummary(StoryDefinition story, string locale)
    {
        // Generate a more detailed summary for story details
        var firstTile = story.Tiles.FirstOrDefault();
        if (firstTile?.Translations?.FirstOrDefault(t => t.LanguageCode == locale)?.Text != null)
        {
            var text = firstTile.Translations.First(t => t.LanguageCode == locale).Text;
            return text.Length > 300 ? text.Substring(0, 300) + "..." : text;
        }
        
        var topicText = story.StoryTopic ?? "Unknown";
        return $"Discover the adventures in {story.Title} - {topicText}. This story contains {story.Tiles.Count} interactive tiles.";
    }

    private StoryMarketplaceItemDto MapToMarketplaceItemDto(StoryMarketplaceInfo smi, string locale)
    {
        var translation = smi.Story.Translations.FirstOrDefault(t => t.LanguageCode == locale);
        var title = translation?.Title ?? smi.Story.Title;

        return new StoryMarketplaceItemDto
        {
            Id = smi.StoryId,
            Title = title,
            CoverImageUrl = smi.Story.CoverImageUrl,
            CreatedBy = smi.Story.CreatedBy,
            Summary = GenerateSummary(smi.Story, locale),
            PriceInCredits = smi.PriceInCredits,
            AgeRating = smi.AgeRating,
            Characters = smi.Characters,
            CreatedAt = smi.CreatedAt,
            StoryTopic = smi.Story.StoryTopic,
            StoryType = smi.Story.StoryType.ToString(),
            Status = smi.Story.Status.ToString()
        };
    }

    private string ExtractRegionFromStoryId(string storyId)
    {
        var regionMap = new Dictionary<string, string>
        {
            { "lunaria", "Lunaria" },
            { "terra", "Terra" },
            { "aetherion", "Aetherion" },
            { "auroria", "Auroria" },
            { "crystalia", "Crystalia" },
            { "pyron", "Pyron" },
            { "zephyra", "Zephyra" },
            { "verdantia", "Verdantia" },
            { "sylvaria", "Sylvaria" },
            { "nocturna", "Nocturna" },
            { "neptunia", "Neptunia" },
            { "oceanica", "Oceanica" },
            { "mechanika", "Mechanika" }
        };

        foreach (var kvp in regionMap)
        {
            if (storyId.Contains(kvp.Key))
                return kvp.Value;
        }

        return "Unknown";
    }

    private string DetermineAgeRating(string storyId)
    {
        if (storyId.Contains("intro") || storyId.Contains("loi"))
            return "5+";
        return "8+";
    }

    private string DetermineDifficulty(string storyId)
    {
        if (storyId.Contains("intro") || storyId.Contains("loi"))
            return "beginner";
        return "intermediate";
    }

    private List<string> ExtractCharactersFromStoryId(string storyId)
    {
        var characters = new List<string>();
        
        if (storyId.Contains("puf") || storyId.Contains("loi"))
        {
            characters.Add("Puf-Puf");
            characters.Add("Emperor Pufus");
        }
        
        if (storyId.Contains("linkaro"))
            characters.Add("Linkaro");
            
        if (storyId.Contains("grubot"))
            characters.Add("Grubot");

        return characters;
    }

    private int DeterminePrice(string storyId)
    {
        // Special case: learn-to-read-s1 should be free
        if (storyId == "learn-to-read-s1")
            return 0;
            
        // Based on TODO: "Stories de pe lunaria 1 da 1 token, cealalta 2 tokeni"
        if (storyId.Contains("lunaria"))
            return 1;
        return 2;
    }

    private int CalculateReadingTime(string storyId)
    {
        // Estimate reading time based on story type
        if (storyId.Contains("intro"))
            return 15; // Longer intro stories
        if (storyId.Contains("s1"))
            return 10; // Season 1 stories
        if (storyId.Contains("s2"))
            return 12; // Season 2 stories
        return 8; // Default reading time
    }

    private string GenerateSummary(StoryDefinition story, string locale)
    {
        // Generate a simple summary based on story title and category
        var topicText = story.StoryTopic ?? "Unknown";
        return $"Discover the adventures in {story.Title} - {topicText}";
    }
}
