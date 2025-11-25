using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Errors;
using XooCreator.BA.Infrastructure.DependencyInjection;
using XooCreator.BA.Infrastructure.Configuration;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.TreeOfLight.Services;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Data.Services;
using XooCreator.BA.Services;

var builder = WebApplication.CreateBuilder(args);

var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(portEnv))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{portEnv}");
}

builder.Services.AddLogging();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEndpointDefinitions();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddCorsConfiguration();

builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Register all application services using extension methods
builder.Services.AddApplicationServices();

builder.Services.AddAuthConfiguration(builder.Configuration);

var app = builder.Build();
app.UseCors("AllowAll");

app.UseGlobalExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();


// Auto-migrate database on startup + initializare date
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<XooDbContext>();
    var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
    var storiesService = scope.ServiceProvider.GetRequiredService<IStoriesService>();
    var treeModelService = scope.ServiceProvider.GetRequiredService<ITreeModelService>();
    var discoverySeeder = scope.ServiceProvider.GetRequiredService<ISeedDiscoveryService>();
    var bestiaryUpdater = scope.ServiceProvider.GetRequiredService<IBestiaryFileUpdater>();
    var heroDefinitionSeeder = scope.ServiceProvider.GetRequiredService<IHeroDefinitionSeedService>();
    var storyTopicsSeeder = scope.ServiceProvider.GetRequiredService<IStoryTopicsSeedService>();

    try
    {
        var recreate = builder.Configuration.GetValue<bool>("Database:RecreateOnStart");
        recreate = true; // Force recreation every time during development

        if (recreate)
        {
            Console.WriteLine("🔄 Forcing database recreation via schema drop...");
            
            try
            {
                // Method that works on cloud platforms (Supabase, Railway, etc.)
                // Drop and recreate the public schema (removes all tables, data, etc.)
                await context.Database.ExecuteSqlRawAsync("DROP SCHEMA public CASCADE;");
                Console.WriteLine("✅ Schema dropped successfully");
                
                await context.Database.ExecuteSqlRawAsync("CREATE SCHEMA public;");
                Console.WriteLine("✅ Schema recreated successfully");
                
                // Apply migrations using the robust migration service
                var migrationSuccess = await migrationService.ApplyMigrationsAsync();
                if (!migrationSuccess)
                {
                    throw new InvalidOperationException("Failed to apply migrations after schema recreation");
                }
                Console.WriteLine("✅ Migrations applied successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Schema recreation failed: {ex.Message}");
                
                // Fallback: try normal migration in case schema operations failed
                try
                {
                    var migrationSuccess = await migrationService.ApplyMigrationsAsync();
                    if (!migrationSuccess)
                    {
                        throw new InvalidOperationException("Failed to apply migrations in fallback");
                    }
                    Console.WriteLine("✅ Fallback migration applied");
                }
                catch (Exception migEx)
                {
                    Console.WriteLine($"❌ Even fallback migration failed: {migEx.Message}");
                    throw;
                }
            }
        }
        else
        {
            // Robust incremental migration path for production
            // Uses idempotent operations - can be safely run multiple times
            Console.WriteLine("🔄 Checking for pending migrations...");
            
            var pendingMigrations = await migrationService.GetPendingMigrationsAsync();
            if (pendingMigrations.Count > 0)
            {
                Console.WriteLine($"🔄 Found {pendingMigrations.Count} pending migration(s): {string.Join(", ", pendingMigrations)}");
            }
            
            var migrationSuccess = await migrationService.ApplyMigrationsAsync();
            if (!migrationSuccess)
            {
                // Log detailed error information instead of throwing
                Console.WriteLine("❌ CRITICAL: Failed to apply database migrations!");
                Console.WriteLine("❌ The application cannot start with an inconsistent database state.");
                Console.WriteLine("❌ Please check the logs above for detailed error information.");
                Console.WriteLine("❌ Common causes:");
                Console.WriteLine("   - Migration contains non-idempotent operations (CREATE TABLE instead of CREATE TABLE IF NOT EXISTS)");
                Console.WriteLine("   - Database schema conflicts with migration expectations");
                Console.WriteLine("   - Missing dependencies or permissions");
                Console.WriteLine("❌ Action required: Fix the migration issue and restart the application.");
                
                // Get applied migrations for diagnostic information
                try
                {
                    var appliedMigrations = await migrationService.GetAppliedMigrationsAsync();
                    Console.WriteLine($"ℹ️  Successfully applied migrations: {string.Join(", ", appliedMigrations)}");
                }
                catch (Exception diagEx)
                {
                    Console.WriteLine($"⚠️  Could not retrieve applied migrations: {diagEx.Message}");
                }
                
                // Only throw if we absolutely cannot continue
                // This gives clear information about what went wrong
                throw new InvalidOperationException(
                    "Database migration failed. Check the logs above for details. " +
                    "The application cannot start with an inconsistent database state. " +
                    "Please fix the migration issue and restart.");
            }
            
            Console.WriteLine("✅ Database migrations completed");
        }

        Console.WriteLine("🌱 Starting data seeding...");

        // Seed discovery items (63 combos)
        await discoverySeeder.EnsureSeedAsync();
        Console.WriteLine("✅ Discovery items seeded");

        await bestiaryUpdater.EnsureImageFileNamesAsync();
        Console.WriteLine("✅ Bestiary images updated");
        
        // Seed hero definitions
        await heroDefinitionSeeder.SeedHeroDefinitionsAsync();
        Console.WriteLine("✅ Hero definitions seeded");

        // Seed story topics and age groups
        await storyTopicsSeeder.SeedTopicsAndAgeGroupsAsync();
        Console.WriteLine("✅ Story topics and age groups seeded");

        // CRITICAL: Stories must be seeded BEFORE TreeModel 
        // because TreeStoryNodes have FK constraints to StoryDefinitions
        await storiesService.InitializeStoriesAsync();
        Console.WriteLine("✅ Stories initialized");

        // Initialize marketplace data for all stories (including independent ones)
        var marketplaceService = scope.ServiceProvider.GetRequiredService<IStoriesMarketplaceService>();
        await marketplaceService.InitializeMarketplaceAsync();
        Console.WriteLine("✅ Marketplace data initialized");

        // Now seed the tree model (which references stories)
        await treeModelService.InitializeTreeModelAsync();
        Console.WriteLine("✅ Tree model initialized");

        if (recreate)
        {
            var wallets = await context.CreditWallets.ToListAsync();
            foreach (var w in wallets)
            {
                if (w.DiscoveryBalance < 5) w.DiscoveryBalance = 5;
            }
            await context.SaveChangesAsync();
            Console.WriteLine("✅ Credit wallets updated");
        }

        Console.WriteLine("🎉 Database setup completed successfully!");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"❌ Migration or initialization failed: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        throw; // Re-throw to prevent app from starting with broken DB
    }
}

// Swagger
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "XooCreator.BA v1.0.0");
    c.RoutePrefix = "swagger";
});

// === Aplicăm CORS global, fără dependență de environment ===

// Map endpoints (Endpoint Discovery)
app.MapDiscoveredEndpoints();

// Sample protected minimal endpoint
app.MapGet("/api/protected/ping", () => Results.Ok(new { ok = true, ts = DateTimeOffset.UtcNow }))
    .WithTags("Auth")
    .RequireAuthorization();

app.Run();
