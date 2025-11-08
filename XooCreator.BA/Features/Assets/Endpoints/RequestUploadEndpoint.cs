using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Features.Assets.DTOs;

namespace XooCreator.BA.Features.Assets.Endpoints;

[Endpoint]
public class RequestUploadEndpoint
{
    private readonly IBlobSasService _sas;
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;

    public RequestUploadEndpoint(IBlobSasService sas, IAuth0UserService auth0, IConfiguration config)
    {
        _sas = sas;
        _auth0 = auth0;
        _config = config;
    }

    [Route("/api/assets/request-upload")]
    [Authorize]
    public static async Task<Results<Ok<RequestUploadResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromServices] RequestUploadEndpoint ep,
        [FromBody] RequestUploadDto dto,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        if (string.IsNullOrWhiteSpace(dto.StoryId) || string.IsNullOrWhiteSpace(dto.Lang) ||
            string.IsNullOrWhiteSpace(dto.Kind) || string.IsNullOrWhiteSpace(dto.FileName))
        {
            return TypedResults.BadRequest("Missing required fields.");
        }

        var contentType = string.IsNullOrWhiteSpace(dto.ContentType) ? "application/octet-stream" : dto.ContentType;
       
        //TODO AI FIX THIS 
        //if (dto.ExpectedSize < 0)
        //    dto.ExpectedSize = 0;

        // Build relative path based on kind
        var relPath = dto.Kind switch
        {
            "cover" => $"cover/{dto.FileName}",
            "tile-image" => string.IsNullOrWhiteSpace(dto.TileId) ? $"tiles/{dto.FileName}" : $"tiles/{dto.TileId}/{dto.FileName}",
            "tile-audio" => $"audio/{dto.FileName}",
            "video" => $"video/{dto.FileName}",
            "meta" => $"meta/{dto.FileName}",
            _ => $"misc/{dto.FileName}"
        };

        // Draft blob path
        var emailEsc = Uri.EscapeDataString(user.Email);
        var blobPath = $"draft/u/{emailEsc}/stories/{dto.StoryId}/{dto.Lang}/{relPath}";

        // Issue PUT SAS
        var putUri = await ep._sas.GetPutSasAsync(ep._sas.DraftContainer, blobPath, contentType!, TimeSpan.FromMinutes(10), ct);

        // Public blob URL (still private container, used only for diagnostics or GET SAS issuance)
        var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, blobPath);

        return TypedResults.Ok(new RequestUploadResponse
        {
            PutUrl = putUri.ToString(),
            BlobUrl = blobClient.Uri.ToString(),
            RelPath = relPath
        });
    }
}

