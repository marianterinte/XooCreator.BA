using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.Stories.Configuration;
using XooCreator.BA.Features.Stories.DTOs;
using XooCreator.BA.Features.Stories.Services.Caching;

namespace XooCreator.BA.Features.Stories.Services;

/// <summary>
/// Reads Welcome Flow config from DB first; falls back to appsettings when table has no row or row is empty.
/// Uses IWelcomeFlowCacheService for caching (24h) and invalidation; no direct cache access here.
/// </summary>
public class WelcomeFlowConfigService : IWelcomeFlowConfigService
{
    private readonly XooDbContext _context;
    private readonly WelcomeFlowOptions _fallbackOptions;
    private readonly IWelcomeFlowCacheService _welcomeFlowCache;

    public WelcomeFlowConfigService(
        XooDbContext context,
        IOptions<WelcomeFlowOptions> fallbackOptions,
        IWelcomeFlowCacheService welcomeFlowCache)
    {
        _context = context;
        _fallbackOptions = fallbackOptions.Value;
        _welcomeFlowCache = welcomeFlowCache;
    }

    public async Task<WelcomeFlowOptions> GetOptionsAsync(CancellationToken ct = default)
    {
        return await _welcomeFlowCache.GetOrSetOptionsAsync(LoadOptionsFromDbAsync, ct);
    }

    public async Task<WelcomeFlowConfigDto> UpdateAsync(WelcomeFlowConfigDto dto, CancellationToken ct = default)
    {
        var row = await _context.WelcomeFlowConfigs.FirstOrDefaultAsync(x => x.Id == 1, ct);
        if (row == null)
        {
            row = new WelcomeFlowConfig { Id = 1 };
            _context.WelcomeFlowConfigs.Add(row);
        }

        row.EntryPointStoryId = (dto.EntryPointStoryId ?? string.Empty).Trim();
        row.KindergartenGirl = (dto.Kindergarten?.Girl ?? string.Empty).Trim();
        row.KindergartenBoy = (dto.Kindergarten?.Boy ?? string.Empty).Trim();
        row.PrimaryGirl = (dto.Primary?.Girl ?? string.Empty).Trim();
        row.PrimaryBoy = (dto.Primary?.Boy ?? string.Empty).Trim();
        row.OlderGirl = (dto.Older?.Girl ?? string.Empty).Trim();
        row.OlderBoy = (dto.Older?.Boy ?? string.Empty).Trim();

        await _context.SaveChangesAsync(ct);

        var options = MapRowToOptions(row, _fallbackOptions);
        _welcomeFlowCache.InvalidateAfterUpdate(options.GetAllowedStoryIds());

        return ToDto(row);
    }

    public async Task<WelcomeFlowConfigDto> GetConfigDtoAsync(CancellationToken ct = default)
    {
        var options = await GetOptionsAsync(ct);
        return new WelcomeFlowConfigDto
        {
            EntryPointStoryId = options.EntryPointStoryId,
            Kindergarten = new WelcomeFlowAgeGroupDto { Girl = options.Kindergarten.Girl, Boy = options.Kindergarten.Boy },
            Primary = new WelcomeFlowAgeGroupDto { Girl = options.Primary.Girl, Boy = options.Primary.Boy },
            Older = new WelcomeFlowAgeGroupDto { Girl = options.Older.Girl, Boy = options.Older.Boy }
        };
    }

    private async Task<WelcomeFlowOptions> LoadOptionsFromDbAsync(CancellationToken ct)
    {
        var row = await _context.WelcomeFlowConfigs
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == 1, ct);

        return MapRowToOptions(row, _fallbackOptions);
    }

    private static WelcomeFlowOptions MapRowToOptions(WelcomeFlowConfig? row, WelcomeFlowOptions fallback)
    {
        if (row == null || string.IsNullOrWhiteSpace(row.EntryPointStoryId))
            return fallback;

        return new WelcomeFlowOptions
        {
            EntryPointStoryId = row.EntryPointStoryId.Trim(),
            Kindergarten = new WelcomeFlowAgeGroupOptions
            {
                Girl = (row.KindergartenGirl ?? string.Empty).Trim(),
                Boy = (row.KindergartenBoy ?? string.Empty).Trim()
            },
            Primary = new WelcomeFlowAgeGroupOptions
            {
                Girl = (row.PrimaryGirl ?? string.Empty).Trim(),
                Boy = (row.PrimaryBoy ?? string.Empty).Trim()
            },
            Older = new WelcomeFlowAgeGroupOptions
            {
                Girl = (row.OlderGirl ?? string.Empty).Trim(),
                Boy = (row.OlderBoy ?? string.Empty).Trim()
            }
        };
    }

    private static WelcomeFlowConfigDto ToDto(WelcomeFlowConfig row)
    {
        return new WelcomeFlowConfigDto
        {
            EntryPointStoryId = row.EntryPointStoryId ?? string.Empty,
            Kindergarten = new WelcomeFlowAgeGroupDto
            {
                Girl = row.KindergartenGirl ?? string.Empty,
                Boy = row.KindergartenBoy ?? string.Empty
            },
            Primary = new WelcomeFlowAgeGroupDto
            {
                Girl = row.PrimaryGirl ?? string.Empty,
                Boy = row.PrimaryBoy ?? string.Empty
            },
            Older = new WelcomeFlowAgeGroupDto
            {
                Girl = row.OlderGirl ?? string.Empty,
                Boy = row.OlderBoy ?? string.Empty
            }
        };
    }
}
