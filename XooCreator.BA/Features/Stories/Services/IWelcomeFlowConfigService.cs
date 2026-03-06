using XooCreator.BA.Features.Stories.Configuration;
using XooCreator.BA.Features.Stories.DTOs;

namespace XooCreator.BA.Features.Stories.Services;

/// <summary>
/// Provides Welcome Flow config: reads from DB first, falls back to appsettings when table is empty.
/// </summary>
public interface IWelcomeFlowConfigService
{
    /// <summary>
    /// Gets the effective Welcome Flow options (from DB or appsettings fallback).
    /// </summary>
    Task<WelcomeFlowOptions> GetOptionsAsync(CancellationToken ct = default);

    /// <summary>
    /// Gets the config as DTO for API (from DB or appsettings fallback).
    /// </summary>
    Task<WelcomeFlowConfigDto> GetConfigDtoAsync(CancellationToken ct = default);

    /// <summary>
    /// Updates the Welcome Flow config in DB (admin). Returns the updated config.
    /// </summary>
    Task<WelcomeFlowConfigDto> UpdateAsync(WelcomeFlowConfigDto dto, CancellationToken ct = default);
}
