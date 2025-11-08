using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Features.Assets.DTOs;

namespace XooCreator.BA.Features.Assets.Endpoints;

[Endpoint]
public class CommitUploadEndpoint
{
    private readonly IBlobSasService _sas;

    public CommitUploadEndpoint(IBlobSasService sas)
    {
        _sas = sas;
    }

    [Route("/api/assets/commit")]
    [Authorize]
    public static async Task<Results<Ok<CommitUploadResponse>, BadRequest<string>>> HandlePost(
        [FromServices] CommitUploadEndpoint ep,
        [FromBody] CommitUploadDto dto,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(dto.Container) || string.IsNullOrWhiteSpace(dto.BlobPath))
        {
            return TypedResults.BadRequest("Missing container or blob path.");
        }

        if (!string.Equals(dto.Container, ep._sas.DraftContainer, StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("Only draft container commits are supported.");
        }

        var blobClient = ep._sas.GetBlobClient(dto.Container, dto.BlobPath);
        var props = await blobClient.GetPropertiesAsync(cancellationToken: ct);

        var size = props.Value.ContentLength;
        // Optionally validate dto.Size matches 'size'

        return TypedResults.Ok(new CommitUploadResponse
        {
            Ok = true,
            Bytes = size,
            ContentType = props.Value.ContentType,
            ETag = props.Value.ETag.ToString()
        });
    }
}


