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

app.UseGlobalExceptionHandling();

app.UseAuthentication();
app.UseAuthorization();


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

        logger.LogInformation("✅ Database connectivity verified. Database objects must be managed via XooCreator.DbScriptRunner scripts (V0001+).");
        Console.WriteLine("🎉 Connectivity OK. Apply SQL scripts via XooCreator.DbScriptRunner before running the app.");
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
