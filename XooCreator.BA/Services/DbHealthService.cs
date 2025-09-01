using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Services;

public sealed class DbHealthService : IDbHealthService
{
    private readonly XooDbContext _db;

    public DbHealthService(XooDbContext db)
    {
        _db = db;
    }

    public async Task<DbHealthResult> GetAsync(CancellationToken ct)
    {
        var canConnect = await _db.Database.CanConnectAsync(ct);
        var pending = await _db.Database.GetPendingMigrationsAsync(ct);
        return new DbHealthResult(canConnect, pending);
    }
}
