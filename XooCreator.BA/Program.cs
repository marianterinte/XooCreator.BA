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

// Add services
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
});

// === CORS: o singură policy super permisivă, valabilă peste tot ===
// Dacă ai nevoie de cookies/credențiale, păstrează AllowCredentials + SetIsOriginAllowed(true).
// Dacă NU ai nevoie de credențiale, vezi varianta comentată mai jos.
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

    // VARIANTĂ FĂRĂ CREDENȚIALE (înlocuiește policy-ul de mai sus dacă nu folosești cookies):
    // options.AddPolicy("AllowAll", policy =>
    //     policy
    //         .AllowAnyOrigin()
    //         .AllowAnyMethod()
    //         .AllowAnyHeader()
    //         .SetPreflightMaxAge(TimeSpan.FromHours(24))
    // );
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
builder.Services.AddScoped<IBestiaryFileUpdater, BestiaryFileUpdater>();

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
app.UseCors("AllowAll");

// Auto-migrate database on startup + initializare date
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<XooDbContext>();
    var storiesService = scope.ServiceProvider.GetRequiredService<IStoriesService>();
    var treeModelService = scope.ServiceProvider.GetRequiredService<ITreeModelService>();
    var discoverySeeder = scope.ServiceProvider.GetRequiredService<ISeedDiscoveryService>();
    var bestiaryUpdater = scope.ServiceProvider.GetRequiredService<IBestiaryFileUpdater>();

    try
    {
        var recreate = builder.Configuration.GetValue<bool>("Database:RecreateOnStart");
        recreate = true;
        if (recreate)
        {
            await context.Database.EnsureDeletedAsync();
        }

        await context.Database.MigrateAsync();

        // Seed discovery items (63 combos)
        await discoverySeeder.EnsureSeedAsync();
        await bestiaryUpdater.EnsureImageFileNamesAsync();

        if (recreate)
        {
            var wallets = await context.CreditWallets.ToListAsync();
            foreach (var w in wallets)
            {
                if (w.DiscoveryBalance < 10) w.DiscoveryBalance = 10;
            }
            await context.SaveChangesAsync();
        }

        await storiesService.InitializeStoriesAsync();
        await treeModelService.InitializeTreeModelAsync();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Migration or initialization failed: {ex.Message}");
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

app.Run();
