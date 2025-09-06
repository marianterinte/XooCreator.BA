using XooCreator.BA.Services;

namespace XooCreator.BA.Features.Endpoints;

public static class CreatureBuilderEndpoints
{
    public static IEndpointRouteBuilder MapCreatureBuilderEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/creature-builder").WithTags("CreatureBuilder");

        group.MapGet("/data", async (ICreatureBuilderService service, CancellationToken ct) =>
        {
            var data = await service.GetDataAsync(ct);
            return Results.Ok(new
            {
                parts = data.Parts.Select(p => new { key = p.Key, name = p.Name, image = p.Image }),
                animals = data.Animals.Select(a => new { src = a.Src, label = a.Label, supports = a.Supports }),
                baseUnlockedAnimalCount = data.BaseUnlockedAnimalCount,
                baseLockedParts = data.BaseLockedParts
            });
        })
        .WithName("GetCreatureBuilderData")
        .Produces<object>(StatusCodes.Status200OK);

        return app;
    }
}
