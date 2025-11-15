using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GetStoryAgeGroupsEndpoint
{
    private readonly XooDbContext _context;

    public GetStoryAgeGroupsEndpoint(XooDbContext context)
    {
        _context = context;
    }

    public record AgeGroupDto(
        string Id,
        string Label,
        int MinAge,
        int MaxAge,
        string? Description,
        int SortOrder
    );

    public record AgeGroupsResponse(
        List<AgeGroupDto> AgeGroups
    );

    [Route("/api/{locale}/story-editor/age-groups")]
    [Authorize]
    public static async Task<Results<Ok<AgeGroupsResponse>, BadRequest<string>>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetStoryAgeGroupsEndpoint ep,
        CancellationToken ct)
    {
        var lang = locale.ToLowerInvariant();

        try
        {
            // Load all age groups with their translations
            var ageGroups = await ep._context.StoryAgeGroups
                .Include(ag => ag.Translations)
                .Where(ag => ag.IsActive)
                .OrderBy(ag => ag.SortOrder)
                .ToListAsync(ct);

            var ageGroupDtos = ageGroups.Select(ag => new AgeGroupDto(
                Id: ag.AgeGroupId,
                Label: ag.Translations
                    .FirstOrDefault(tr => tr.LanguageCode == lang)?.Label
                    ?? ag.Translations.FirstOrDefault()?.Label
                    ?? ag.AgeGroupId,
                MinAge: ag.MinAge,
                MaxAge: ag.MaxAge,
                Description: ag.Translations
                    .FirstOrDefault(tr => tr.LanguageCode == lang)?.Description
                    ?? ag.Translations.FirstOrDefault()?.Description,
                SortOrder: ag.SortOrder
            )).ToList();

            return TypedResults.Ok(new AgeGroupsResponse(AgeGroups: ageGroupDtos));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Failed to load age groups: {ex.Message}");
        }
    }
}

