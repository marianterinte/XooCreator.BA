using System.Reflection;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace XooCreator.BA.Infrastructure.Endpoints;

public static class EndpointRegistrationExtensions
{
    public static IServiceCollection AddEndpointDefinitions(this IServiceCollection services, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            assemblies = new[] { Assembly.GetExecutingAssembly() };

        var endpointTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<EndpointAttribute>() != null);

        foreach (var type in endpointTypes)
        {
            services.AddScoped(type);
        }

        return services;
    }

    public static IEndpointRouteBuilder MapDiscoveredEndpoints(this IEndpointRouteBuilder app, params Assembly[] assemblies)
    {
        if (assemblies == null || assemblies.Length == 0)
            assemblies = new[] { Assembly.GetExecutingAssembly() };

        var endpointTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => !t.IsAbstract && !t.IsInterface && t.GetCustomAttribute<EndpointAttribute>() != null);

        foreach (var type in endpointTypes)
        {
            var methods = type.GetMethods(BindingFlags.Public | BindingFlags.Static)
                .Where(m => m.Name.StartsWith("Handle", StringComparison.Ordinal));

            foreach (var method in methods)
            {
                var verbPart = method.Name["Handle".Length..];
                if (string.IsNullOrWhiteSpace(verbPart)) continue;
                var httpVerb = verbPart.ToLowerInvariant();

                var routeAttr = method.GetCustomAttribute<RouteAttribute>();
                var route = routeAttr?.Template ?? $"/api/{type.Name.Replace("Endpoint", string.Empty).ToKebabCase()}/{verbPart.ToLowerInvariant()}";

                var del = method.CreateDelegate(GetDelegateType(method));

                switch (httpVerb)
                {
                    case "get": app.MapGet(route, del); break;
                    case "post": app.MapPost(route, del); break;
                    case "put": app.MapPut(route, del); break;
                    case "delete": app.MapDelete(route, del); break;
                    case "patch": app.MapMethods(route, new[] { "PATCH" }, del); break;
                }
            }
        }

        return app;
    }

    private static Type GetDelegateType(MethodInfo method)
    {
        var parameterTypes = method.GetParameters().Select(p => p.ParameterType).ToList();
        var returnType = method.ReturnType;
        return Expression.GetDelegateType(parameterTypes.Concat(new[] { returnType }).ToArray());
    }

    private static string ToKebabCase(this string value)
    {
        if (string.IsNullOrEmpty(value)) return value;
        var chars = new List<char>(value.Length + 10);
        for (int i = 0; i < value.Length; i++)
        {
            var c = value[i];
            if (char.IsUpper(c))
            {
                if (i > 0) chars.Add('-');
                chars.Add(char.ToLowerInvariant(c));
            }
            else
            {
                chars.Add(c);
            }
        }
        return new string(chars.ToArray());
    }
}
