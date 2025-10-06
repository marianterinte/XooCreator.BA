using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetHeroDefinitionEndpoint
{
    private readonly ITreeOfHeroesService _service;

    public GetHeroDefinitionEndpoint(ITreeOfHeroesService service)
    {
        _service = service;
    }

    [Route("/api/{locale}/tree-of-heroes/definitions/{heroId}")] // GET
    [Authorize]
    public static async Task<Results<Ok<HeroDefinitionDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetHeroDefinitionEndpoint ep,
        [FromRoute] string heroId)
    {
        var definition = await ep._service.GetHeroDefinitionByIdAsync(heroId, locale);
        
        if (definition == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(definition);
    }
}
