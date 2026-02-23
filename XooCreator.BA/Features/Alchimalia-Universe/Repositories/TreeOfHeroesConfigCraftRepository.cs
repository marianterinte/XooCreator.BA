using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Repositories;

public class TreeOfHeroesConfigCraftRepository : ITreeOfHeroesConfigCraftRepository
{
    private readonly XooDbContext _context;

    public TreeOfHeroesConfigCraftRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<TreeOfHeroesConfigCraft?> GetAsync(Guid configId, CancellationToken ct = default)
    {
        return await _context.TreeOfHeroesConfigCrafts
            .FirstOrDefaultAsync(x => x.Id == configId, ct);
    }

    public async Task<TreeOfHeroesConfigCraft?> GetWithDetailsAsync(Guid configId, CancellationToken ct = default)
    {
        return await _context.TreeOfHeroesConfigCrafts
            .Include(x => x.Nodes)
            .Include(x => x.Edges)
            .FirstOrDefaultAsync(x => x.Id == configId, ct);
    }

    public async Task<List<TreeOfHeroesConfigCraft>> ListAsync(string? status = null, CancellationToken ct = default)
    {
        var query = _context.TreeOfHeroesConfigCrafts.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status);
        return await query.OrderByDescending(x => x.UpdatedAt).ToListAsync(ct);
    }

    public async Task<int> CountAsync(string? status = null, CancellationToken ct = default)
    {
        var query = _context.TreeOfHeroesConfigCrafts.AsQueryable();
        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(x => x.Status == status);
        return await query.CountAsync(ct);
    }

    public async Task<TreeOfHeroesConfigCraft> CreateAsync(TreeOfHeroesConfigCraft config, CancellationToken ct = default)
    {
        config.CreatedAt = DateTime.UtcNow;
        config.UpdatedAt = DateTime.UtcNow;
        _context.TreeOfHeroesConfigCrafts.Add(config);
        await _context.SaveChangesAsync(ct);
        return config;
    }

    public async Task SaveAsync(TreeOfHeroesConfigCraft config, CancellationToken ct = default)
    {
        var existing = await _context.TreeOfHeroesConfigCrafts
            .Include(x => x.Nodes)
            .Include(x => x.Edges)
            .FirstOrDefaultAsync(x => x.Id == config.Id, ct);

        if (existing == null)
        {
            _context.TreeOfHeroesConfigCrafts.Add(config);
        }
        else
        {
            _context.TreeOfHeroesConfigCrafts.Update(config);
        }

        config.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }
}
