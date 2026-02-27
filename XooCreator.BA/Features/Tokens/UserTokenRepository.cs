using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TreeOfHeroes.DTOs;
using XooCreator.BA.Features.TreeOfLight.DTOs;

namespace XooCreator.BA.Features.Tokens;

public interface IUserTokenRepository
{
    /// <summary>
    /// Returns all token balances (all families and values) for the given user,
    /// in the same shape as GET /tree-of-heroes/tokens/all.
    /// </summary>
    Task<List<TokenBalanceItemDto>> GetAllTokenBalancesAsync(Guid userId, CancellationToken ct = default);
}

public class UserTokenRepository : IUserTokenRepository
{
    private readonly XooDbContext _context;

    public UserTokenRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<TokenBalanceItemDto>> GetAllTokenBalancesAsync(Guid userId, CancellationToken ct = default)
    {
        var list = new List<TokenBalanceItemDto>();

        var balances = await _context.UserTokenBalances
            .AsNoTracking()
            .Where(b => b.UserId == userId && b.Quantity > 0)
            .Select(b => new TokenBalanceItemDto { Type = b.Type, Value = b.Value, Quantity = b.Quantity })
            .ToListAsync(ct);

        var discoveryType = TokenFamily.Discovery.ToString();
        const string discoveryValue = "discovery credit";

        // Include all balances from the generic ledger EXCEPT the special Discovery credit entry,
        // which is represented by the CreditWallet balance instead.
        list.AddRange(balances.Where(b =>
            !string.Equals(b.Type, discoveryType, StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(b.Value, discoveryValue, StringComparison.OrdinalIgnoreCase)));

        var wallet = await _context.CreditWallets
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.UserId == userId, ct);
        if (wallet != null && wallet.DiscoveryBalance > 0)
        {
            list.Add(new TokenBalanceItemDto
            {
                Type = discoveryType,
                Value = discoveryValue,
                Quantity = (int)Math.Min(wallet.DiscoveryBalance, int.MaxValue)
            });
        }

        return list;
    }
}

