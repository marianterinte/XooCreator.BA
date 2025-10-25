using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.Stories;

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
            .Include(smi => smi.Story)
                .ThenInclude(s => s.Tiles)
            .Where(smi => smi.Story.IsActive);

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
            query = query.Where(smi => request.Categories.Contains(smi.Story.Category));
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

        // Get user's purchased stories
        var purchasedStoryIds = await _context.StoryPurchases
            .Where(sp => sp.UserId == userId)
            .Select(sp => sp.StoryId)
            .ToListAsync();

        // Get user's story progress
        var storyProgress = await _context.UserStoryReadProgress
            .Where(usp => usp.UserId == userId)
            .GroupBy(usp => usp.StoryId)
            .Select(g => new StoryProgressInfo(g.Key, g.Count()))
            .ToListAsync();

        return stories.Select(smi => MapToMarketplaceItemDto(smi, locale, purchasedStoryIds.Contains(smi.StoryId), storyProgress)).ToList();
    }

    public async Task<List<StoryMarketplaceItemDto>> GetFeaturedStoriesAsync(Guid userId, string locale)
    {
        var featuredStories = await _context.StoryMarketplaceInfos
            .Include(smi => smi.Story)
                .ThenInclude(s => s.Translations)
            .Include(smi => smi.Story)
                .ThenInclude(s => s.Tiles)
            .Where(smi => smi.IsFeatured && smi.Story.IsActive)
            .OrderBy(smi => smi.Story.SortOrder)
            .Take(5)
            .ToListAsync();

        var purchasedStoryIds = await _context.StoryPurchases
            .Where(sp => sp.UserId == userId)
            .Select(sp => sp.StoryId)
            .ToListAsync();

        var storyProgress = await _context.UserStoryReadProgress
            .Where(usp => usp.UserId == userId)
            .GroupBy(usp => usp.StoryId)
            .Select(g => new StoryProgressInfo(g.Key, g.Count()))
            .ToListAsync();

        return featuredStories.Select(smi => MapToMarketplaceItemDto(smi, locale, purchasedStoryIds.Contains(smi.StoryId), storyProgress)).ToList();
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
            .Include(sp => sp.Story)
                .ThenInclude(s => s.Tiles)
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
                .Include(smi => smi.Story)
                    .ThenInclude(s => s.Tiles)
                .FirstOrDefaultAsync(smi => smi.StoryId == sp.StoryId);
            
            if (marketplaceInfo != null)
            {
                result.Add(MapToMarketplaceItemDto(marketplaceInfo, locale, true, storyProgress));
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
                    Tags = new List<string> { story.Category },
                    IsFeatured = story.StoryId.Contains("intro") || story.StoryId.Contains("loi"),
                    IsNew = false,
                    EstimatedReadingTime = CalculateReadingTime(story.Tiles.Count),
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

    private StoryMarketplaceItemDto MapToMarketplaceItemDto(StoryMarketplaceInfo smi, string locale, bool isPurchased, List<StoryProgressInfo> storyProgress)
    {
        var translation = smi.Story.Translations.FirstOrDefault(t => t.LanguageCode == locale);
        var title = translation?.Title ?? smi.Story.Title;
        
        var progress = storyProgress.FirstOrDefault(sp => sp.StoryId == smi.StoryId);
        var progressCount = progress?.ProgressCount ?? 0;
        var totalTiles = smi.Story.Tiles.Count;
        var progressPercentage = totalTiles > 0 ? (int)((double)progressCount / totalTiles * 100) : 0;
        var isCompleted = progressPercentage >= 100;

        return new StoryMarketplaceItemDto
        {
            Id = smi.StoryId,
            Title = title,
            CoverImageUrl = smi.Story.CoverImageUrl,
            Summary = GenerateSummary(smi.Story, locale),
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
            IsCompleted = isCompleted,
            ProgressPercentage = progressPercentage,
            CreatedAt = smi.CreatedAt,
            UnlockedStoryHeroes = new List<string>() // TODO: Add UnlockedStoryHeroes to StoryDefinition if needed
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
        // Based on TODO: "Stories de pe lunaria 1 da 1 token, cealalta 2 tokeni"
        if (storyId.Contains("lunaria"))
            return 1;
        return 2;
    }

    private int CalculateReadingTime(int tileCount)
    {
        // Estimate 2 minutes per tile
        return tileCount * 2;
    }

    private string GenerateSummary(StoryDefinition story, string locale)
    {
        // Generate a simple summary based on the first tile
        var firstTile = story.Tiles.FirstOrDefault();
        if (firstTile?.Translations?.FirstOrDefault(t => t.LanguageCode == locale)?.Text != null)
        {
            var text = firstTile.Translations.First(t => t.LanguageCode == locale).Text;
            return text.Length > 150 ? text.Substring(0, 150) + "..." : text;
        }
        
        return $"Discover the adventures in {story.Title}";
    }
}
