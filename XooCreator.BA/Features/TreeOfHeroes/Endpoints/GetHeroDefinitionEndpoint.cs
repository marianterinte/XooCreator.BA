using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
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
    public static async Task<Results<Ok<HeroDefinitionDto>, NotFound, UnauthorizedHttpResult>> HandleGet(
        [FromServices] GetHeroDefinitionEndpoint ep,
        [FromRoute] string heroId)
    {
        var definition = await ep._service.GetHeroDefinitionByIdAsync(heroId);
        
        if (definition == null)
        {
            return TypedResults.NotFound();
        }

        return TypedResults.Ok(definition);
    }
}
