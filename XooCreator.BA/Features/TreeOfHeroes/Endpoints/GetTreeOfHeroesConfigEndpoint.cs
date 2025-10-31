using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Features.TreeOfHeroes.Services;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetTreeOfHeroesConfigEndpoint
{
    [Route("/api/{locale}/tree-of-heroes/config")] // GET
    [Authorize]
    public static async Task<IResult> HandleGet(
        [FromRoute] string locale,
        [FromServices] ITreeOfHeroesService treeOfHeroesService,
        CancellationToken ct)
    {
        try
        {
            var config = await treeOfHeroesService.GetTreeOfHeroesConfigAsync();
            return Results.Ok(config);
        }
        catch (Exception ex)
        {
            return Results.Problem($"Failed to get tree of heroes config: {ex.Message}");
        }
    }
}
