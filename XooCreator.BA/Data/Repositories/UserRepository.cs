using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Repositories;

public interface IUserRepository
{
    Task<User?> GetByAuth0SubAsync(string sub, CancellationToken ct = default);
    Task<User> EnsureAsync(string sub, string displayName, CancellationToken ct = default);
}

public class UserRepository : IUserRepository
{
    private readonly XooDbContext _db;

    public UserRepository(XooDbContext db) => _db = db;

    public Task<User?> GetByAuth0SubAsync(string sub, CancellationToken ct = default)
        => _db.Users.AsNoTracking().FirstOrDefaultAsync(u => u.Auth0Sub == sub, ct);

    public async Task<User> EnsureAsync(string sub, string displayName, CancellationToken ct = default)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Auth0Sub == sub, ct);
        if (user != null) return user;

        user = new User { Id = Guid.NewGuid(), Auth0Sub = sub, DisplayName = displayName, CreatedAt = DateTime.UtcNow };
        _db.Users.Add(user);
        _db.CreditWallets.Add(new CreditWallet { UserId = user.Id, Balance = 0, UpdatedAt = DateTime.UtcNow });
        await _db.SaveChangesAsync(ct);
        return user;
    }
}
