using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetAuthorsEndpoint
{
    public record AuthorsResponse(
        List<string> Authors
    );

    [Route("/api/{locale}/story-editor/authors")]
    [Authorize]
    public static Results<Ok<AuthorsResponse>, BadRequest<string>> HandleGet(
        [FromRoute] string locale)
    {
        var lang = locale.ToLowerInvariant();
        
        try
        {
            var authors = GetAuthorsForLanguage(lang);
            return TypedResults.Ok(new AuthorsResponse(Authors: authors));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Failed to load authors: {ex.Message}");
        }
    }

    private static List<string> GetAuthorsForLanguage(string lang)
    {
        try
        {
            var basePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Data", "SeedData", "Story-Editor", "authors.json");
            
            if (!File.Exists(basePath))
            {
                return new List<string>();
            }

            var json = File.ReadAllText(basePath);
            var data = JsonSerializer.Deserialize<AuthorsData>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Map language codes to author list keys
            return lang switch
            {
                "ro-ro" => data?.RomanianAuthors ?? new List<string>(),
                "hu-hu" => data?.HungarianAuthors ?? new List<string>(),
                "en-us" or "en-gb" => data?.EnglishAmericanAuthors ?? new List<string>(),
                _ => data?.EnglishAmericanAuthors ?? new List<string>() // Default to English
            };
        }
        catch
        {
            return new List<string>();
        }
    }

    // DTOs for JSON deserialization
    private class AuthorsData
    {
        [JsonPropertyName("romanian_authors")]
        public List<string>? RomanianAuthors { get; set; }
        
        [JsonPropertyName("hungarian_authors")]
        public List<string>? HungarianAuthors { get; set; }
        
        [JsonPropertyName("english_american_authors")]
        public List<string>? EnglishAmericanAuthors { get; set; }
    }
}

