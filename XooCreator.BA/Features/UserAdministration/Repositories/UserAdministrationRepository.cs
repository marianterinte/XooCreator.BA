using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.UserAdministration.Repositories;

public interface IUserAdministrationRepository
{
    Task<List<AlchimaliaUser>> GetAllUsersAsync(CancellationToken ct = default);
    Task<AlchimaliaUser?> GetUserByIdAsync(Guid userId, CancellationToken ct = default);
    Task<bool> UpdateUserRoleAsync(Guid userId, UserRole role, CancellationToken ct = default);
    Task<bool> UpdateUserRolesAsync(Guid userId, List<UserRole> roles, CancellationToken ct = default);
}

public class UserAdministrationRepository : IUserAdministrationRepository
{
    private readonly XooDbContext _db;

    public UserAdministrationRepository(XooDbContext db) => _db = db;

    public async Task<List<AlchimaliaUser>> GetAllUsersAsync(CancellationToken ct = default)
    {
        return await _db.AlchimaliaUsers
            .AsNoTracking()
            .OrderBy(u => u.LastName)
            .ThenBy(u => u.FirstName)
            .ToListAsync(ct);
    }

    public async Task<AlchimaliaUser?> GetUserByIdAsync(Guid userId, CancellationToken ct = default)
    {
        return await _db.AlchimaliaUsers
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Id == userId, ct);
    }

    public async Task<bool> UpdateUserRoleAsync(Guid userId, UserRole role, CancellationToken ct = default)
    {
        // For backward compatibility, convert single role to list
        return await UpdateUserRolesAsync(userId, new List<UserRole> { role }, ct);
    }

    public async Task<bool> UpdateUserRolesAsync(Guid userId, List<UserRole> roles, CancellationToken ct = default)
    {
        var user = await _db.AlchimaliaUsers.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null)
            return false;

        // Update both Role (for backward compatibility) and Roles
        user.Roles = roles;
        // Set Role to first role for backward compatibility, or Reader if empty
        user.Role = roles.Count > 0 ? roles[0] : UserRole.Reader;
        user.UpdatedAt = DateTime.UtcNow;
        
        await _db.SaveChangesAsync(ct);
        return true;
    }
}

