using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace XooCreator.BA.Infrastructure.Configuration;

public static class AuthConfiguration
{
    public static IServiceCollection AddAuthConfiguration(this IServiceCollection services, IConfiguration configuration)
    {
        var auth0Section = configuration.GetSection("Auth0");
        var auth0Domain = auth0Section["Domain"];
        var auth0Audience = auth0Section["Audience"];

        services.AddAuthentication(options =>
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
                OnMessageReceived = context =>
                {
                    // EventSource (SSE) cannot set custom headers, so we support passing the JWT in query string
                    // ONLY for our SSE endpoint(s).
                    var path = context.HttpContext.Request.Path;
                    if (path.HasValue &&
                        path.Value.StartsWith("/api/jobs/", StringComparison.OrdinalIgnoreCase) &&
                        path.Value.EndsWith("/events", StringComparison.OrdinalIgnoreCase))
                    {
                        var accessToken = context.HttpContext.Request.Query["access_token"].ToString();
                        if (!string.IsNullOrWhiteSpace(accessToken))
                        {
                            context.Token = accessToken;
                        }
                    }

                    return Task.CompletedTask;
                },
                OnAuthenticationFailed = context =>
                {
                    var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                    var logger = loggerFactory?.CreateLogger("XooCreator.BA.Infrastructure.Configuration.JwtBearer");
                    logger?.LogWarning("[JWT] Authentication failed: {Message}", context.Exception.Message);
                    return Task.CompletedTask;
                },
                OnChallenge = context =>
                {
                    if (context.AuthenticateFailure != null)
                    {
                        var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                        var logger = loggerFactory?.CreateLogger("XooCreator.BA.Infrastructure.Configuration.JwtBearer");
                        logger?.LogWarning("[JWT] Challenge failed: {Message}", context.AuthenticateFailure.Message);
                    }
                    return Task.CompletedTask;
                },
                OnTokenValidated = context =>
                {
                    var iss = context.Principal?.FindFirst("iss")?.Value ?? "<none>";
                    var aud = string.Join(",", context.Principal?.FindAll("aud").Select(c => c.Value) ?? Array.Empty<string>());
                    var sub = context.Principal?.FindFirst("sub")?.Value ?? "<none>";
                    var loggerFactory = context.HttpContext.RequestServices.GetService<ILoggerFactory>();
                    var logger = loggerFactory?.CreateLogger("XooCreator.BA.Infrastructure.Configuration.JwtBearer");
                    logger?.LogDebug("[JWT] Token validated. iss={Iss} aud={Aud} sub={Sub}", iss, aud, sub);
                    return Task.CompletedTask;
                }
            };
        });

        services.AddAuthorization();

        return services;
    }
} 
