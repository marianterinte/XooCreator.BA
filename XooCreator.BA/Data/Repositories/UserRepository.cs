using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Repositories;

public interface IUserRepository
{
    Task<UserAlchimalia?> GetByAuth0SubAsync(string sub, CancellationToken ct = default);
    Task<UserAlchimalia> EnsureAsync(string sub, string displayName, CancellationToken ct = default);
}

public class UserRepository : IUserRepository
{
    private readonly XooDbContext _db;

    public UserRepository(XooDbContext db) => _db = db;

    public Task<UserAlchimalia?> GetByAuth0SubAsync(string sub, CancellationToken ct = default)
        => _db.UsersAlchimalia.AsNoTracking().FirstOrDefaultAsync(u => u.Auth0Sub == sub, ct);

    public async Task<UserAlchimalia> EnsureAsync(string sub, string displayName, CancellationToken ct = default)
    {
        var user = await _db.UsersAlchimalia.FirstOrDefaultAsync(u => u.Auth0Sub == sub, ct);
        if (user != null) return user;

        user = new UserAlchimalia { Id = Guid.NewGuid(), Auth0Sub = sub, DisplayName = displayName, CreatedAt = DateTime.UtcNow };
        _db.UsersAlchimalia.Add(user);
        _db.CreditWallets.Add(new CreditWallet { UserId = user.Id, Balance = 0, UpdatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync(ct);
        return user;
    }
}
