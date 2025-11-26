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
            var cs = ResolveConnectionString(configuration);
            var dbSchema = configuration.GetValue<string>("Database:Schema") ?? "public";
            
            // Add search_path to connection string if schema is not public
            if (dbSchema != "public")
            {
                var builder = new NpgsqlConnectionStringBuilder(cs);
                builder.SearchPath = dbSchema;
                cs = builder.ConnectionString;
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

    private static string ResolveConnectionString(IConfiguration configuration)
    {
        var dbUrl = Environment.GetEnvironmentVariable("DATABASE_URL");

        if (string.IsNullOrWhiteSpace(dbUrl))
        {
            var configured = configuration.GetConnectionString("Postgres");
            if (!string.IsNullOrWhiteSpace(configured))
            {
                if (configured.StartsWith("env:", StringComparison.OrdinalIgnoreCase))
                {
                    var envName = configured.Substring(4).Trim();
                    if (!string.IsNullOrWhiteSpace(envName))
                    {
                        dbUrl = Environment.GetEnvironmentVariable(envName);
                    }
                }
                else
                {
                    dbUrl = configured;
                }
            }
        }

        if (string.IsNullOrWhiteSpace(dbUrl))
        {
            return "Host=localhost;Port=5432;Database=xoo_db;Username=postgres;Password=admin";
        }

        return BuildConnectionString(dbUrl);
    }

    private static string BuildConnectionString(string value)
    {
        if (!value.Contains("://", StringComparison.Ordinal))
        {
            return value;
        }

        if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
        {
            return value;
        }

        var userInfo = uri.UserInfo.Split(':', 2);
        var npg = new NpgsqlConnectionStringBuilder
        {
            Host = uri.Host,
            Port = uri.Port,
            Username = userInfo.ElementAtOrDefault(0) ?? "postgres",
            Password = userInfo.ElementAtOrDefault(1) ?? string.Empty,
            Database = uri.AbsolutePath.Trim('/'),
            SslMode = SslMode.Require
        };

        return npg.ConnectionString;
    }
}
