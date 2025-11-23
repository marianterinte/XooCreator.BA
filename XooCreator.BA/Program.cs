using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using XooCreator.BA.Infrastructure.Swagger;
using Npgsql;
using XooCreator.BA.Data;
using XooCreator.BA.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Errors;
using XooCreator.BA.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.TreeOfLight.Services;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Data.Services;
using XooCreator.BA.Data.Interceptors;

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
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "XooCreator.BA",
        Version = "v1.0.0",
        Description = "XooCreator Backend API"
    });
    c.OperationFilter<LocaleParameterOperationFilter>();
    c.OperationFilter<BusinessFolderTagOperationFilter>();

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
        policy
            .SetIsOriginAllowed(_ => true) // acceptă orice Origin; va întoarce Originul cererii (nu "*")
            .AllowAnyMethod()
            .AllowAnyHeader()
            .AllowCredentials()
            .SetPreflightMaxAge(TimeSpan.FromHours(24))
    );

});

builder.Services.AddDbContext<XooDbContext>(options =>
{
    var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");
    string cs;
    if (!string.IsNullOrWhiteSpace(dbUrl))
    {
        var uri = new Uri(dbUrl);
        var userInfo = uri.UserInfo.Split(':');
        var npg = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Username = userInfo.ElementAtOrDefault(0) ?? "postgres",
            Password = userInfo.ElementAtOrDefault(1) ?? string.Empty,
            Database = uri.AbsolutePath.Trim('/'),
            SslMode = SslMode.Require
        };
        cs = npg.ConnectionString;
    }
    else
    {
        cs = builder.Configuration.GetConnectionString("Postgres")
            ?? "Host=localhost;Port=5432;Database=xoo_db;Username=postgres;Password=admin";
    }

    options.UseNpgsql(cs);
    
    // Add interceptor to automatically make migration SQL commands idempotent
    // This transforms CREATE TABLE, CREATE INDEX, ALTER TABLE ADD CONSTRAINT, etc.
    // to use IF NOT EXISTS, making all migrations safe to run multiple times
    var loggerFactory = builder.Services.BuildServiceProvider().GetService<ILoggerFactory>();
    var logger = loggerFactory?.CreateLogger<IdempotentMigrationCommandInterceptor>();
    options.AddInterceptors(new IdempotentMigrationCommandInterceptor(logger));
});

// Register all application services using extension methods
builder.Services.AddApplicationServices();

var auth0Section = builder.Configuration.GetSection("Auth0");
var auth0Domain = auth0Section["Domain"];
var auth0Audience = auth0Section["Audience"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.Authority = $"https://{auth0Domain}";
    options.Audience = auth0Audience;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidIssuer = $"https://{auth0Domain}/",
        ValidateAudience = true,
        ValidAudience = auth0Audience,
        ValidateLifetime = true
    };

    options.Events = new JwtBearerEvents
    {
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"[JWT] Authentication failed: {context.Exception.Message}");
            return Task.CompletedTask;
        },
        OnChallenge = context =>
        {
            if (context.AuthenticateFailure != null)
            {
                Console.WriteLine($"[JWT] Challenge failed: {context.AuthenticateFailure.Message}");
            }
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            var iss = context.Principal?.FindFirst("iss")?.Value ?? "<none>";
            var aud = string.Join(",", context.Principal?.FindAll("aud").Select(c => c.Value) ?? Array.Empty<string>());
            var sub = context.Principal?.FindFirst("sub")?.Value ?? "<none>";
            Console.WriteLine($"[JWT] Token validated. iss={iss} aud={aud} sub={sub}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddAuthorization();

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
        recreate = false; // Force recreation every time during development

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
