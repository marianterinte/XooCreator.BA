using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Npgsql;
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

// Store startup exception for display
Exception? startupException = null;

var builder = WebApplication.CreateBuilder(args);

// Add Application Insights
builder.Services.AddApplicationInsightsTelemetry();

var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(portEnv))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{portEnv}");
}

static string QuoteIdentifier(string identifier)
    => $"\"{identifier.Replace("\"", "\"\"")}\"";

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

// Configure detailed error page for startup errors
StartupErrorHandler.ConfigureErrorPage(app);

app.UseCors("AllowAll");

app.UseGlobalExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();


// Auto-migrate database on startup + initializare date
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var context = scope.ServiceProvider.GetRequiredService<XooDbContext>();
    // DISABLED: Temporarily disabled robust migration service
    // var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
    var storiesService = scope.ServiceProvider.GetRequiredService<IStoriesService>();
    var treeModelService = scope.ServiceProvider.GetRequiredService<ITreeModelService>();
    var discoverySeeder = scope.ServiceProvider.GetRequiredService<ISeedDiscoveryService>();
    var bestiaryUpdater = scope.ServiceProvider.GetRequiredService<IBestiaryFileUpdater>();
    var heroDefinitionSeeder = scope.ServiceProvider.GetRequiredService<IHeroDefinitionSeedService>();
    var storyTopicsSeeder = scope.ServiceProvider.GetRequiredService<IStoryTopicsSeedService>();

    try
    {
        logger.LogInformation("🚀 Starting database initialization...");
        var recreate = builder.Configuration.GetValue<bool>("Database:RecreateOnStart");
        var dbSchema = builder.Configuration.GetValue<string>("Database:Schema") ?? "public";
        logger.LogInformation("📊 Database configuration - RecreateOnStart: {Recreate}, Schema: {Schema}", recreate, dbSchema);

        if (recreate)
        {
            Console.WriteLine("🔄 Forcing database recreation via schema drop...");
            Console.WriteLine($"📊 Target schema: {dbSchema}");
            
            try
            {
                // Method that works on cloud platforms (Supabase, Railway, etc.)
                // Drop and recreate the schema (removes all tables, data, etc.)
                var schemaName = QuoteIdentifier(dbSchema);
                Console.WriteLine($"🔄 Dropping schema {dbSchema}...");
                await context.Database.ExecuteSqlRawAsync($"DROP SCHEMA IF EXISTS {schemaName} CASCADE;");
                Console.WriteLine("✅ Schema dropped successfully");
                
                Console.WriteLine($"🔄 Creating schema {dbSchema}...");
                await context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA {schemaName};");
                Console.WriteLine("✅ Schema recreated successfully");
                
                // Apply migrations directly using EF Core's built-in method
                Console.WriteLine("🔄 Applying migrations to recreated schema...");
                await context.Database.MigrateAsync();
                Console.WriteLine("✅ Migrations applied successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine("");
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine($"❌ Schema recreation failed: {ex.GetType().Name}");
                Console.WriteLine($"❌ Error: {ex.Message}");
                if (ex is Npgsql.PostgresException pgEx)
                {
                    Console.WriteLine($"❌ PostgreSQL Error Code: {pgEx.SqlState}");
                    Console.WriteLine($"❌ PostgreSQL Error Detail: {pgEx.Detail ?? "N/A"}");
                }
                Console.WriteLine("═══════════════════════════════════════════════════════════");
                Console.WriteLine("");
                
                // Fallback: try normal migration in case schema operations failed
                Console.WriteLine("🔄 Attempting fallback: normal migration path...");
                try
                {
                    await context.Database.MigrateAsync();
                    Console.WriteLine("✅ Fallback migration applied");
                }
                catch (Exception migEx)
                {
                    Console.WriteLine("");
                    Console.WriteLine("═══════════════════════════════════════════════════════════");
                    Console.WriteLine($"❌ Even fallback migration failed: {migEx.GetType().Name}");
                    Console.WriteLine($"❌ Error: {migEx.Message}");
                    if (migEx is Npgsql.PostgresException fallbackPgEx)
                    {
                        Console.WriteLine($"❌ PostgreSQL Error Code: {fallbackPgEx.SqlState}");
                        Console.WriteLine($"❌ PostgreSQL Error Detail: {fallbackPgEx.Detail ?? "N/A"}");
                    }
                    Console.WriteLine("═══════════════════════════════════════════════════════════");
                    Console.WriteLine("");
                    throw;
                }
            }
        }
        else
        {
            // DISABLED: Robust incremental migration path temporarily disabled
            // Using simple migration approach instead
            // Ensure schema exists before running migrations
            var schemaName = QuoteIdentifier(dbSchema);
            Console.WriteLine($"🔄 Ensuring schema '{dbSchema}' exists...");
            await context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA IF NOT EXISTS {schemaName};");
            Console.WriteLine($"✅ Schema '{dbSchema}' ensured");
            
            // Simple migration path - using EF Core's built-in method directly
            Console.WriteLine($"📊 Using schema: {dbSchema}");
            Console.WriteLine("🔄 Applying migrations...");
            await context.Database.MigrateAsync();
            Console.WriteLine("✅ Migrations applied successfully");
        }

        logger.LogInformation("🌱 Starting data seeding...");
        Console.WriteLine("🌱 Starting data seeding...");

        // Seed discovery items (63 combos)
        logger.LogInformation("🌱 Seeding discovery items...");
        await discoverySeeder.EnsureSeedAsync();
        logger.LogInformation("✅ Discovery items seeded");
        Console.WriteLine("✅ Discovery items seeded");

        logger.LogInformation("🌱 Updating bestiary image file names...");
        await bestiaryUpdater.EnsureImageFileNamesAsync();
        logger.LogInformation("✅ Bestiary images updated");
        Console.WriteLine("✅ Bestiary images updated");
        
        // Seed hero definitions
        logger.LogInformation("🌱 Seeding hero definitions...");
        await heroDefinitionSeeder.SeedHeroDefinitionsAsync();
        logger.LogInformation("✅ Hero definitions seeded");
        Console.WriteLine("✅ Hero definitions seeded");

        // Seed story topics and age groups
        logger.LogInformation("🌱 Seeding story topics and age groups...");
        await storyTopicsSeeder.SeedTopicsAndAgeGroupsAsync();
        logger.LogInformation("✅ Story topics and age groups seeded");
        Console.WriteLine("✅ Story topics and age groups seeded");

        // CRITICAL: Stories must be seeded BEFORE TreeModel 
        // because TreeStoryNodes have FK constraints to StoryDefinitions
        logger.LogInformation("🌱 Initializing stories...");
        await storiesService.InitializeStoriesAsync();
        logger.LogInformation("✅ Stories initialized");
        Console.WriteLine("✅ Stories initialized");

        // Initialize marketplace data for all stories (including independent ones)
        logger.LogInformation("🌱 Initializing marketplace data...");
        var marketplaceService = scope.ServiceProvider.GetRequiredService<IStoriesMarketplaceService>();
        await marketplaceService.InitializeMarketplaceAsync();
        logger.LogInformation("✅ Marketplace data initialized");
        Console.WriteLine("✅ Marketplace data initialized");

        // Now seed the tree model (which references stories)
        logger.LogInformation("🌱 Initializing tree model...");
        await treeModelService.InitializeTreeModelAsync();
        logger.LogInformation("✅ Tree model initialized");
        Console.WriteLine("✅ Tree model initialized");

        if (recreate)
        {
            logger.LogInformation("🌱 Updating credit wallets...");
            var wallets = await context.CreditWallets.ToListAsync();
            foreach (var w in wallets)
            {
                if (w.DiscoveryBalance < 5) w.DiscoveryBalance = 5;
            }
            await context.SaveChangesAsync();
            logger.LogInformation("✅ Credit wallets updated");
            Console.WriteLine("✅ Credit wallets updated");
        }

        logger.LogInformation("🎉 Database setup completed successfully!");
        Console.WriteLine("🎉 Database setup completed successfully!");
    }
    catch (Exception ex)
    {
        logger.LogCritical(ex, "❌ CRITICAL: Migration or initialization failed!");
        logger.LogCritical("Exception Type: {ExceptionType}", ex.GetType().FullName);
        logger.LogCritical("Exception Message: {ExceptionMessage}", ex.Message);
        
        if (ex is Npgsql.PostgresException pgEx)
        {
            logger.LogCritical("PostgreSQL Error Code: {SqlState}", pgEx.SqlState);
            logger.LogCritical("PostgreSQL Error Detail: {Detail}", pgEx.Detail ?? "N/A");
            logger.LogCritical("PostgreSQL Error Hint: {Hint}", pgEx.Hint ?? "N/A");
        }
        
        if (ex.StackTrace != null)
        {
            logger.LogCritical("Stack Trace: {StackTrace}", ex.StackTrace);
        }
        
        // Also write to console for visibility
        Console.WriteLine($"❌ Migration or initialization failed: {ex.Message}");
        Console.WriteLine($"Stack trace: {ex.StackTrace}");
        
        // Store exception for display instead of throwing immediately
        startupException = ex;
        
        // Don't throw - let app start so error page can be displayed
        // throw; // Re-throw to prevent app from starting with broken DB
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

// Display startup error if any
if (startupException != null)
{
    app.MapGet("/", async (HttpContext context) =>
    {
        var html = StartupErrorHandler.GenerateErrorPageHtml(startupException);
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(html);
    });
    
    app.MapGet("/swagger", async (HttpContext context) =>
    {
        var html = StartupErrorHandler.GenerateErrorPageHtml(startupException);
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(html);
    });
    
    app.MapGet("/swagger/index.html", async (HttpContext context) =>
    {
        var html = StartupErrorHandler.GenerateErrorPageHtml(startupException);
        context.Response.ContentType = "text/html; charset=utf-8";
        await context.Response.WriteAsync(html);
    });
}

// Map endpoints (Endpoint Discovery)
app.MapDiscoveredEndpoints();

// Sample protected minimal endpoint
app.MapGet("/api/protected/ping", () => Results.Ok(new { ok = true, ts = DateTimeOffset.UtcNow }))
    .WithTags("Auth")
    .RequireAuthorization();

app.Run();
