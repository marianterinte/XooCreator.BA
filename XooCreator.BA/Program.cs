using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using XooCreator.BA.Infrastructure.Swagger;
using Npgsql;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Repositories;
using XooCreator.BA.Services;
using XooCreator.BA.Features.TreeOfLight;
using XooCreator.BA.Features.Stories;
using XooCreator.BA.Features.Payment;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Errors;
using XooCreator.BA.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Features.Stories.Services;
using XooCreator.BA.Features.Payment.Services;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Repositories;
using XooCreator.BA.Features.TalesOfAlchimalia.Market.Services;
using XooCreator.BA.Features.TreeOfHeroes.Services;
using XooCreator.BA.Features.TreeOfHeroes.Repositories;
using XooCreator.BA.Features.TreeOfLight.Services;
using XooCreator.BA.Features.TreeOfLight.Repositories;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Features.Stories.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Features.StoryEditor.Repositories;

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
});

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<XooCreator.BA.Features.UserAdministration.Repositories.IUserAdministrationRepository, XooCreator.BA.Features.UserAdministration.Repositories.UserAdministrationRepository>();
builder.Services.AddScoped<XooCreator.BA.Features.UserAdministration.Services.IUserAdministrationService, XooCreator.BA.Features.UserAdministration.Services.UserAdministrationService>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IAuth0UserService, Auth0UserService>();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddSingleton<IBlobSasService, BlobSasService>();
builder.Services.AddScoped<IDbHealthService, DbHealthService>();
builder.Services.AddScoped<ICreatureBuilderService, CreatureBuilderService>();
builder.Services.AddScoped<ISeedDiscoveryService, SeedDiscoveryService>();
builder.Services.AddScoped<IBestiaryFileUpdater, BestiaryFileUpdater>();
builder.Services.AddScoped<IHeroDefinitionSeedService, HeroDefinitionSeedService>();
builder.Services.AddScoped<IHeroTreeProvider, HeroTreeProvider>();


builder.Services.AddScoped<IUserProfileService, UserProfileService>();

builder.Services.AddScoped<ITreeOfLightTranslationService, TreeOfLightTranslationService>();
builder.Services.AddScoped<ITreeOfLightRepository, TreeOfLightRepository>();
builder.Services.AddScoped<ITreeOfLightService, TreeOfLightService>();
builder.Services.AddScoped<ITreeModelRepository, TreeModelRepository>();
builder.Services.AddScoped<ITreeModelService, TreeModelService>();

builder.Services.AddScoped<ITreeOfHeroesRepository, TreeOfHeroesRepository>();
builder.Services.AddScoped<ITreeOfHeroesService, TreeOfHeroesService>();

builder.Services.AddScoped<IStoriesService, StoriesService>();
builder.Services.AddScoped<IStoriesRepository, StoriesRepository>();
builder.Services.AddScoped<XooCreator.BA.Features.StoryEditor.Repositories.IStoryCraftsRepository, XooCreator.BA.Features.StoryEditor.Repositories.StoryCraftsRepository>();
builder.Services.AddScoped<IStoryEditorService, StoryEditorService>();

// Story Marketplace Services
builder.Services.AddScoped<IStoriesMarketplaceRepository, StoriesMarketplaceRepository>();
builder.Services.AddScoped<IStoriesMarketplaceService, StoriesMarketplaceService>();

// Payment services
builder.Services.AddScoped<IPaymentService, PaymentService>();

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
    var storiesService = scope.ServiceProvider.GetRequiredService<IStoriesService>();
    var treeModelService = scope.ServiceProvider.GetRequiredService<ITreeModelService>();
    var discoverySeeder = scope.ServiceProvider.GetRequiredService<ISeedDiscoveryService>();
    var bestiaryUpdater = scope.ServiceProvider.GetRequiredService<IBestiaryFileUpdater>();
    var heroDefinitionSeeder = scope.ServiceProvider.GetRequiredService<IHeroDefinitionSeedService>();

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
                
                // Apply migrations to recreate all tables
                await context.Database.MigrateAsync();
                Console.WriteLine("✅ Migrations applied successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Schema recreation failed: {ex.Message}");
                
                // Fallback: try normal migration in case schema operations failed
                try
                {
                    await context.Database.MigrateAsync();
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
            // Normal migration path for production
            await context.Database.MigrateAsync();
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
