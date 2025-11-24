using Microsoft.AspNetCore.Authentication.JwtBearer;
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

        services.AddAuthorization();

        return services;
    }
} 
