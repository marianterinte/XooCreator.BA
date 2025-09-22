using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;

namespace XooCreator.BA.Features.TreeOfHeroes.Endpoints;

[Endpoint]
public class GetTreeOfHeroesConfigEndpoint
{
    [Route("/api/tree-of-heroes/config")] // GET
    public static async Task<IResult> HandleGet(
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
