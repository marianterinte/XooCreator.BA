using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetAuthorsEndpoint
{
    public record AuthorDto(
        Guid Id,
        string Name
    );

    public record AuthorsResponse(
        List<AuthorDto> Authors
    );

    [Route("/api/{locale}/story-editor/authors")]
    [Authorize]
    public static async Task<Results<Ok<AuthorsResponse>, BadRequest<string>>> HandleGet(
        [FromRoute] string locale,
        [FromServices] XooDbContext dbContext)
    {
        var lang = locale.ToLowerInvariant();
        
        try
        {
            var authors = await dbContext.ClassicAuthors
                .Where(a => a.LanguageCode == lang && a.IsActive)
                .OrderBy(a => a.SortOrder)
                .ThenBy(a => a.Name)
                .Select(a => new AuthorDto(a.Id, a.Name))
                .ToListAsync();
            
            return TypedResults.Ok(new AuthorsResponse(Authors: authors));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Failed to load authors: {ex.Message}");
        }
    }
}

