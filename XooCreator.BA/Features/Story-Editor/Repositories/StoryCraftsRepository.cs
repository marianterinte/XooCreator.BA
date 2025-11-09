using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Repositories;

public class StoryCraftsRepository : IStoryCraftsRepository
{
    private readonly XooDbContext _context;

    public StoryCraftsRepository(XooDbContext context)
    {
        _context = context;
    }

    public async Task<StoryCraft?> GetAsync(string storyId, LanguageCode lang, CancellationToken ct = default)
    {
        var id = (storyId ?? string.Empty).Trim();
        return await _context.StoryCrafts.FirstOrDefaultAsync(x => x.StoryId == id && x.Lang == lang, ct);
    }

    public async Task<StoryCraft> CreateAsync(Guid ownerUserId, string storyId, LanguageCode lang, string status, string json, CancellationToken ct = default)
    {
        var craft = new StoryCraft
        {
            Id = Guid.NewGuid(),
            OwnerUserId = ownerUserId,
            StoryId = (storyId ?? string.Empty).Trim(),
            Lang = lang,
            Status = string.IsNullOrWhiteSpace(status) ? "draft" : status,
            Json = string.IsNullOrWhiteSpace(json) ? "{}" : json,
            UpdatedAt = DateTime.UtcNow
        };
        _context.StoryCrafts.Add(craft);
        await _context.SaveChangesAsync(ct);
        return craft;
    }

    public async Task UpsertAsync(Guid ownerUserId, string storyId, LanguageCode lang, string status, string json, CancellationToken ct = default)
    {
        var existing = await GetAsync(storyId, lang, ct);
        if (existing == null)
        {
            await CreateAsync(ownerUserId, storyId, lang, status, json, ct);
            return;
        }

        existing.OwnerUserId = ownerUserId;
        if (!string.IsNullOrWhiteSpace(status)) existing.Status = status;
        if (!string.IsNullOrWhiteSpace(json)) existing.Json = json;
        existing.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }

    public Task<List<StoryCraft>> ListByOwnerAsync(Guid ownerUserId, CancellationToken ct = default)
        => _context.StoryCrafts
            .Where(x => x.OwnerUserId == ownerUserId)
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

    public Task<List<StoryCraft>> ListAllAsync(CancellationToken ct = default)
        => _context.StoryCrafts
            .OrderByDescending(x => x.UpdatedAt)
            .ToListAsync(ct);

    public async Task SaveAsync(StoryCraft craft, CancellationToken ct = default)
    {
        craft.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
    }
}
