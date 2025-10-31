using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.DTOs;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;

namespace XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;

public interface IStoriesMarketplaceService
{
    Task<GetMarketplaceStoriesResponse> GetMarketplaceStoriesAsync(Guid userId, string locale, SearchStoriesRequest request);
    Task<PurchaseStoryResponse> PurchaseStoryAsync(Guid userId, PurchaseStoryRequest request);
    Task<GetUserPurchasedStoriesResponse> GetUserPurchasedStoriesAsync(Guid userId, string locale);
    Task<StoryDetailsDto?> GetStoryDetailsAsync(string storyId, Guid userId, string locale);
    Task InitializeMarketplaceAsync(); // Keep for Program.cs startup
}

public class StoriesMarketplaceService : IStoriesMarketplaceService
{
    private readonly IStoriesMarketplaceRepository _repository;
    private readonly XooDbContext _context;

    public StoriesMarketplaceService(IStoriesMarketplaceRepository repository, XooDbContext context)
    {
        _repository = repository;
        _context = context;
    }

    public async Task<GetMarketplaceStoriesResponse> GetMarketplaceStoriesAsync(Guid userId, string locale, SearchStoriesRequest request)
    {
        try
        {
            var stories = await _repository.GetMarketplaceStoriesAsync(userId, locale, request);
            var featuredStories = await _repository.GetFeaturedStoriesAsync(userId, locale);
            var availableRegions = await _repository.GetAvailableRegionsAsync();
            var availableAgeRatings = await _repository.GetAvailableAgeRatingsAsync();
            var availableCharacters = await _repository.GetAvailableCharactersAsync();

            return new GetMarketplaceStoriesResponse
            {
                Stories = stories,
                FeaturedStories = featuredStories,
                AvailableRegions = availableRegions,
                AvailableAgeRatings = availableAgeRatings,
                AvailableCharacters = availableCharacters,
                TotalCount = stories.Count,
                HasMore = stories.Count == request.PageSize
            };
        }
        catch (Exception ex)
        {
            // Log error
            return new GetMarketplaceStoriesResponse
            {
                Stories = new List<StoryMarketplaceItemDto>(),
                FeaturedStories = new List<StoryMarketplaceItemDto>(),
                AvailableRegions = new List<string>(),
                AvailableAgeRatings = new List<string>(),
                AvailableCharacters = new List<string>(),
                TotalCount = 0,
                HasMore = false
            };
        }
    }

    public async Task<PurchaseStoryResponse> PurchaseStoryAsync(Guid userId, PurchaseStoryRequest request)
    {
        try
        {
            // Get story marketplace info to determine price
            var marketplaceInfo = await _context.StoryMarketplaceInfos
                .FirstOrDefaultAsync(smi => smi.StoryId == request.StoryId);

            if (marketplaceInfo == null)
            {
                return new PurchaseStoryResponse
                {
                    Success = false,
                    ErrorMessage = "Story not found in marketplace"
                };
            }

            // Check if already purchased
            var isAlreadyPurchased = await _repository.IsStoryPurchasedAsync(userId, request.StoryId);
            if (isAlreadyPurchased)
            {
                return new PurchaseStoryResponse
                {
                    Success = false,
                    ErrorMessage = "Story already purchased"
                };
            }

            // Attempt purchase
            var purchaseSuccess = await _repository.PurchaseStoryAsync(userId, request.StoryId, marketplaceInfo.PriceInCredits);

            if (!purchaseSuccess)
            {
                // Get current credit balance to provide helpful error message
                var wallet = await _context.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
                var currentBalance = wallet?.DiscoveryBalance ?? 0;

                return new PurchaseStoryResponse
                {
                    Success = false,
                    ErrorMessage = currentBalance < marketplaceInfo.PriceInCredits 
                        ? "Insufficient credits" 
                        : "Purchase failed",
                    RemainingCredits = currentBalance
                };
            }

            // Get updated credit balance
            var updatedWallet = await _context.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            var remainingCredits = updatedWallet?.DiscoveryBalance ?? 0;

            return new PurchaseStoryResponse
            {
                Success = true,
                RemainingCredits = remainingCredits,
                CreditsSpent = marketplaceInfo.PriceInCredits
            };
        }
        catch (Exception ex)
        {
            return new PurchaseStoryResponse
            {
                Success = false,
                ErrorMessage = "An error occurred during purchase"
            };
        }
    }

    public async Task<GetUserPurchasedStoriesResponse> GetUserPurchasedStoriesAsync(Guid userId, string locale)
    {
        try
        {
            var purchasedStories = await _repository.GetUserPurchasedStoriesAsync(userId, locale);

            return new GetUserPurchasedStoriesResponse
            {
                PurchasedStories = purchasedStories,
                TotalCount = purchasedStories.Count
            };
        }
        catch (Exception ex)
        {
            return new GetUserPurchasedStoriesResponse
            {
                PurchasedStories = new List<StoryMarketplaceItemDto>(),
                TotalCount = 0
            };
        }
    }

    public async Task<StoryDetailsDto?> GetStoryDetailsAsync(string storyId, Guid userId, string locale)
    {
        try
        {
            return await _repository.GetStoryDetailsAsync(storyId, userId, locale);
        }
        catch (Exception ex)
        {
            // Log error
            return null;
        }
    }

    public async Task InitializeMarketplaceAsync()
    {
        try
        {
            await _repository.SeedMarketplaceDataAsync();
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }
}
