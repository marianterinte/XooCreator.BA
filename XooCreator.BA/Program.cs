using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Features.Builder;
using XooCreator.BA.Data.Repositories;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Minimal API - keep controllers for Swagger grouping if needed
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// EF Core PostgreSQL
builder.Services.AddDbContext<XooDbContext>(options =>
{
    var cs = builder.Configuration.GetConnectionString("Postgres");
    options.UseNpgsql(cs);
});

// Repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Minimal API endpoint for builder data
app.MapGet("/api/builder/data", () => Results.Ok(BuilderData.GetCreatureBuilderData()))
    .WithName("GetCreatureBuilderData")
    .WithTags("Builder")
    .Produces(StatusCodes.Status200OK);

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
.Produces(StatusCodes.Status200OK)
.Produces(StatusCodes.Status500InternalServerError);

app.Run();
