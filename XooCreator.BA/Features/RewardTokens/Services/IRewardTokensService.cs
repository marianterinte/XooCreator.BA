using XooCreator.BA.Features.RewardTokens.DTOs;

namespace XooCreator.BA.Features.RewardTokens.Services;

public interface IRewardTokensService
{
    Task<List<RewardTokenDto>> GetActiveTokensAsync(CancellationToken ct = default);
    Task<List<RewardTokenDto>> GetAllTokensAsync(CancellationToken ct = default);
    Task<RewardTokenDto?> GetByIdAsync(Guid id, CancellationToken ct = default);
    Task<RewardTokenDto> CreateAsync(RewardTokenCreateOrUpdateDto dto, CancellationToken ct = default);
    Task<RewardTokenDto?> UpdateAsync(Guid id, RewardTokenCreateOrUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(Guid id, CancellationToken ct = default);
}
