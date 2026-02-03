using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using XooCreator.BA.Features.Assets.DTOs;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Services;

namespace XooCreator.BA.Features.Assets.Endpoints;

[Endpoint]
public class RequestUploadEndpoint
{
    private readonly IBlobSasService _sas;
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;
    private readonly IStoryAssetReplacementService _assetReplacementService;

    public RequestUploadEndpoint(
        IBlobSasService sas, 
        IAuth0UserService auth0, 
        IConfiguration config,
        IStoryAssetReplacementService assetReplacementService)
    {
        _sas = sas;
        _auth0 = auth0;
        _config = config;
        _assetReplacementService = assetReplacementService;
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

        // Validări minime (tip/extension/size)
        var ext = Path.GetExtension(dto.FileName).ToLowerInvariant();
        var allowedImage = new[] { ".png", ".jpg", ".jpeg", ".webp" };
        var allowedAudio = new[] { ".mp3", ".m4a", ".wav", ".aac", ".ogg" };
        var allowedVideo = new[] { ".mp4", ".webm" };

        long maxImage = ep._config.GetValue<long?>("Uploads:MaxImageBytes") ?? 10 * 1024 * 1024;
        long maxAudio = ep._config.GetValue<long?>("Uploads:MaxAudioBytes") ?? 50 * 1024 * 1024;
        long maxVideo = ep._config.GetValue<long?>("Uploads:MaxVideoBytes") ?? 200 * 1024 * 1024;

        // Ensure ExpectedSize is non-negative
        var expectedSize = dto.ExpectedSize < 0 ? 0 : dto.ExpectedSize;

        if (string.Equals(dto.Kind, "cover", StringComparison.OrdinalIgnoreCase) ||
            string.Equals(dto.Kind, "tile-image", StringComparison.OrdinalIgnoreCase))
        {
            if (!allowedImage.Contains(ext)) return TypedResults.BadRequest("Unsupported image extension.");
            if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase)) return TypedResults.BadRequest("Invalid image content-type.");
            if (expectedSize > maxImage) return TypedResults.BadRequest("Image too large.");
        }
        else if (string.Equals(dto.Kind, "tile-audio", StringComparison.OrdinalIgnoreCase))
        {
            if (!allowedAudio.Contains(ext)) return TypedResults.BadRequest("Unsupported audio extension.");
            if (!contentType.StartsWith("audio/", StringComparison.OrdinalIgnoreCase)) return TypedResults.BadRequest("Invalid audio content-type.");
            if (expectedSize > maxAudio) return TypedResults.BadRequest("Audio too large.");
        }
        else if (string.Equals(dto.Kind, "video", StringComparison.OrdinalIgnoreCase))
        {
            if (!allowedVideo.Contains(ext)) return TypedResults.BadRequest("Unsupported video extension.");
            if (!contentType.StartsWith("video/", StringComparison.OrdinalIgnoreCase)) return TypedResults.BadRequest("Invalid video content-type.");
            if (expectedSize > maxVideo) return TypedResults.BadRequest("Video too large.");
        }

        var assetType = dto.Kind switch
        {
            "cover" => StoryAssetPathMapper.AssetType.Image,
            "tile-image" => StoryAssetPathMapper.AssetType.Image,
            "tile-audio" => StoryAssetPathMapper.AssetType.Audio,
            "video" => StoryAssetPathMapper.AssetType.Video,
            _ => StoryAssetPathMapper.AssetType.Image
        };

        var lang = assetType == StoryAssetPathMapper.AssetType.Audio || assetType == StoryAssetPathMapper.AssetType.Video
            ? dto.Lang
            : null;
        
        // Determine which email to use for asset path:
        // - If user is admin AND OwnerEmail is provided, use OwnerEmail (admin editing another user's draft)
        // - Otherwise, use current user's email (normal behavior)
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var emailToUse = (isAdmin && !string.IsNullOrWhiteSpace(dto.OwnerEmail))
            ? dto.OwnerEmail!.Trim()
            : user.Email;
        
        // Delete old asset if it exists and update DB with new filename
        await ep._assetReplacementService.ReplaceAssetAsync(
            emailToUse, 
            dto.StoryId, 
            dto.TileId, 
            dto.Kind, 
            dto.FileName, 
            lang, 
            ct);
        
        var asset = new StoryAssetPathMapper.AssetInfo(dto.FileName, assetType, lang);
        var blobPath = StoryAssetPathMapper.BuildDraftPath(asset, emailToUse, dto.StoryId);

        var putUri = await ep._sas.GetPutSasAsync(ep._sas.DraftContainer, blobPath, contentType!, TimeSpan.FromMinutes(10), ct);
        var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, blobPath);

        return TypedResults.Ok(new RequestUploadResponse
        {
            PutUrl = putUri.ToString(),
            BlobUrl = blobClient.Uri.ToString(),
            RelPath = dto.FileName,
            Container = ep._sas.DraftContainer,
            BlobPath = blobPath
        });
    }
}

