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
public class RequestEpicHeroImageUploadEndpoint
{
    private readonly IBlobSasService _sas;
    private readonly IEpicHeroRepository _repository;
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;

    public RequestEpicHeroImageUploadEndpoint(
        IBlobSasService sas,
        IEpicHeroRepository repository,
        IAuth0UserService auth0,
        IConfiguration config)
    {
        _sas = sas;
        _repository = repository;
        _auth0 = auth0;
        _config = config;
    }

    public record EpicHeroImageUploadRequest
    {
        public required string FileName { get; init; }
        public long ExpectedSize { get; init; }
        public string? ContentType { get; init; }
    }

    [Route("/api/story-editor/heroes/{heroId}/assets/request-upload")]
    [Authorize]
    public static async Task<Results<
        Ok<RequestUploadResponse>,
        BadRequest<string>,
        NotFound,
        UnauthorizedHttpResult,
        ForbidHttpResult>> HandlePost(
            [FromRoute] string heroId,
            [FromBody] EpicHeroImageUploadRequest request,
            [FromServices] RequestEpicHeroImageUploadEndpoint ep,
            CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        heroId = (heroId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(heroId))
        {
            return TypedResults.BadRequest("Hero ID is required.");
        }

        var heroCraft = await ep._repository.GetCraftAsync(heroId, ct);
        if (heroCraft == null)
        {
            return TypedResults.NotFound();
        }

        var isOwner = heroCraft.OwnerUserId == user.Id;
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
        
        // Path format: heroes/{ownerEmail}/{heroId}/{filename}
        var blobPath = $"heroes/{emailEsc}/{heroId}/{fileName}";
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

