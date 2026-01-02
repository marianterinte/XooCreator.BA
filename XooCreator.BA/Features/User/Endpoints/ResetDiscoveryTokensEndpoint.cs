using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using XooCreator.BA.Features.TreeOfLight.DTOs;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class ResetDiscoveryTokensEndpoint
{
    private readonly XooDbContext _context;
    private readonly ITreeOfHeroesRepository _treeOfHeroesRepository;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<ResetDiscoveryTokensEndpoint> _logger;

    public ResetDiscoveryTokensEndpoint(
        XooDbContext context,
        ITreeOfHeroesRepository treeOfHeroesRepository,
        IAuth0UserService auth0,
        ILogger<ResetDiscoveryTokensEndpoint> logger)
    {
        _context = context;
        _treeOfHeroesRepository = treeOfHeroesRepository;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/tokens/discovery/reset")]
    [Authorize]
    public static async Task<Results<Ok<ResetDiscoveryTokensResponse>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] ResetDiscoveryTokensEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        using var transaction = await ep._context.Database.BeginTransactionAsync(ct);
        
        try
        {
            // Calculate consumed tokens from UserBestiary (each discovery entry = 1 token consumed)
            // This is the source of truth for discovery tokens consumed in Imagination Laboratory
            var discoveryBestiary = await ep._context.UserBestiary
                .Where(ub => ub.UserId == user.Id && ub.BestiaryType == "discovery")
                .ToListAsync(ct);

            int consumedTokens = discoveryBestiary.Count;

            // Get wallet to return tokens
            var wallet = await ep._context.CreditWallets
                .FirstOrDefaultAsync(w => w.UserId == user.Id, ct);

            if (wallet == null)
            {
                wallet = new CreditWallet
                {
                    UserId = user.Id,
                    Balance = 0,
                    DiscoveryBalance = 0,
                    UpdatedAt = DateTime.UtcNow
                };
                ep._context.CreditWallets.Add(wallet);
            }

            // Return consumed tokens back to DiscoveryBalance
            // This restores the tokens that were spent in the Imagination Laboratory
            wallet.DiscoveryBalance += consumedTokens;
            wallet.UpdatedAt = DateTime.UtcNow;

            // Delete UserBestiary entries with BestiaryType = "discovery"
            // This resets the progress in the Discovery tab (Hero Book)
            if (discoveryBestiary.Count > 0)
            {
                ep._context.UserBestiary.RemoveRange(discoveryBestiary);
            }

            // Reset HasVisitedImaginationLaboratory flag (optional - can keep true)
            // For now, we'll keep it true as it's just a visit flag

            await ep._context.SaveChangesAsync(ct);
            await transaction.CommitAsync(ct);

            var tokensReturned = consumedTokens;

            ep._logger.LogInformation("ResetDiscoveryTokens: Successfully reset tokens userId={UserId}, tokensReturned={TokensReturned}", 
                user.Id, tokensReturned);

            var response = new ResetDiscoveryTokensResponse
            {
                Success = true,
                TokensReturned = tokensReturned
            };

            return TypedResults.Ok(response);
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync(ct);
            ep._logger.LogError(ex, "ResetDiscoveryTokens: Failed to reset tokens userId={UserId}", user.Id);
            
            var response = new ResetDiscoveryTokensResponse
            {
                Success = false,
                ErrorMessage = ex.Message,
                TokensReturned = 0
            };

            return TypedResults.Ok(response);
        }
    }
}

public record ResetDiscoveryTokensResponse
{
    public bool Success { get; init; }
    public string? ErrorMessage { get; init; }
    public int TokensReturned { get; init; }
}

