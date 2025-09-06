using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Services;

namespace XooCreator.BA.Features.CreatureBuilder.Endpoints;

[Endpoint]
public class GetCreatureBuilderDataEndpoint
{
    private readonly ICreatureBuilderService _service;
    public GetCreatureBuilderDataEndpoint(ICreatureBuilderService service) => _service = service;

    [Route("/api/creature-builder/data")] // GET /api/creature-builder/data
    public static async Task<IResult> HandleGet([FromServices] GetCreatureBuilderDataEndpoint ep, CancellationToken ct)
    {
        var data = await ep._service.GetDataAsync(ct);
        var shaped = new
        {
            parts = data.Parts.Select(p => new { key = p.Key, name = p.Name, image = p.Image }),
            animals = data.Animals.Select(a => new { src = a.Src, label = a.Label, supports = a.Supports }),
            baseUnlockedAnimalCount = data.BaseUnlockedAnimalCount,
            baseLockedParts = data.BaseLockedParts
        };
        return Results.Json(shaped);
    }
}
