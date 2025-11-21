using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data.SeedData;

/// <summary>
/// Helper class for managing the seed user account used for seeded stories.
/// Ensures a consistent user ID is used for all seed data ownership.
/// </summary>
public static class SeedUserHelper
{
    public const string SeedUserEmail = "seed@alchimalia.com";
    public const string SeedUserAuth0Id = "seed|seed@alchimalia.com"; // Special Auth0Id format for seed user
    public const string SeedUserName = "Alchimalia";
    
    /// <summary>
    /// Gets or creates the seed user in the database.
    /// Returns the user's GUID, or null if creation fails.
    /// </summary>
    public static async Task<Guid?> GetOrCreateSeedUserIdAsync(XooDbContext db, CancellationToken ct = default)
    {
        // Try to find existing seed user by email
        var seedUser = await db.AlchimaliaUsers
            .FirstOrDefaultAsync(u => u.Email == SeedUserEmail, ct);
        
        if (seedUser != null)
        {
            return seedUser.Id;
        }
        
        // Try to find by Auth0Id as fallback
        seedUser = await db.AlchimaliaUsers
            .FirstOrDefaultAsync(u => u.Auth0Id == SeedUserAuth0Id, ct);
        
        if (seedUser != null)
        {
            // Update email if it was different
            if (seedUser.Email != SeedUserEmail)
            {
                seedUser.Email = SeedUserEmail;
                await db.SaveChangesAsync(ct);
            }
            return seedUser.Id;
        }
        
        // Create new seed user
        try
        {
            seedUser = new AlchimaliaUser
            {
                Id = Guid.NewGuid(),
                Auth0Id = SeedUserAuth0Id,
                Email = SeedUserEmail,
                Name = SeedUserName,
                FirstName = "Alchimalia",
                LastName = "",
                Role = UserRole.Admin, // Seed user should have Admin role for system stories
                Roles = new List<UserRole> { UserRole.Admin, UserRole.Creator }, // Multiple roles
                CreatedAt = DateTime.UtcNow,
                LastLoginAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };
            
            db.AlchimaliaUsers.Add(seedUser);
            
            // Create credit wallet for seed user
            db.CreditWallets.Add(new CreditWallet
            {
                UserId = seedUser.Id,
                Balance = 0,
                UpdatedAt = DateTime.UtcNow
            });
            
            await db.SaveChangesAsync(ct);
            return seedUser.Id;
        }
        catch (Exception ex)
        {
            // Log error but don't throw - return null to allow fallback
            Console.WriteLine($"[SEED USER] Failed to create seed user: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Gets the seed user ID synchronously (for use in static mappers).
    /// This should only be used when a DbContext is available in the calling context.
    /// </summary>
    public static Guid? GetSeedUserId(XooDbContext db)
    {
        try
        {
            var seedUser = db.AlchimaliaUsers
                .AsNoTracking()
                .FirstOrDefault(u => u.Email == SeedUserEmail || u.Auth0Id == SeedUserAuth0Id);
            
            return seedUser?.Id;
        }
        catch
        {
            return null;
        }
    }
}

