using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Npgsql;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Repositories;
using XooCreator.BA.Endpoints;
using XooCreator.BA.Services;
using XooCreator.BA.Features.TreeOfLight;
using XooCreator.BA.Features.Stories;
using XooCreator.BA.Infrastructure;

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
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyMethod()
              .AllowAnyHeader()
              .AllowCredentials();
    });

    // More permissive policy for development
    options.AddPolicy("AllowDevelopment", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
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

// Tree of Light Services
builder.Services.AddScoped<ITreeOfLightRepository, TreeOfLightRepository>();
builder.Services.AddScoped<ITreeOfLightService, TreeOfLightService>();

// Stories Services
builder.Services.AddScoped<IStoriesRepository, StoriesRepository>();
builder.Services.AddScoped<IStoriesService, StoriesService>();

var app = builder.Build();

// Auto-migrate database on startup (for Railway)
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<XooDbContext>();
    var storiesService = scope.ServiceProvider.GetRequiredService<IStoriesService>();
    
    try
    {
        context.Database.Migrate();
        // Initialize stories after migration
        await storiesService.InitializeStoriesAsync();
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

if (app.Environment.IsDevelopment())
{
    app.UseCors("AllowDevelopment");
}
else
{
    app.UseCors("AllowProduction");
}

// Map endpoints by domain
app.MapControllers(); // Left for any remaining controllers (can be removed later if none)
app.MapCreatureBuilderEndpoints();
app.MapSystemEndpoints();
app.MapStoryEndpoints();

app.Run();
