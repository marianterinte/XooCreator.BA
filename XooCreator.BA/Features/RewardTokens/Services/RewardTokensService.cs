using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.RewardTokens.DTOs;

namespace XooCreator.BA.Features.RewardTokens.Services;

public class RewardTokensService : IRewardTokensService
{
    private readonly XooDbContext _context;

    public RewardTokensService(XooDbContext context)
    {
        _context = context;
    }

    public async Task<List<RewardTokenDto>> GetActiveTokensAsync(CancellationToken ct = default)
    {
        return await _context.RewardTokenDefinitions
            .Where(t => t.IsActive)
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.Type)
            .ThenBy(t => t.Value)
            .Select(t => new RewardTokenDto
            {
                Id = t.Id,
                Type = t.Type,
                Value = t.Value,
                DisplayNameKey = t.DisplayNameKey,
                Icon = t.Icon,
                SortOrder = t.SortOrder,
                IsActive = t.IsActive
            })
            .ToListAsync(ct);
    }

    public async Task<List<RewardTokenDto>> GetAllTokensAsync(CancellationToken ct = default)
    {
        return await _context.RewardTokenDefinitions
            .OrderBy(t => t.SortOrder)
            .ThenBy(t => t.Type)
            .ThenBy(t => t.Value)
            .Select(t => new RewardTokenDto
            {
                Id = t.Id,
                Type = t.Type,
                Value = t.Value,
                DisplayNameKey = t.DisplayNameKey,
                Icon = t.Icon,
                SortOrder = t.SortOrder,
                IsActive = t.IsActive
            })
            .ToListAsync(ct);
    }

    public async Task<RewardTokenDto?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.RewardTokenDefinitions.FindAsync(new object[] { id }, ct);
        return entity == null ? null : MapToDto(entity);
    }

    public async Task<RewardTokenDto> CreateAsync(RewardTokenCreateOrUpdateDto dto, CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var entity = new RewardTokenDefinition
        {
            Id = Guid.NewGuid(),
            Type = dto.Type.Trim(),
            Value = dto.Value.Trim(),
            DisplayNameKey = dto.DisplayNameKey.Trim(),
            Icon = string.IsNullOrWhiteSpace(dto.Icon) ? null : dto.Icon.Trim(),
            SortOrder = dto.SortOrder,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now
        };
        _context.RewardTokenDefinitions.Add(entity);
        await _context.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<RewardTokenDto?> UpdateAsync(Guid id, RewardTokenCreateOrUpdateDto dto, CancellationToken ct = default)
    {
        var entity = await _context.RewardTokenDefinitions.FindAsync(new object[] { id }, ct);
        if (entity == null) return null;

        entity.Type = dto.Type.Trim();
        entity.Value = dto.Value.Trim();
        entity.DisplayNameKey = dto.DisplayNameKey.Trim();
        entity.Icon = string.IsNullOrWhiteSpace(dto.Icon) ? null : dto.Icon.Trim();
        entity.SortOrder = dto.SortOrder;
        entity.UpdatedAt = DateTime.UtcNow;

        await _context.SaveChangesAsync(ct);
        return MapToDto(entity);
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken ct = default)
    {
        var entity = await _context.RewardTokenDefinitions.FindAsync(new object[] { id }, ct);
        if (entity == null) return false;

        entity.IsActive = false;
        entity.UpdatedAt = DateTime.UtcNow;
        await _context.SaveChangesAsync(ct);
        return true;
    }

    private static RewardTokenDto MapToDto(RewardTokenDefinition e) => new()
    {
        Id = e.Id,
        Type = e.Type,
        Value = e.Value,
        DisplayNameKey = e.DisplayNameKey,
        Icon = e.Icon,
        SortOrder = e.SortOrder,
        IsActive = e.IsActive
    };
}
