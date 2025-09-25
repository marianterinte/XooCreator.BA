using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Repositories;

public interface IUserRepository
{
    Task<UserAlchimalia?> GetByAuth0SubAsync(string sub, CancellationToken ct = default);
    Task<UserAlchimalia> EnsureAsync(string sub, string displayName, string email, CancellationToken ct = default);
}

public class UserRepository : IUserRepository
{
    private readonly XooDbContext _db;

    public UserRepository(XooDbContext db) => _db = db;

    public Task<UserAlchimalia?> GetByAuth0SubAsync(string sub, CancellationToken ct = default)
        => _db.UsersAlchimalia.AsNoTracking().FirstOrDefaultAsync(u => u.Auth0Sub == sub, ct);

    public async Task<UserAlchimalia> EnsureAsync(string sub, string displayName, string email, CancellationToken ct = default)
    {
        var user = await _db.UsersAlchimalia.FirstOrDefaultAsync(u => u.Auth0Sub == sub, ct);
        if (user != null) return user;

        user = new UserAlchimalia { 
            Id = Guid.NewGuid(), 
            Auth0Sub = sub, 
            DisplayName = displayName, 
            Email = email,
            CreatedAt = DateTime.UtcNow 
        };
        _db.UsersAlchimalia.Add(user);
        _db.CreditWallets.Add(new CreditWallet { UserId = user.Id, Balance = 0, UpdatedAt = DateTime.UtcNow });
        
        // Seed generic balances for the 5 core tokens by default
        _db.UserTokenBalances.AddRange(new []
        {
            new UserTokenBalance { Id = Guid.NewGuid(), UserId = user.Id, Type = "TreeOfHeroes", Value = "courage", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.NewGuid(), UserId = user.Id, Type = "TreeOfHeroes", Value = "curiosity", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.NewGuid(), UserId = user.Id, Type = "TreeOfHeroes", Value = "thinking", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.NewGuid(), UserId = user.Id, Type = "TreeOfHeroes", Value = "creativity", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow },
            new UserTokenBalance { Id = Guid.NewGuid(), UserId = user.Id, Type = "TreeOfHeroes", Value = "safety", Quantity = 5, CreatedAt = DateTime.UtcNow, UpdatedAt = DateTime.UtcNow }
        });
        
        await _db.SaveChangesAsync(ct);
        return user;
    }
}
