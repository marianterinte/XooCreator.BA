using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class SaveGeneratedImageEndpoint
{
    private readonly IBlobSasService _blobService;
    private readonly IAuth0UserService _auth0;

    public SaveGeneratedImageEndpoint(IBlobSasService blobService, IAuth0UserService auth0)
    {
        _blobService = blobService;
        _auth0 = auth0;
    }

    public record SaveImageRequest(
        string ImageData, // Base64-encoded image
        string MimeType,  // Image MIME type (e.g., "image/png")
        string TileId,
        string StoryId
    );

    public record SaveImageResponse(
        string ImageUrl,
        string BlobPath,
        string FileName
    );

    [Route("/api/{locale}/story-editor/save-image")]
    [Authorize]
    public static async Task<Results<Ok<SaveImageResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] SaveGeneratedImageEndpoint ep,
        [FromBody] SaveImageRequest request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.ImageData))
        {
            return TypedResults.BadRequest("ImageData is required");
        }

        if (string.IsNullOrWhiteSpace(request.MimeType))
        {
            return TypedResults.BadRequest("MimeType is required");
        }

        if (string.IsNullOrWhiteSpace(request.TileId))
        {
            return TypedResults.BadRequest("TileId is required");
        }

        if (string.IsNullOrWhiteSpace(request.StoryId))
        {
            return TypedResults.BadRequest("StoryId is required");
        }

        try
        {
            // Decode base64 image data
            byte[] imageBytes;
            try
            {
                imageBytes = Convert.FromBase64String(request.ImageData);
            }
            catch (FormatException)
            {
                return TypedResults.BadRequest("Invalid base64 image data");
            }

            // Determine file extension from MIME type
            var extension = request.MimeType switch
            {
                "image/png" => "png",
                "image/jpeg" or "image/jpg" => "jpg",
                "image/webp" => "webp",
                "image/gif" => "gif",
                _ => "png" // Default to PNG
            };

            // Generate filename: {tileId}_image.{ext}
            var fileName = $"{request.TileId}_image.{extension}";

            // Build blob path using existing mapper (images don't need language code)
            var assetInfo = new StoryAssetPathMapper.AssetInfo(fileName, StoryAssetPathMapper.AssetType.Image, null);
            var blobPath = StoryAssetPathMapper.BuildDraftPath(assetInfo, user.Email, request.StoryId);

            // Upload to Azure Blob Storage
            var blobClient = ep._blobService.GetBlobClient(ep._blobService.DraftContainer, blobPath);
            
            using (var stream = new MemoryStream(imageBytes))
            {
                await blobClient.UploadAsync(stream, overwrite: true, cancellationToken: ct);
            }

            // Set content type
            var headers = new Azure.Storage.Blobs.Models.BlobHttpHeaders
            {
                ContentType = request.MimeType
            };
            await blobClient.SetHttpHeadersAsync(headers, cancellationToken: ct);

            var imageUrl = blobClient.Uri.ToString();

            return TypedResults.Ok(new SaveImageResponse(
                ImageUrl: imageUrl,
                BlobPath: blobPath,
                FileName: fileName
            ));
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"Failed to save image: {ex.Message}");
        }
    }
}

