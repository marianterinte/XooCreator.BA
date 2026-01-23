using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.User.DTOs;

namespace XooCreator.BA.Features.User.Services;

public class CreatorTokenService : ICreatorTokenService
{
    private readonly XooDbContext _context;

    public CreatorTokenService(XooDbContext context)
    {
        _context = context;
    }

    public async Task<GetCreatorTokensResponse> GetCreatorTokensAsync(Guid userId, CancellationToken ct)
    {
        var balances = await _context.CreatorTokenBalances
            .Where(b => b.UserId == userId)
            .OrderBy(b => b.TokenType)
            .ThenBy(b => b.TokenValue)
            .Select(b => new CreatorTokenBalanceDto
            {
                TokenType = b.TokenType,
                TokenValue = b.TokenValue,
                Quantity = b.Quantity,
                IsAdminOverride = b.IsAdminOverride,
                OverrideAt = b.OverrideAt
            })
            .ToListAsync(ct);

        return new GetCreatorTokensResponse
        {
            UserId = userId,
            Tokens = balances
        };
    }

    public async Task<CreatorTokenBalanceDto> OverrideTokenAsync(Guid userId, OverrideCreatorTokenRequest request, Guid adminUserId, CancellationToken ct)
    {
        var balance = await _context.CreatorTokenBalances
            .FirstOrDefaultAsync(b => 
                b.UserId == userId && 
                b.TokenType == request.TokenType && 
                b.TokenValue == request.TokenValue, ct);

        if (balance == null)
        {
            balance = new CreatorTokenBalance
            {
                UserId = userId,
                TokenType = request.TokenType,
                TokenValue = request.TokenValue,
                Quantity = request.Quantity,
                IsAdminOverride = true,
                OverrideByUserId = adminUserId,
                OverrideAt = DateTime.UtcNow,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.CreatorTokenBalances.Add(balance);
        }
        else
        {
            balance.Quantity = request.Quantity;
            balance.IsAdminOverride = true;
            balance.OverrideByUserId = adminUserId;
            balance.OverrideAt = DateTime.UtcNow;
            balance.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);

        return new CreatorTokenBalanceDto
        {
            TokenType = balance.TokenType,
            TokenValue = balance.TokenValue,
            Quantity = balance.Quantity,
            IsAdminOverride = balance.IsAdminOverride,
            OverrideAt = balance.OverrideAt
        };
    }

    public async Task<bool> AwardTokenAsync(Guid userId, string tokenType, string tokenValue, int quantity, CancellationToken ct)
    {
        // Only award if not overridden by admin
        var balance = await _context.CreatorTokenBalances
            .FirstOrDefaultAsync(b => 
                b.UserId == userId && 
                b.TokenType == tokenType && 
                b.TokenValue == tokenValue, ct);

        if (balance != null && balance.IsAdminOverride)
        {
            // Don't modify admin overridden values
            return false;
        }

        if (balance == null)
        {
            balance = new CreatorTokenBalance
            {
                UserId = userId,
                TokenType = tokenType,
                TokenValue = tokenValue,
                Quantity = quantity,
                IsAdminOverride = false,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            _context.CreatorTokenBalances.Add(balance);
        }
        else
        {
            balance.Quantity += quantity;
            balance.UpdatedAt = DateTime.UtcNow;
        }

        await _context.SaveChangesAsync(ct);
        return true;
    }
}
