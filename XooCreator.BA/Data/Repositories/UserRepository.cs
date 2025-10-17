using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Repositories;

public interface IUserRepository
{
    Task<AlchimaliaUser?> GetByAuth0IdAsync(string auth0Id, CancellationToken ct = default);
    Task<AlchimaliaUser> EnsureAsync(string auth0Id, string name, string email, string? picture = null, CancellationToken ct = default);
}

public class UserRepository : IUserRepository
{
    private readonly XooDbContext _db;

    public UserRepository(XooDbContext db) => _db = db;

    public Task<AlchimaliaUser?> GetByAuth0IdAsync(string auth0Id, CancellationToken ct = default)
        => _db.AlchimaliaUsers.AsNoTracking().FirstOrDefaultAsync(u => u.Auth0Id == auth0Id, ct);

    public async Task<AlchimaliaUser> EnsureAsync(string auth0Id, string name, string email, string? picture = null, CancellationToken ct = default)
    {
        var user = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Auth0Id == auth0Id, ct);
        if (user != null) 
        {
            // Update profile data and last login
            user.Name = name;
            user.Email = email;
            user.Picture = picture;
            user.LastLoginAt = DateTime.UtcNow;
            user.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
            return user;
        }

        user = new AlchimaliaUser { 
            Id = Guid.NewGuid(), 
            Auth0Id = auth0Id, 
            Name = name, 
            Email = email,
            Picture = picture,
            CreatedAt = DateTime.UtcNow,
            LastLoginAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };
        _db.AlchimaliaUsers.Add(user);
        _db.CreditWallets.Add(new CreditWallet { UserId = user.Id, Balance = 0, UpdatedAt = DateTime.UtcNow });
        
        await _db.SaveChangesAsync(ct);
        return user;
    }
}
