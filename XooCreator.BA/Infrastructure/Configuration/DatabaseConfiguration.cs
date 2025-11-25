using Microsoft.EntityFrameworkCore;
using Npgsql;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Interceptors;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace XooCreator.BA.Infrastructure.Configuration;

public static class DatabaseConfiguration
{
    public static IServiceCollection AddDatabaseConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<XooDbContext>(options =>
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
                cs = configuration.GetConnectionString("Postgres")
                    ?? "Host=localhost;Port=5432;Database=xoo_db;Username=postgres;Password=admin";
            }

            options.UseNpgsql(cs);
            
            // Add interceptor to automatically make migration SQL commands idempotent
            // This transforms CREATE TABLE, CREATE INDEX, ALTER TABLE ADD CONSTRAINT, etc.
            // to use IF NOT EXISTS, making all migrations safe to run multiple times
            var loggerFactory = services.BuildServiceProvider().GetService<ILoggerFactory>();
            var logger = loggerFactory?.CreateLogger<IdempotentMigrationCommandInterceptor>();
            options.AddInterceptors(new IdempotentMigrationCommandInterceptor(logger));
        });

        return services;
    }
}
