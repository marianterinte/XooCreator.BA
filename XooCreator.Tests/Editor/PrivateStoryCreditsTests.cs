using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Features.User.Repositories;
using XooCreator.BA.Features.User.Services;
using Xunit;

namespace XooCreator.Tests.Editor;

/// <summary>
/// Tests for full story generation credits: spend and insufficient balance.
/// </summary>
public class PrivateStoryCreditsTests
{
    private static (XooDbContext Db, UserProfileService Service) CreateContextAndService()
    {
        var options = new DbContextOptionsBuilder<XooDbContext>()
            .UseInMemoryDatabase(databaseName: "PrivateStoryCredits_" + Guid.NewGuid().ToString("N"))
            .Options;
        var config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?> { ["Database:Schema"] = "public" })
            .Build();
        var db = new XooDbContext(options, config);
        var userRepo = new Mock<IUserRepository>().Object;
        var service = new UserProfileService(db, userRepo);
        return (db, service);
    }

    [Fact]
    public async Task SpendFullStoryCreditsAsync_WhenBalanceSufficient_ReturnsSuccessAndDecrementsBalance()
    {
        var (db, service) = CreateContextAndService();
        var userId = Guid.NewGuid();
        db.CreditWallets.Add(new CreditWallet
        {
            UserId = userId,
            Balance = 0,
            DiscoveryBalance = 0,
            GenerativeBalance = 0,
            FullStoryGenerationBalance = 2,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var result = await service.SpendFullStoryCreditsAsync(userId, new SpendCreditsRequest { Amount = 1, Reference = "full-story-generation" });

        Assert.True(result.Success);
        Assert.Equal(1, result.NewBalance);
        var wallet = await db.CreditWallets.FirstAsync(w => w.UserId == userId);
        Assert.Equal(1, wallet.FullStoryGenerationBalance);
        var tx = await db.CreditTransactions.FirstOrDefaultAsync(t => t.UserId == userId && t.Reference == "full-story-generation");
        Assert.NotNull(tx);
        Assert.Equal(-1, tx.Amount);
    }

    [Fact]
    public async Task SpendFullStoryCreditsAsync_WhenBalanceInsufficient_ReturnsFailure()
    {
        var (db, service) = CreateContextAndService();
        var userId = Guid.NewGuid();
        db.CreditWallets.Add(new CreditWallet
        {
            UserId = userId,
            Balance = 0,
            DiscoveryBalance = 0,
            GenerativeBalance = 0,
            FullStoryGenerationBalance = 0,
            UpdatedAt = DateTime.UtcNow
        });
        await db.SaveChangesAsync();

        var result = await service.SpendFullStoryCreditsAsync(userId, new SpendCreditsRequest { Amount = 1, Reference = "full-story-generation" });

        Assert.False(result.Success);
        Assert.Contains("Insufficient", result.ErrorMessage ?? "");
        Assert.Equal(0, result.NewBalance);
        var wallet = await db.CreditWallets.FirstAsync(w => w.UserId == userId);
        Assert.Equal(0, wallet.FullStoryGenerationBalance);
    }

    [Fact]
    public async Task SpendFullStoryCreditsAsync_WhenWalletMissing_ReturnsFailure()
    {
        var (_, service) = CreateContextAndService();
        var userId = Guid.NewGuid();

        var result = await service.SpendFullStoryCreditsAsync(userId, new SpendCreditsRequest { Amount = 1, Reference = "full-story-generation" });

        Assert.False(result.Success);
        Assert.NotNull(result.ErrorMessage);
        Assert.Equal(0, result.NewBalance);
    }
}
