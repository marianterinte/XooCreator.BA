using XooCreator.BA.Features.User.DTOs;

namespace XooCreator.BA.Features.User.Services;

public interface ICreatorTokenService
{
    Task<GetCreatorTokensResponse> GetCreatorTokensAsync(Guid userId, CancellationToken ct);
    Task<CreatorTokenBalanceDto> OverrideTokenAsync(Guid userId, OverrideCreatorTokenRequest request, Guid adminUserId, CancellationToken ct);
    Task<bool> AwardTokenAsync(Guid userId, string tokenType, string tokenValue, int quantity, CancellationToken ct);
}
