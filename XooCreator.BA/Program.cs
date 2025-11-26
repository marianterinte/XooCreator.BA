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
    var migrationService = scope.ServiceProvider.GetRequiredService<IDatabaseMigrationService>();
    var storiesService = scope.ServiceProvider.GetRequiredService<IStoriesService>();
    var treeModelService = scope.ServiceProvider.GetRequiredService<ITreeModelService>();
    var discoverySeeder = scope.ServiceProvider.GetRequiredService<ISeedDiscoveryService>();
    var bestiaryUpdater = scope.ServiceProvider.GetRequiredService<IBestiaryFileUpdater>();
    var heroDefinitionSeeder = scope.ServiceProvider.GetRequiredService<IHeroDefinitionSeedService>();
    var storyTopicsSeeder = scope.ServiceProvider.GetRequiredService<IStoryTopicsSeedService>();

    try
    {
        logger.LogInformation("🚀 Starting database initialization...");
        var dbSchema = builder.Configuration.GetValue<string>("Database:Schema") ?? "alchimalia_schema";
        logger.LogInformation("📊 Database schema: {Schema}", dbSchema);

        try
        {
            var dbConnection = context.Database.GetDbConnection();
            var connectionBuilder = new NpgsqlConnectionStringBuilder(dbConnection.ConnectionString);
            var sanitizedInfo = $"{connectionBuilder.Host}:{connectionBuilder.Port}/{connectionBuilder.Database} (User={connectionBuilder.Username}, SSL Mode={connectionBuilder.SslMode})";
            logger.LogInformation("🔍 Target database: {Info}", sanitizedInfo);
            Console.WriteLine($"🔍 Target database: {sanitizedInfo}");
        }
        catch (Exception parseEx)
        {
            logger.LogWarning(parseEx, "⚠️ Could not parse connection string for logging");
            Console.WriteLine($"⚠️ Could not parse connection string for logging: {parseEx.Message}");
        }

        try
        {
            var connectivityOk = await context.Database.CanConnectAsync();
            if (!connectivityOk)
            {
                logger.LogError("❌ Database connectivity test failed");
                Console.WriteLine("❌ Database connectivity test failed");
                throw new InvalidOperationException("Cannot connect to the configured database. Check network/firewall settings and credentials.");
            }

            logger.LogInformation("✅ Database connectivity test succeeded");
            Console.WriteLine("✅ Database connectivity test succeeded");
        }
        catch (Exception connectEx)
        {
            logger.LogCritical(connectEx, "❌ Exception while testing database connectivity");
            Console.WriteLine($"❌ Exception while testing database connectivity: {connectEx.Message}");
            throw;
        }

        var schemaName = QuoteIdentifier(dbSchema);
        Console.WriteLine($"🔄 Ensuring schema '{dbSchema}' exists...");
        await context.Database.ExecuteSqlRawAsync($"CREATE SCHEMA IF NOT EXISTS {schemaName};");
        Console.WriteLine($"✅ Schema '{dbSchema}' ensured");

        Console.WriteLine("🔄 Checking for pending migrations...");
        var pendingMigrations = await migrationService.GetPendingMigrationsAsync();
        if (pendingMigrations.Count > 0)
        {
            Console.WriteLine($"🔄 Found {pendingMigrations.Count} pending migration(s): {string.Join(", ", pendingMigrations)}");
        }
        else
        {
            Console.WriteLine("✅ No pending migrations found");
        }

        Console.WriteLine("🔄 Applying migrations...");
        var migrationApplied = await migrationService.ApplyMigrationsAsync();
        if (!migrationApplied)
        {
            Console.WriteLine("");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("❌ CRITICAL: Failed to apply database migrations!");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("❌ The application cannot start with an inconsistent database state.");
            Console.WriteLine("❌ Please check the detailed error logs ABOVE for the exact failure reason.");
            Console.WriteLine("═══════════════════════════════════════════════════════════");
            Console.WriteLine("");
            throw new InvalidOperationException("Database migration failed. Check the logs above for details.");
        }

        Console.WriteLine("✅ Database migrations completed");

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

if (app.Environment.IsDevelopment())
{
    app.MapGet("/debug/db-state", async (
        IDatabaseMigrationService migrationService,
        XooDbContext context) =>
    {
        var conn = context.Database.GetDbConnection();
        var connBuilder = new NpgsqlConnectionStringBuilder(conn.ConnectionString);

        var info = new
        {
            Connection = new
            {
                Host = connBuilder.Host,
                Port = connBuilder.Port,
                Database = connBuilder.Database,
                User = connBuilder.Username,
                SslMode = connBuilder.SslMode.ToString()
            },
            ConfiguredSchema = context.Model.GetDefaultSchema(),
            ForcedSchema = Environment.GetEnvironmentVariable("DB_FORCE_SCHEMA"),
            CanConnect = await context.Database.CanConnectAsync(),
            AppliedMigrations = await migrationService.GetAppliedMigrationsAsync(),
            PendingMigrations = await migrationService.GetPendingMigrationsAsync()
        };

        return Results.Json(info);
    })
    .WithTags("Debug")
    .WithName("GetDatabaseState");
}

// Sample protected minimal endpoint
app.MapGet("/api/protected/ping", () => Results.Ok(new { ok = true, ts = DateTimeOffset.UtcNow }))
    .WithTags("Auth")
    .RequireAuthorization();

app.Run();
