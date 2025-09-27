using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace XooCreator.BA.Infrastructure.Swagger
{
    public class LocaleParameterOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            var hasLocale = context.ApiDescription.RelativePath?.Contains("{locale}") == true;
            if (!hasLocale) return;

            operation.Parameters ??= new List<OpenApiParameter>();
            // Ensure a locale parameter exists with example
            if (!operation.Parameters.Any(p => string.Equals(p.Name, "locale", StringComparison.OrdinalIgnoreCase)))
            {
                operation.Parameters.Insert(0, new OpenApiParameter
                {
                    Name = "locale",
                    In = ParameterLocation.Path,
                    Required = true,
                    Schema = new OpenApiSchema { Type = "string" },
                    Description = "Locale segment (e.g., ro-ro, en-us)",
                    Example = new Microsoft.OpenApi.Any.OpenApiString("ro-ro")
                });
            }
        }
    }
}
