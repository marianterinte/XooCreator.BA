using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
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
    Task EnsureStoryReaderAsync(Guid userId, string storyId, StoryAcquisitionSource source);
    Task<int> GetStoryReadersCountAsync(string storyId);
    Task<List<StoryReadersAggregate>> GetTopStoriesByReadersAsync(int limit);
    Task<List<StoryReadersTrendPoint>> GetReadersTrendAsync(int days);
    Task<List<StoryReadersCorrelationItem>> GetReadersVsReviewsAsync(int limit);
    Task<int> GetTotalReadersAsync();
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
            // Determine price based on story seed or heuristics
            var def = await _context.StoryDefinitions
                .FirstOrDefaultAsync(s => s.StoryId == request.StoryId && s.IsActive);

            if (def == null)
            {
                return new PurchaseStoryResponse
                {
                    Success = false,
                    ErrorMessage = "Story not found"
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
            var price = await _repository.GetComputedPriceAsync(request.StoryId);

            var purchaseSuccess = await _repository.PurchaseStoryAsync(userId, request.StoryId, price);

            if (!purchaseSuccess)
            {
                // Get current credit balance to provide helpful error message
                var wallet = await _context.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
                var currentBalance = wallet?.DiscoveryBalance ?? 0;

                return new PurchaseStoryResponse
                {
                    Success = false,
                    ErrorMessage = currentBalance < price 
                        ? "Insufficient credits" 
                        : "Purchase failed",
                    RemainingCredits = currentBalance
                };
            }

            await _repository.EnsureStoryReaderAsync(userId, request.StoryId, StoryAcquisitionSource.Purchase);

            // Get updated credit balance
            var updatedWallet = await _context.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            var remainingCredits = updatedWallet?.DiscoveryBalance ?? 0;

            return new PurchaseStoryResponse
            {
                Success = true,
                RemainingCredits = remainingCredits,
                CreditsSpent = price
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
            // No-op: marketplace seeding removed
            await Task.CompletedTask;
        }
        catch (Exception ex)
        {
            // Log error
            throw;
        }
    }

    public Task EnsureStoryReaderAsync(Guid userId, string storyId, StoryAcquisitionSource source) =>
        _repository.EnsureStoryReaderAsync(userId, storyId, source);

    public Task<int> GetStoryReadersCountAsync(string storyId) =>
        _repository.GetStoryReadersCountAsync(storyId);

    public Task<List<StoryReadersAggregate>> GetTopStoriesByReadersAsync(int limit) =>
        _repository.GetTopStoriesByReadersAsync(limit);

    public Task<List<StoryReadersTrendPoint>> GetReadersTrendAsync(int days) =>
        _repository.GetReadersTrendAsync(days);

    public Task<List<StoryReadersCorrelationItem>> GetReadersVsReviewsAsync(int limit) =>
        _repository.GetReadersVsReviewsAsync(limit);

    public Task<int> GetTotalReadersAsync() =>
        _repository.GetTotalReadersAsync();
}
