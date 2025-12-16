using Microsoft.OpenApi.Models;
using XooCreator.BA.Infrastructure.Swagger;

namespace XooCreator.BA.Infrastructure.Configuration;

public static class SwaggerConfiguration
{
    public static IServiceCollection AddSwaggerConfiguration(this IServiceCollection services)
    {
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "XooCreator.BA",
                Version = "v1.0.0",
                Description = "XooCreator Backend API"
            });
            
            // Use full type name as schemaId to avoid conflicts between types with same name in different namespaces
            c.CustomSchemaIds(type => type.FullName?.Replace("+", ".") ?? type.Name);
            
            c.OperationFilter<LocaleParameterOperationFilter>();
            c.OperationFilter<BusinessFolderTagOperationFilter>();

            c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Description = "Enter 'Bearer {token}'"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
} 
