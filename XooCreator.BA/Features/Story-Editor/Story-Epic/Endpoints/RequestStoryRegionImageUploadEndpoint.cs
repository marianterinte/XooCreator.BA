using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Assets.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Repositories;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using Microsoft.Extensions.Configuration;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class RequestStoryRegionImageUploadEndpoint
{
    private readonly IBlobSasService _sas;
    private readonly IStoryRegionRepository _repository;
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;

    public RequestStoryRegionImageUploadEndpoint(
        IBlobSasService sas,
        IStoryRegionRepository repository,
        IAuth0UserService auth0,
        IConfiguration config)
    {
        _sas = sas;
        _repository = repository;
        _auth0 = auth0;
        _config = config;
    }

    public record StoryRegionImageUploadRequest
    {
        public required string FileName { get; init; }
        public long ExpectedSize { get; init; }
        public string? ContentType { get; init; }
    }

    [Route("/api/story-editor/regions/{regionId}/assets/request-upload")]
    [Authorize]
    public static async Task<Results<
        Ok<RequestUploadResponse>,
        BadRequest<string>,
        NotFound,
        UnauthorizedHttpResult,
        ForbidHttpResult>> HandlePost(
            [FromRoute] string regionId,
            [FromBody] StoryRegionImageUploadRequest request,
            [FromServices] RequestStoryRegionImageUploadEndpoint ep,
            CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        regionId = (regionId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(regionId))
        {
            return TypedResults.BadRequest("Region ID is required.");
        }

        var region = await ep._repository.GetAsync(regionId, ct);
        if (region == null)
        {
            return TypedResults.NotFound();
        }

        var isOwner = region.OwnerUserId == user.Id;
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isOwner && !isAdmin)
        {
            return TypedResults.Forbid();
        }

        var fileName = Path.GetFileName(request.FileName ?? string.Empty);
        if (string.IsNullOrWhiteSpace(fileName))
        {
            return TypedResults.BadRequest("File name is required.");
        }

        var ext = Path.GetExtension(fileName).ToLowerInvariant();
        var allowed = new[] { ".png", ".jpg", ".jpeg", ".webp" };
        if (!allowed.Contains(ext))
        {
            return TypedResults.BadRequest("Unsupported image extension.");
        }

        var maxImageBytes = ep._config.GetValue<long?>("Uploads:MaxImageBytes") ?? 10 * 1024 * 1024;
        if (request.ExpectedSize <= 0 || request.ExpectedSize > maxImageBytes)
        {
            return TypedResults.BadRequest("Image too large.");
        }

        var contentType = string.IsNullOrWhiteSpace(request.ContentType)
            ? "application/octet-stream"
            : request.ContentType;
        if (!contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("Invalid image content type.");
        }

        var email = user.Email;
        if (string.IsNullOrWhiteSpace(email))
        {
            return TypedResults.BadRequest("User email is required.");
        }

        var emailEsc = Uri.EscapeDataString(email.Trim());
        
        // Path format: regions/{ownerEmail}/{regionId}/{filename}
        var blobPath = $"regions/{emailEsc}/{regionId}/{fileName}";
        var putUri = await ep._sas.GetPutSasAsync(ep._sas.DraftContainer, blobPath, contentType, TimeSpan.FromMinutes(15), ct);
        var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, blobPath);

        return TypedResults.Ok(new RequestUploadResponse
        {
            PutUrl = putUri.ToString(),
            BlobUrl = blobClient.Uri.ToString(),
            RelPath = fileName,
            Container = ep._sas.DraftContainer,
            BlobPath = blobPath
        });
    }
}

