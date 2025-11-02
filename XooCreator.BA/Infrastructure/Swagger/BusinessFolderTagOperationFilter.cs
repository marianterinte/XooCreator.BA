using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;
using System.Text.RegularExpressions;

namespace XooCreator.BA.Infrastructure.Swagger
{
    /// <summary>
    /// Operation filter that automatically groups endpoints by business folder based on their namespace.
    /// Extracts the feature name from namespace (e.g., Features.Stories.Endpoints -> "Stories")
    /// or handles subfolders (e.g., Features.TalesOfAlchimalia.Market.Endpoints -> "TalesOfAlchimalia")
    /// </summary>
    public class BusinessFolderTagOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            // Get the method info from the context
            var methodInfo = context.MethodInfo;
            if (methodInfo == null || methodInfo.DeclaringType == null)
                return;

            var declaringType = methodInfo.DeclaringType;
            var namespaceName = declaringType.Namespace ?? string.Empty;

            // Only process endpoints in Features namespace
            if (!namespaceName.Contains(".Features."))
                return;

            // Extract business folder name from namespace
            // Pattern: XooCreator.BA.Features.{BusinessFolder}.{SubFolder}.Endpoints
            // or: XooCreator.BA.Features.{BusinessFolder}.Endpoints
            var featureMatch = Regex.Match(namespaceName, @"\.Features\.([^\.]+)");
            
            if (featureMatch.Success)
            {
                var businessFolder = featureMatch.Groups[1].Value;
                
                // Format the tag name nicely (e.g., "TreeOfHeroes" -> "Tree Of Heroes")
                var formattedTag = FormatTagName(businessFolder);
                
                // Initialize tags list if needed
                operation.Tags ??= new List<OpenApiTag>();
                
                // Remove any auto-generated tags (usually the endpoint class name)
                // but keep manually set tags (like "Auth") if they exist
                var manuallySetTags = operation.Tags
                    .Where(t => t.Name == "Auth" || t.Name == "System" || preferredTags.Contains(t.Name))
                    .ToList();
                
                // Clear all tags and add the business folder tag first, then manually set tags
                operation.Tags.Clear();
                
                // Add the business folder tag
                operation.Tags.Add(new OpenApiTag { Name = formattedTag });
                
                // Add back manually set tags if they exist and are different
                foreach (var manualTag in manuallySetTags.Where(t => t.Name != formattedTag))
                {
                    if (!operation.Tags.Any(t => t.Name == manualTag.Name))
                    {
                        operation.Tags.Add(manualTag);
                    }
                }
            }
        }
        
        // Tags that should be preserved if manually set
        private static readonly HashSet<string> preferredTags = new HashSet<string>
        {
            "Auth",
            "System",
            "Health"
        };

        private string FormatTagName(string folderName)
        {
            // Convert PascalCase to readable format
            // "TreeOfHeroes" -> "Tree Of Heroes"
            // "TalesOfAlchimalia" -> "Tales Of Alchimalia"
            // "Stories" -> "Stories"
            
            if (string.IsNullOrEmpty(folderName))
                return folderName;

            // Insert space before capital letters (except the first one)
            var result = Regex.Replace(folderName, "(?<!^)([A-Z])", " $1");
            
            return result;
        }
    }
}

