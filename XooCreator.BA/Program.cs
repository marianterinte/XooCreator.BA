using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Repositories;
using XooCreator.BA.Services;
using XooCreator.BA.Features.TreeOfLight;
using XooCreator.BA.Features.Stories;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

var builder = WebApplication.CreateBuilder(args);

// Bind port from PORT env var (Railway) if provided
var portEnv = Environment.GetEnvironmentVariable("PORT");
if (!string.IsNullOrWhiteSpace(portEnv))
{
    builder.WebHost.UseUrls($"http://0.0.0.0:{portEnv}");
}

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// register endpoint handlers via reflection
builder.Services.AddEndpointDefinitions();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "XooCreator.BA",
        Version = "v1.0.0",
        Description = "XooCreator Backend API"
    });
});

// Add CORS policy for Angular frontend
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAngularDev", policy =>
    {
        policy.WithOrigins("http://localhost:4200", "https://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // More permissive policy for development
    options.AddPolicy("AllowDevelopment", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader()
              .SetIsOriginAllowed(origin => true) // Allow any origin for development
              .SetPreflightMaxAge(TimeSpan.FromSeconds(86400)); // Cache preflight for 24 hours
    });

    // Railway production policy
    options.AddPolicy("AllowProduction", policy =>
    {
        policy.AllowAnyOrigin() // Railway apps can have dynamic URLs
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// EF Core PostgreSQL
builder.Services.AddDbContext<XooDbContext>(options =>
{
    // Railway provides DATABASE_URL in URL form: postgres://user:pass@host:port/db
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

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

// Services
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IUserContextService, UserContextService>();
builder.Services.AddScoped<IDbHealthService, DbHealthService>();
builder.Services.AddScoped<ICreatureBuilderService, CreatureBuilderService>();
builder.Services.AddScoped<ISeedDiscoveryService, SeedDiscoveryService>();

// User Services
builder.Services.AddScoped<XooCreator.BA.Features.User.IUserProfileService, XooCreator.BA.Features.User.UserProfileService>();

// Tree of Light Services
builder.Services.AddScoped<ITreeOfLightRepository, TreeOfLightRepository>();
builder.Services.AddScoped<ITreeOfLightService, TreeOfLightService>();
builder.Services.AddScoped<ITreeModelRepository, TreeModelRepository>();
builder.Services.AddScoped<ITreeModelService, TreeModelService>();

// Stories Services
builder.Services.AddScoped<IStoriesRepository, StoriesRepository>();
builder.Services.AddScoped<IStoriesService, StoriesService>();

var app = builder.Build();

// Auto-migrate database on startup (and optionally recreate in Development)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<XooDbContext>();
    var storiesService = scope.ServiceProvider.GetRequiredService<IStoriesService>();
    var treeModelService = scope.ServiceProvider.GetRequiredService<ITreeModelService>();
    var discoverySeeder = scope.ServiceProvider.GetRequiredService<ISeedDiscoveryService>();

    try
    {
        // Only allow destructive recreate in Development and when explicitly enabled
        var isDevelopment = app.Environment.IsDevelopment();
        var recreate = builder.Configuration.GetValue<bool>("Database:RecreateOnStart");


        // TODO : disable when in production unless explicitly needed
        if (recreate)
        {
            await context.Database.EnsureDeletedAsync();
        }

        await context.Database.MigrateAsync();

        // Seed discovery items (63 combos)
        await discoverySeeder.EnsureSeedAsync();

        // Dev convenience: ensure discovery credits for test users
        if (recreate)
        {
            var wallets = await context.CreditWallets.ToListAsync();
            foreach (var w in wallets)
            {
                if (w.DiscoveryBalance < 10) w.DiscoveryBalance = 10; // give some discovery credits for testing
            }
            await context.SaveChangesAsync();
        }

        // Initialize stories after migration
        await storiesService.InitializeStoriesAsync();
        // Initialize tree model after stories are seeded
        await treeModelService.InitializeTreeModelAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration or initialization failed: {ex.Message}");
    }
}

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c =>
{
    c.SwaggerEndpoint("/swagger/v1/swagger.json", "XooCreator.BA v1.0.0");
    c.RoutePrefix = "swagger";
});

// Apply CORS before other middleware
if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowDevelopment");
}
else
{
    app.UseCors("AllowProduction");
}

// Map endpoints by domain
// app.MapControllers(); // removed legacy controllers after migration to minimal APIs
// app.MapCreatureBuilderEndpoints(); // migrated to [Endpoint] pattern
// app.MapSystemEndpoints(); // migrated to [Endpoint] pattern
// app.MapStoryEndpoints(); // replaced by discovered endpoints
app.MapDiscoveredEndpoints();

app.Run();
