using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace XooCreator.BA.Data;

public sealed class XooDbContextFactory : IDesignTimeDbContextFactory<XooDbContext>
{
    public XooDbContext CreateDbContext(string[] args)
    {
        var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";

        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile($"appsettings.{env}.json", optional: true, reloadOnChange: false)
            .AddUserSecrets<XooDbContextFactory>(optional: true)   // if you use secrets
            .AddEnvironmentVariables()
            .Build();

        var cs = config.GetConnectionString("DefaultConnection");
        if (string.IsNullOrWhiteSpace(cs))
        {
            cs = config.GetConnectionString("Postgres");
        }
        if (string.IsNullOrWhiteSpace(cs))
            throw new InvalidOperationException("Connection string 'DefaultConnection' or 'Postgres' not found.");

        var options = new DbContextOptionsBuilder<XooDbContext>()
            .UseNpgsql(cs)
            .Options;

        return new XooDbContext(options, config);
    }
}
