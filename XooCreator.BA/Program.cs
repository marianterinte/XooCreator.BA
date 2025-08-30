using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
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
});

// EF Core PostgreSQL
builder.Services.AddDbContext<XooDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Postgres");
    options.UseNpgsql(cs);
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "XooCreator.BA v1.0.0");
        c.RoutePrefix = "swagger";
    });
    
    // Use more permissive CORS in development
    app.UseCors("AllowDevelopment");
}
else
{
    // Use specific CORS policy in production
    app.UseCors("AllowAngularDev");
}

app.UseHttpsRedirection();

// Minimal API endpoint for builder data from DB
app.MapGet("/api/builder/data", async (XooDbContext db, CancellationToken ct) =>
{
    var parts = await db.BodyParts
        .Select(p => new { key = p.Key, name = p.Name, image = p.Image })
        .ToListAsync(ct);

    var animalsRaw = await db.Animals
        .Select(a => new
        {
            src = a.Src,
            label = a.Label,
            supports = a.SupportedParts.Select(sp => sp.PartKey)
        })
        .ToListAsync(ct);

    var config = await db.BuilderConfigs.FirstOrDefaultAsync(ct);
    var baseLockedParts = await db.BodyParts.Where(p => p.IsBaseLocked).Select(p => p.Key).ToListAsync(ct);

    return Results.Ok(new
    {
        parts,
        animals = animalsRaw,
        baseUnlockedAnimalCount = config?.BaseUnlockedAnimalCount ?? 3,
        baseLockedParts
    });
})
.WithName("GetCreatureBuilderData")
.WithTags("Builder")
.Produces<object>(StatusCodes.Status200OK);

// Simple DB health endpoint (tests EF connectivity)
app.MapGet("/api/db/health", async (XooDbContext db, CancellationToken ct) =>
{
    try
    {
        var canConnect = await db.Database.CanConnectAsync(ct);
        var pending = await db.Database.GetPendingMigrationsAsync(ct);
        return Results.Ok(new { canConnect, pendingMigrations = pending });
    }
    catch (Exception ex)
    {
        return Results.Problem(ex.Message);
    }
})
.WithName("DbHealth")
.WithTags("System")
.Produces<object>(StatusCodes.Status200OK)
.Produces<ProblemDetails>(StatusCodes.Status500InternalServerError);

app.Run();
