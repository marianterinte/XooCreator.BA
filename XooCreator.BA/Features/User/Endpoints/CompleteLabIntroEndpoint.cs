
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.User.Endpoints;

[Endpoint]
public class CompleteLabIntroEndpoint
{
    [Route("/api/{locale}/user/complete-lab-intro")] // POST
    [Authorize]
    public static async Task<IResult> HandlePost(
        [FromRoute] string locale,
        [FromServices] IUserContextService userContext,
        [FromServices] XooDbContext dbContext,
        CancellationToken ct)
    {
        var userId = await userContext.GetUserIdAsync();
        if (userId == null)
        {
            return Results.Unauthorized();
        }

        var user = await dbContext.AlchimaliaUsers.FindAsync(userId.Value);
        if (user == null)
        {
            return Results.NotFound("User not found.");
        }

        if (!user.HasVisitedImaginationLaboratory)
        {
            user.HasVisitedImaginationLaboratory = true;
            
            var wallet = await dbContext.CreditWallets.FirstOrDefaultAsync(w => w.UserId == userId.Value, ct);
            if (wallet == null)
            {
                wallet = new CreditWallet
                {
                    UserId = userId.Value,
                    Balance = 0,
                    DiscoveryBalance = 5,
                    UpdatedAt = DateTime.UtcNow
                };
                dbContext.CreditWallets.Add(wallet);
            }
            else
            {
                wallet.DiscoveryBalance += 5;
                wallet.UpdatedAt = DateTime.UtcNow;
            }
            
            var transaction = new CreditTransaction
            {
                Id = Guid.NewGuid(),
                UserId = userId.Value,
                Amount = 5,
                Type = CreditTransactionType.Grant,
                Reference = "Lab Intro Completion Reward",
                CreatedAt = DateTime.UtcNow
            };
            dbContext.CreditTransactions.Add(transaction);
            
            await dbContext.SaveChangesAsync(ct);
        }

        return Results.Ok();
    }
}
