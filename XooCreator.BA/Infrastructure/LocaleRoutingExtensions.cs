using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace XooCreator.BA.Infrastructure;

public static class LocaleRoutingExtensions
{
    private static readonly Regex LocaleRegex = new Regex("^[a-z]{2}(?:-[a-z]{2})?$", RegexOptions.IgnoreCase | RegexOptions.Compiled);
    public const string RequestLocaleItemKey = "RequestLocale";

    public static IApplicationBuilder UseLocaleInApiPath(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            var path = context.Request.Path.Value ?? string.Empty;
            if (path.StartsWith("/api/", StringComparison.OrdinalIgnoreCase))
            {
                // Split: "", "api", maybe "{locale}", rest...
                var segments = path.Split('/', StringSplitOptions.RemoveEmptyEntries);
                if (segments.Length >= 2)
                {
                    var maybeLocale = segments[1];
                    if (LocaleRegex.IsMatch(maybeLocale))
                    {
                        // Store normalized locale
                        context.Items[RequestLocaleItemKey] = maybeLocale.ToLowerInvariant();

                        // Rewrite path to strip locale: /api/{rest}
                        var restSegments = segments.Length > 2 ? 
                            "/" + string.Join('/', segments.Skip(2)) : "/";
                        context.Request.Path = new PathString("/api" + restSegments.TrimEnd('/'));
                    }
                }
            }

            await next();
        });
    }
}


