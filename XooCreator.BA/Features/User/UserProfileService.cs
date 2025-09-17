using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Repositories;

namespace XooCreator.BA.Features.User;

public interface IUserProfileService
{
    Task<GetUserProfileResponse> GetUserProfileAsync(Guid userId);
    Task<SpendCreditsResponse> SpendCreditsAsync(Guid userId, SpendCreditsRequest request);
    Task<SpendCreditsResponse> SpendDiscoveryCreditsAsync(Guid userId, SpendCreditsRequest request);
    Task<SpendCreditsResponse> SpendGenerativeCreditsAsync(Guid userId, SpendCreditsRequest request);
}

public class UserProfileService : IUserProfileService
{
    private readonly XooDbContext _db;
    private readonly IUserRepository _userRepository;

    public UserProfileService(XooDbContext db, IUserRepository userRepository)
    {
        _db = db;
        _userRepository = userRepository;
    }

    public async Task<GetUserProfileResponse> GetUserProfileAsync(Guid userId)
    {
        try
        {
            var user = await _db.UsersAlchimalia
                .Include(u => u == null) // Force explicit loading
                .FirstOrDefaultAsync(u => u.Id == userId);

            if (user == null)
            {
                return new GetUserProfileResponse
                {
                    Success = false,
                    ErrorMessage = "User not found"
                };
            }

            var wallet = await _db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            var credits = wallet?.Balance ?? 0;

            // Check if user has ever purchased credits
            var hasEverPurchased = await _db.CreditTransactions
                .AnyAsync(t => t.UserId == userId && t.Type == CreditTransactionType.Purchase);

            var lastPurchase = await _db.CreditTransactions
                .Where(t => t.UserId == userId && t.Type == CreditTransactionType.Purchase)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            var lastTransaction = await _db.CreditTransactions
                .Where(t => t.UserId == userId)
                .OrderByDescending(t => t.CreatedAt)
                .FirstOrDefaultAsync();

            // Get base configuration
            var config = await _db.BuilderConfigs.FirstOrDefaultAsync();
            var baseUnlockedAnimalIds = GetBaseUnlockedAnimalIds(config);
            var baseUnlockedBodyPartKeys = GetBaseUnlockedBodyPartKeys(config);

            // Get locked/unlocked parts
            var allParts = await _db.BodyParts
                .Select(p => p.Key)
                .ToListAsync();

            // Logic: If user has ever purchased credits, they have full access
            var hasFullAccess = hasEverPurchased && credits > 0;
            var unlockedParts = hasFullAccess ? allParts : baseUnlockedBodyPartKeys;
            var lockedParts = hasFullAccess ? new List<string>() : allParts.Except(baseUnlockedBodyPartKeys).ToList();

            // Animal count: if full access, all animals, otherwise base count
            var totalAnimals = await _db.Animals.Where(a => !a.IsHybrid).CountAsync();
            var unlockedAnimalCount = hasFullAccess ? totalAnimals : baseUnlockedAnimalIds.Count;

            var userProfile = new UserProfileDto
            {
                Id = user.Id.ToString(),
                DisplayName = user.DisplayName,
                Email = user.Email,
                CreatedAt = user.CreatedAt,
                Credits = new UserCreditsDto
                {
                    Balance = credits,
                    HasEverPurchased = hasEverPurchased,
                    LastPurchaseAt = lastPurchase?.CreatedAt,
                    LastTransactionAt = lastTransaction?.CreatedAt
                },
                Permissions = new UserPermissionsDto
                {
                    HasFullAccess = hasFullAccess,
                    UnlockedAnimalCount = unlockedAnimalCount,
                    UnlockedParts = unlockedParts,
                    LockedParts = lockedParts
                }
            };

            return new GetUserProfileResponse
            {
                User = userProfile,
                Success = true
            };
        }
        catch (Exception ex)
        {
            return new GetUserProfileResponse
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    public async Task<SpendCreditsResponse> SpendCreditsAsync(Guid userId, SpendCreditsRequest request)
    {
        try
        {
            var wallet = await _db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                return new SpendCreditsResponse
                {
                    Success = false,
                    ErrorMessage = "Credit wallet not found"
                };
            }

            if (wallet.Balance < request.Amount)
            {
                return new SpendCreditsResponse
                {
                    Success = false,
                    ErrorMessage = "Insufficient credits",
                    NewBalance = wallet.Balance
                };
            }

            // Deduct credits
            wallet.Balance -= request.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            // Record transaction
            var transaction = new CreditTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -request.Amount, // Negative for spending
                Type = CreditTransactionType.Spend,
                Reference = request.Reference,
                CreatedAt = DateTime.UtcNow
            };

            _db.CreditTransactions.Add(transaction);
            await _db.SaveChangesAsync();

            return new SpendCreditsResponse
            {
                Success = true,
                NewBalance = wallet.Balance
            };
        }
        catch (Exception ex)
        {
            return new SpendCreditsResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                NewBalance = 0
            };
        }
    }

    public async Task<SpendCreditsResponse> SpendDiscoveryCreditsAsync(Guid userId, SpendCreditsRequest request)
    {
        try
        {
            var wallet = await _db.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId);
            if (wallet == null)
            {
                return new SpendCreditsResponse { Success = false, ErrorMessage = "Credit wallet not found", NewBalance = 0 };
            }

            if (wallet.DiscoveryBalance < request.Amount)
            {
                return new SpendCreditsResponse { Success = false, ErrorMessage = "Insufficient discovery credits", NewBalance = wallet.DiscoveryBalance };
            }

            wallet.DiscoveryBalance -= request.Amount;
            wallet.UpdatedAt = DateTime.UtcNow;

            var transaction = new CreditTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Amount = -request.Amount,
                Type = CreditTransactionType.Spend,
                Reference = request.Reference,
                CreatedAt = DateTime.UtcNow
            };

            _db.CreditTransactions.Add(transaction);
            await _db.SaveChangesAsync();

            return new SpendCreditsResponse { Success = true, NewBalance = wallet.DiscoveryBalance };
        }
        catch (Exception ex)
        {
            return new SpendCreditsResponse { Success = false, ErrorMessage = ex.Message, NewBalance = 0 };
        }
    }

    public Task<SpendCreditsResponse> SpendGenerativeCreditsAsync(Guid userId, SpendCreditsRequest request)
    {
        // For now, generative == legacy balance path
        return SpendCreditsAsync(userId, request);
    }

    private static List<string> GetBaseUnlockedAnimalIds(BuilderConfig? config)
    {
        if (config?.BaseUnlockedAnimalIds == null)
            return new List<string> { "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000003" }; // Default: Bunny, Cat, Giraffe

        try
        {
            return JsonSerializer.Deserialize<List<string>>(config.BaseUnlockedAnimalIds) ?? new List<string>();
        }
        catch
        {
            return new List<string> { "00000000-0000-0000-0000-000000000001", "00000000-0000-0000-0000-000000000002", "00000000-0000-0000-0000-000000000003" }; // Fallback
        }
    }

    private static List<string> GetBaseUnlockedBodyPartKeys(BuilderConfig? config)
    {
        if (config?.BaseUnlockedBodyPartKeys == null)
            return new List<string> { "head", "body", "arms" }; // Default: first 3 body parts

        try
        {
            return JsonSerializer.Deserialize<List<string>>(config.BaseUnlockedBodyPartKeys) ?? new List<string>();
        }
        catch
        {
            return new List<string> { "head", "body", "arms" }; // Fallback
        }
    }
}
