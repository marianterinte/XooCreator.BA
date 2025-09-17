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
                    case "get": 
                        app.MapGet(route, del);
                        Console.WriteLine($"\u001b[32m✓\u001b[0m \u001b[36mGET\u001b[0m \u001b[33m{route}\u001b[0m \u001b[90m->\u001b[0m \u001b[35m{type.Name}.{method.Name}\u001b[0m");
                        break;
                    case "post": 
                        app.MapPost(route, del);
                        Console.WriteLine($"\u001b[32m✓\u001b[0m \u001b[34mPOST\u001b[0m \u001b[33m{route}\u001b[0m \u001b[90m->\u001b[0m \u001b[35m{type.Name}.{method.Name}\u001b[0m");
                        break;
                    case "put": 
                        app.MapPut(route, del);
                        Console.WriteLine($"\u001b[32m✓\u001b[0m \u001b[33mPUT\u001b[0m \u001b[33m{route}\u001b[0m \u001b[90m->\u001b[0m \u001b[35m{type.Name}.{method.Name}\u001b[0m");
                        break;
                    case "delete": 
                        app.MapDelete(route, del);
                        Console.WriteLine($"\u001b[32m✓\u001b[0m \u001b[31mDELETE\u001b[0m \u001b[33m{route}\u001b[0m \u001b[90m->\u001b[0m \u001b[35m{type.Name}.{method.Name}\u001b[0m");
                        break;
                    case "patch": 
                        app.MapMethods(route, new[] { "PATCH" }, del);
                        Console.WriteLine($"\u001b[32m✓\u001b[0m \u001b[37mPATCH\u001b[0m \u001b[33m{route}\u001b[0m \u001b[90m->\u001b[0m \u001b[35m{type.Name}.{method.Name}\u001b[0m");
                        break;
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
