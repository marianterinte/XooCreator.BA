using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Features.Assets.DTOs;

namespace XooCreator.BA.Features.Assets.Endpoints;

[Endpoint]
public class RequestReadEndpoint
{
    private readonly IBlobSasService _sas;

    public RequestReadEndpoint(IBlobSasService sas)
    {
        _sas = sas;
    }

    [Route("/api/assets/request-read")]
    [Authorize]
    public static async Task<Results<Ok<RequestReadResponse>, BadRequest<string>, NotFound<string>>> HandleGet(
        [FromServices] RequestReadEndpoint ep,
        [FromQuery] string container,
        [FromQuery] string blobPath,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(container) || string.IsNullOrWhiteSpace(blobPath))
        {
            return TypedResults.BadRequest("Missing container or blob path.");
        }

        // Only allow draft container for security
        if (!string.Equals(container, ep._sas.DraftContainer, StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("Only draft container reads are supported.");
        }

        var blobClient = ep._sas.GetBlobClient(container, blobPath);
        
        // Check if blob exists
        if (!await blobClient.ExistsAsync(ct))
        {
            return TypedResults.NotFound($"Blob not found at path: {blobPath}");
        }

        // Issue GET SAS with 1 hour TTL for preview
        var readUri = await ep._sas.GetReadSasAsync(container, blobPath, TimeSpan.FromHours(1), ct);

        return TypedResults.Ok(new RequestReadResponse
        {
            ReadUrl = readUri.ToString()
        });
    }
}

