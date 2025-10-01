using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetHeroDefinitionsEndpoint
{
    private readonly ITreeOfHeroesService _service;

    public GetHeroDefinitionsEndpoint(ITreeOfHeroesService service)
    {
        _service = service;
    }

    [Route("/api/{locale}/tree-of-heroes/definitions")] // GET
    public static async Task<Results<Ok<List<HeroDefinitionDto>>, UnauthorizedHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromServices] GetHeroDefinitionsEndpoint ep)
    {
        var definitions = await ep._service.GetHeroDefinitionsAsync(locale);
        return TypedResults.Ok(definitions);
    }
}
