using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Data.SeedData.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Mappers;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

public record StoryProgressInfo(string StoryId, int ProgressCount);

public interface IStoriesMarketplaceRepository
{
    Task<List<StoryMarketplaceItemDto>> GetMarketplaceStoriesAsync(Guid userId, string locale, SearchStoriesRequest request);
    Task<List<StoryMarketplaceItemDto>> GetFeaturedStoriesAsync(Guid userId, string locale);
    Task<List<string>> GetAvailableRegionsAsync();
    Task<List<string>> GetAvailableAgeRatingsAsync();
    Task<List<string>> GetAvailableCharactersAsync();
    Task<bool> PurchaseStoryAsync(Guid userId, string storyId, double creditsSpent);
    Task<bool> IsStoryPurchasedAsync(Guid userId, string storyId);
    Task<List<StoryMarketplaceItemDto>> GetUserPurchasedStoriesAsync(Guid userId, string locale);
    Task<StoryDetailsDto?> GetStoryDetailsAsync(string storyId, Guid userId, string locale);
    Task<double> GetComputedPriceAsync(string storyId);
}

public class StoriesMarketplaceRepository : IStoriesMarketplaceRepository
{
    private readonly XooDbContext _context;
    private readonly StoryDetailsMapper _storyDetailsMapper;

    public StoriesMarketplaceRepository(XooDbContext context, StoryDetailsMapper storyDetailsMapper)
    {
        _context = context;
        _storyDetailsMapper = storyDetailsMapper;
    }

    public async Task<List<StoryMarketplaceItemDto>> GetMarketplaceStoriesAsync(Guid userId, string locale, SearchStoriesRequest request)
    {
        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();
        
        var query = _context.StoryDefinitions
            .Include(s => s.Translations)
            .Where(s => s.IsActive);

        // Filtre implicite: doar Published + StoryType = Indie (dacă nu s-au cerut categorii specifice)
        query = query.Where(s => s.Status == StoryStatus.Published);
        if (!(request.Categories?.Any() ?? false))
        {
            query = query.Where(s => s.StoryType == StoryType.Indie);
        }



        // Apply filters
        //if (!string.IsNullOrEmpty(request.SearchTerm))
        //{
        //    query = query.Where(smi => 
        //        smi.Story.Title.Contains(request.SearchTerm) ||
        //        smi.Story.Translations.Any(t => t.LanguageCode == normalizedLocale && t.Title.Contains(request.SearchTerm)));
        //}

        //if (request.Regions.Any())
        //{
        //    query = query.Where(smi => request.Regions.Contains(smi.Region));
        //}

        //if (request.AgeRatings.Any())
        //{
        //    query = query.Where(smi => request.AgeRatings.Contains(smi.AgeRating));
        //}

        //if (request.Characters.Any())
        //{
        //    query = query.Where(smi => smi.Characters.Any(c => request.Characters.Contains(c)));
        //}

        //if (request.Categories.Any())
        //{
        //    query = query.Where(smi => smi.Story.StoryTopic != null && request.Categories.Contains(smi.Story.StoryTopic));
        //}

        //if (request.Difficulties.Any())
        //{
        //    query = query.Where(smi => request.Difficulties.Contains(smi.Difficulty));
        //}

        //// Apply completion status filter
        //switch (request.CompletionStatus)
        //{
        //    case "completed":
        //        var completedStoryIds = await _context.UserStoryReadProgress
        //            .Where(usp => usp.UserId == userId)
        //            .GroupBy(usp => usp.StoryId)
        //            .Where(g => g.Count() > 0) // Has progress
        //            .Select(g => g.Key)
        //            .ToListAsync();
        //        query = query.Where(smi => completedStoryIds.Contains(smi.StoryId));
        //        break;
        //    case "in-progress":
        //        var inProgressStoryIds = await _context.UserStoryReadProgress
        //            .Where(usp => usp.UserId == userId)
        //            .Select(usp => usp.StoryId)
        //            .Distinct()
        //            .ToListAsync();
        //        query = query.Where(smi => inProgressStoryIds.Contains(smi.StoryId));
        //        break;
        //    case "not-started":
        //        var startedStoryIds = await _context.UserStoryReadProgress
        //            .Where(usp => usp.UserId == userId)
        //            .Select(usp => usp.StoryId)
        //            .Distinct()
        //            .ToListAsync();
        //        query = query.Where(smi => !startedStoryIds.Contains(smi.StoryId));
        //        break;
        //}

        // Apply sorting
        //query = request.SortBy switch
        //{
        //    "title" => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.Story.Title) : query.OrderBy(smi => smi.Story.Title),
        //    "date" => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.CreatedAt) : query.OrderBy(smi => smi.CreatedAt),
        //    "difficulty" => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.Difficulty) : query.OrderBy(smi => smi.Difficulty),
        //    "price" => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.PriceInCredits) : query.OrderBy(smi => smi.PriceInCredits),
        //    _ => request.SortOrder == "desc" ? query.OrderByDescending(smi => smi.Story.SortOrder) : query.OrderBy(smi => smi.Story.SortOrder)
        //};

        // Apply pagination
        var stories = await query
            .Skip((request.Page - 1) * request.PageSize)
            .Take(request.PageSize)
            .ToListAsync();

        // Map from StoryDefinition directly
        return await MapToMarketplaceListAsync(stories, normalizedLocale, userId);
    }

    public async Task<List<StoryMarketplaceItemDto>> GetFeaturedStoriesAsync(Guid userId, string locale)
    {
        // Normalize locale to lowercase (e.g., "ro-RO" -> "ro-ro") to match database LanguageCode format
        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();
        
        var featuredStories = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Where(s => s.IsActive && s.Status == StoryStatus.Published)
            .OrderBy(s => s.SortOrder)
            .Take(5)
            .ToListAsync();

        return await MapToMarketplaceListAsync(featuredStories, normalizedLocale, userId);
    }

    public async Task<List<string>> GetAvailableRegionsAsync()
    {
        var ids = await _context.StoryDefinitions
            .Where(s => s.IsActive && s.Status == StoryStatus.Published)
            .Select(s => s.StoryId)
            .ToListAsync();
        return ids.Select(ExtractRegionFromStoryId)
            .Distinct()
            .OrderBy(r => r)
            .ToList();
    }

    public async Task<List<string>> GetAvailableAgeRatingsAsync()
    {
        var ids = await _context.StoryDefinitions
            .Where(s => s.IsActive && s.Status == StoryStatus.Published)
            .Select(s => s.StoryId)
            .ToListAsync();
        return ids.Select(DetermineAgeRating)
            .Distinct()
            .OrderBy(r => r)
            .ToList();
    }

    public async Task<List<string>> GetAvailableCharactersAsync()
    {
        var ids = await _context.StoryDefinitions
            .Where(s => s.IsActive && s.Status == StoryStatus.Published)
            .Select(s => s.StoryId)
            .ToListAsync();
        return ids.SelectMany(ExtractCharactersFromStoryId)
            .Distinct()
            .OrderBy(c => c)
            .ToList();
    }

    public async Task<bool> PurchaseStoryAsync(Guid userId, string storyId, double creditsSpent)
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

        var ids = purchasedStories.Select(sp => sp.StoryId).ToList();
        var defs = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Where(s => ids.Contains(s.StoryId))
            .ToListAsync();
        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();
        return await MapToMarketplaceListAsync(defs, normalizedLocale, userId);
    }

    public async Task<StoryDetailsDto?> GetStoryDetailsAsync(string storyId, Guid userId, string locale)
    {
        var def = await _context.StoryDefinitions
            .Include(s => s.Translations)
            .Include(s => s.Tiles)
            .FirstOrDefaultAsync(s => s.StoryId == storyId && s.IsActive && s.Status == StoryStatus.Published);

        if (def == null)
            return null;

        // Check if user has purchased this story
        var isPurchased = await _context.StoryPurchases
            .AnyAsync(sp => sp.UserId == userId && sp.StoryId == storyId);

        // Check if user owns this story (UserOwnedStories)
        var ownedRow = await _context.UserOwnedStories
            .AnyAsync(uos => uos.UserId == userId && uos.StoryDefinitionId == def.Id);
        var isOwned = isPurchased || ownedRow;

        // Get user's story progress
        var storyProgress = await _context.UserStoryReadProgress
            .Where(usp => usp.UserId == userId && usp.StoryId == storyId)
            .CountAsync();

        var totalTiles = def.Tiles.Count;
        var progressPercentage = totalTiles > 0 ? (int)((double)storyProgress / totalTiles * 100) : 0;
        var isCompleted = progressPercentage >= 100;

        // Normalize locale to lowercase for mapping
        var normalizedLocale = (locale ?? "ro-ro").ToLowerInvariant();
        return await _storyDetailsMapper.MapToStoryDetailsFromDefinitionAsync(def, normalizedLocale, isPurchased, isOwned, isCompleted, progressPercentage, userId);
    }

    // Removed old MapToStoryDetailsDto using StoryMarketplaceInfo

    private string? GetSummaryFromJson(string storyId, string locale)
    {
        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var candidates = new[]
            {
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "seed@alchimalia.com", "independent", "i18n", locale, $"{storyId}.json"),
                Path.Combine(baseDir, "Data", "SeedData", "Stories", "seed@alchimalia.com", "i18n", locale, $"{storyId}.json")
            };

            foreach (var filePath in candidates)
            {
                if (!File.Exists(filePath)) continue;
                
                var json = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<StorySeedDataJsonProbe>(json, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    PropertyNameCaseInsensitive = true
                });
                
                if (!string.IsNullOrWhiteSpace(data?.Summary))
                {
                    return data.Summary;
                }
            }
        }
        catch { }
        
        return null;
    }

    private sealed class StorySeedDataJsonProbe
    {
        public string? Summary { get; set; }
    }

    // Removed old MapToMarketplaceItemDto using StoryMarketplaceInfo

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

    // New helpers for mapping from StoryDefinition directly
    private async Task<List<StoryMarketplaceItemDto>> MapToMarketplaceListAsync(List<StoryDefinition> defs, string locale, Guid userId)
    {
        var result = new List<StoryMarketplaceItemDto>();
        foreach (var def in defs)
        {
            result.Add(await MapToMarketplaceItemFromDefinitionAsync(def, locale, userId));
        }
        return result;
    }

    private async Task<StoryMarketplaceItemDto> MapToMarketplaceItemFromDefinitionAsync(StoryDefinition def, string locale, Guid userId)
    {
        var translation = def.Translations?.FirstOrDefault(t => t.LanguageCode == locale);
        var title = translation?.Title ?? def.Title;

        // Extract available languages from translations
        var availableLanguages = def.Translations?
            .Select(t => t.LanguageCode)
            .OrderBy(l => l)
            .ToList() ?? new List<string>();

        // Get author name from database
        string? authorName = null;
        if (def.CreatedBy.HasValue)
        {
            authorName = await _context.Set<AlchimaliaUser>()
                .Where(u => u.Id == def.CreatedBy.Value)
                .Select(u => u.Name)
                .FirstOrDefaultAsync();
        }

        // Get summary from JSON file for the current locale, or use StoryDefinition.Summary, or empty string
        var summary = GetSummaryFromJson(def.StoryId, locale) 
            ?? def.Summary 
            ?? string.Empty;

        // Check if user has purchased this story
        var isPurchased = await _context.StoryPurchases
            .AnyAsync(sp => sp.UserId == userId && sp.StoryId == def.StoryId);

        // Check if user owns this story (UserOwnedStories)
        var ownedRow = await _context.UserOwnedStories
            .AnyAsync(uos => uos.UserId == userId && uos.StoryDefinitionId == def.Id);
        var isOwned = isPurchased || ownedRow;

        return new StoryMarketplaceItemDto
        {
            Id = def.StoryId,
            Title = title,
            CoverImageUrl = def.CoverImageUrl,
            CreatedBy = def.CreatedBy,
            CreatedByName = authorName,
            Summary = summary,
            PriceInCredits = def.PriceInCredits,
            AgeRating = DetermineAgeRating(def.StoryId),
            Characters = ExtractCharactersFromStoryId(def.StoryId),
            CreatedAt = def.CreatedAt,
            StoryTopic = def.StoryTopic,
            StoryType = def.StoryType.ToString(),
            Status = def.Status.ToString(),
            AvailableLanguages = availableLanguages,
            IsPurchased = isPurchased,
            IsOwned = isOwned
        };
    }


    public async Task<double> GetComputedPriceAsync(string storyId)
    {
        var def = await _context.StoryDefinitions
            .FirstOrDefaultAsync(s => s.StoryId == storyId);
        return def?.PriceInCredits ?? 0;
    }

}
