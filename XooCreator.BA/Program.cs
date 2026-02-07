using Microsoft.EntityFrameworkCore;
using Npgsql;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Errors;
using XooCreator.BA.Infrastructure.DependencyInjection;
using XooCreator.BA.Infrastructure.Configuration;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Middleware;
using XooCreator.BA.Infrastructure;
using XooCreator.DbScriptRunner;
using XooCreator.BA.Infrastructure.Services.Images;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Caching;
using XooCreator.BA.Features.AlchimaliaUniverse.Services;

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

builder.Services.AddLogging(builder =>
{
    builder.AddConsole(options =>
    {
        options.FormatterName = "RedError";
    });
    builder.AddConsoleFormatter<XooCreator.BA.Infrastructure.Logging.RedErrorConsoleFormatter, Microsoft.Extensions.Logging.Console.SimpleConsoleFormatterOptions>(options =>
    {
        options.TimestampFormat = "HH:mm:ss ";
        options.SingleLine = false;
        options.IncludeScopes = false;
    });
});
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddEndpointDefinitions();
builder.Services.AddSwaggerConfiguration();

builder.Services.AddCorsConfiguration();

builder.Services.AddDatabaseConfiguration(builder.Configuration);

// Register all application services using extension methods
builder.Services.AddApplicationServices();
builder.Services.Configure<XooCreator.BA.Infrastructure.Caching.UniverseCachingOptions>(
    builder.Configuration.GetSection(XooCreator.BA.Infrastructure.Caching.UniverseCachingOptions.SectionName));
builder.Services.Configure<ImageCompressionOptions>(builder.Configuration.GetSection(ImageCompressionOptions.SectionName));
builder.Services.Configure<MarketplaceCacheOptions>(builder.Configuration.GetSection(MarketplaceCacheOptions.SectionName));
builder.Services.AddHostedService<StoryPublishQueueWorker>();
builder.Services.AddHostedService<StoryVersionQueueWorker>();
builder.Services.AddHostedService<StoryImportQueueWorker>();
builder.Services.AddHostedService<StoryForkQueueWorker>();
builder.Services.AddHostedService<StoryForkAssetsQueueWorker>();
builder.Services.AddHostedService<StoryExportQueueWorker>();
builder.Services.AddHostedService<StoryDocumentExportQueueWorker>();
builder.Services.AddHostedService<StoryAudioExportQueueWorker>();
builder.Services.AddHostedService<StoryAudioImportQueueWorker>();
builder.Services.AddHostedService<ExportCleanupService>();
builder.Services.AddHostedService<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicPublishQueueJob>();
builder.Services.AddHostedService<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicVersionQueueJob>();
builder.Services.AddHostedService<XooCreator.BA.Features.StoryEditor.StoryEpic.Services.EpicAggregatesQueueJob>();
builder.Services.AddHostedService<HeroDefinitionVersionQueueWorker>();
builder.Services.AddHostedService<AnimalVersionQueueWorker>();
builder.Services.AddHostedService<HeroPublishQueueWorker>();
builder.Services.AddHostedService<AnimalPublishQueueWorker>();

builder.Services.AddAuthConfiguration(builder.Configuration);

// Configure request size limits for file uploads (especially import-full endpoint)
// ImportFullStoryEndpoint allows up to 500MB, so we need to configure both Kestrel and FormOptions
builder.WebHost.ConfigureKestrel(options =>
{
    // Set max request body size to 600MB (500MB + 100MB headroom for multipart overhead)
    options.Limits.MaxRequestBodySize = 600 * 1024 * 1024; // 600MB
});

// Configure FormOptions for multipart/form-data uploads
builder.Services.Configure<Microsoft.AspNetCore.Http.Features.FormOptions>(options =>
{
    // Set multipart body length limit to 600MB (matching Kestrel limit)
    options.MultipartBodyLengthLimit = 600 * 1024 * 1024; // 600MB
    // Increase value length limit for form fields
    options.ValueLengthLimit = int.MaxValue;
    // Increase key length limit
    options.KeyLengthLimit = int.MaxValue;
});

var app = builder.Build();

// Configure detailed error page for startup errors
StartupErrorHandler.ConfigureErrorPage(app);

app.UseCors("AllowAll");

// Request performance tracking (must be before exception handling to measure full request)
app.UseRequestPerformanceTracking();

app.UseGlobalExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();

// Locale routing middleware (must be before MapDiscoveredEndpoints)
app.UseLocaleInApiPath();

// Connectivity check only - schema/scripts handled via XooCreator.DbScriptRunner
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    var context = scope.ServiceProvider.GetRequiredService<XooDbContext>();

    try
    {
        logger.LogInformation("🚀 Starting database connectivity check...");
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

        logger.LogInformation("✅ Database connectivity verified.");
        
        // Check if we should run database scripts on startup (local development only)
        var runScriptsOnStartup = builder.Configuration.GetValue<bool>("Database:RunScriptsOnStartup", false);
        if (runScriptsOnStartup)
        {
            logger.LogInformation("📜 Database:RunScriptsOnStartup is enabled. Running SQL scripts...");
            Console.WriteLine("📜 Running database scripts on startup...");
            
            try
            {
                var dbConnection = context.Database.GetDbConnection();
                var connectionString = dbConnection.ConnectionString;
                
                // Find Database/Scripts folder relative to project root
                // Try multiple paths: from current directory, from base directory, from solution root
                string? scriptsPath = null;
                var searchPaths = new[]
                {
                    // From project root (when running from project directory)
                    Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "Database", "Scripts"),
                    // From bin/Debug or bin/Release (when running compiled app)
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "Database", "Scripts"),
                    // From solution root (BA/XooCreator.BA/Database/Scripts)
                    Path.Combine(Directory.GetCurrentDirectory(), "..", "Database", "Scripts"),
                    // Absolute path from base directory
                    Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "Database", "Scripts")
                };
                
                foreach (var path in searchPaths)
                {
                    var fullPath = Path.GetFullPath(path);
                    if (Directory.Exists(fullPath))
                    {
                        // Verify it's the correct folder by checking for V*.sql files
                        var hasScripts = Directory.EnumerateFiles(fullPath, "V*.sql", SearchOption.TopDirectoryOnly).Any();
                        if (hasScripts)
                        {
                            scriptsPath = fullPath;
                            break;
                        }
                    }
                }
                
                if (scriptsPath == null || !Directory.Exists(scriptsPath))
                {
                    logger.LogWarning("⚠️ Database scripts folder not found. Searched paths: {Paths}. Skipping script execution.", 
                        string.Join(", ", searchPaths.Select(p => Path.GetFullPath(p))));
                    Console.WriteLine("⚠️ Database scripts folder not found. Skipping script execution.");
                }
                else
                {
                    logger.LogInformation("📁 Scripts path: {ScriptsPath}", scriptsPath);
                    Console.WriteLine($"📁 Scripts path: {scriptsPath}");
                    
                    var options = new RunnerOptions
                    {
                        ConnectionString = connectionString,
                        Schema = dbSchema,
                        ScriptsPath = scriptsPath,
                        DryRun = false
                    };
                    options.ApplyDefaults();
                    
                    var scriptLogger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>()
                        .CreateLogger<ScriptRunnerService>();
                    var scriptRunner = new ScriptRunnerService(options, scriptLogger);
                    
                    await scriptRunner.RunAsync(CancellationToken.None);
                    
                    logger.LogInformation("✅ Database scripts executed successfully.");
                    Console.WriteLine("✅ Database scripts executed successfully.");
                }
            }
            catch (Exception scriptEx)
            {
                logger.LogError(scriptEx, "❌ Failed to execute database scripts on startup");
                Console.WriteLine($"❌ Failed to execute database scripts: {scriptEx.Message}");
                // Don't throw - allow app to continue even if scripts fail
                // This allows the app to start even if scripts have issues
            }
        }
        else
        {
            logger.LogInformation("📜 Database:RunScriptsOnStartup is disabled. Database objects must be managed via XooCreator.DbScriptRunner scripts (V0001+).");
            Console.WriteLine("📜 Database scripts will not run automatically. Use XooCreator.DbScriptRunner to apply scripts manually.");
        }
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

if (app.Environment.IsDevelopment() || app.Environment.EnvironmentName == "Local")
{
    app.MapGet("/debug/db-state", async (XooDbContext context) =>
    {
        var dbConnection = context.Database.GetDbConnection();
        var connBuilder = new NpgsqlConnectionStringBuilder(dbConnection.ConnectionString);
        var schema = context.Model.GetDefaultSchema() ?? "alchimalia_schema";
        var schemaIdentifier = schema.Replace("\"", "\"\"");

        var versions = new List<object>();
        string? versionsError = null;

        try
        {
            await context.Database.OpenConnectionAsync();
            await using var command = dbConnection.CreateCommand();
            command.CommandText = $"""
                                   SELECT script_name, checksum, executed_at, execution_time_ms, status
                                   FROM "{schemaIdentifier}"."schema_versions"
                                   ORDER BY executed_at;
                                   """;

            await using var reader = await command.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var executedAt = reader.GetFieldValue<DateTime>(2);
                var executedAtUtc = DateTime.SpecifyKind(executedAt, DateTimeKind.Utc);

                versions.Add(new
                {
                    Script = reader.GetString(0),
                    Checksum = reader.GetString(1),
                    ExecutedAtUtc = executedAtUtc,
                    ExecutionTimeMs = reader.IsDBNull(3) ? (long?)null : reader.GetInt64(3),
                    Status = reader.GetString(4)
                });
            }
        }
        catch (PostgresException ex) when (ex.SqlState == "42P01")
        {
            versionsError = $"Table {schema}.schema_versions not found. Run V0001__initial_full_schema.sql via XooCreator.DbScriptRunner.";
        }
        catch (Exception ex)
        {
            versionsError = $"Failed to read schema_versions: {ex.Message}";
        }
        finally
        {
            await context.Database.CloseConnectionAsync();
        }

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
            ConfiguredSchema = schema,
            ForcedSchema = Environment.GetEnvironmentVariable("DB_FORCE_SCHEMA"),
            CanConnect = await context.Database.CanConnectAsync(),
            SchemaVersions = versions,
            SchemaVersionsError = versionsError,
            UtcTimestamp = DateTimeOffset.UtcNow
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
