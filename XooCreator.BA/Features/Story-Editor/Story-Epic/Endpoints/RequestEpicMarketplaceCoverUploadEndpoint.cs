using System.IO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.Assets.DTOs;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class RequestEpicMarketplaceCoverUploadEndpoint
{
    private readonly IBlobSasService _sas;
    private readonly XooDbContext _context;
    private readonly IAuth0UserService _auth0;
    private readonly IConfiguration _config;

    public RequestEpicMarketplaceCoverUploadEndpoint(
        IBlobSasService sas,
        XooDbContext context,
        IAuth0UserService auth0,
        IConfiguration config)
    {
        _sas = sas;
        _context = context;
        _auth0 = auth0;
        _config = config;
    }

    public record EpicMarketplaceCoverUploadRequest
    {
        public required string FileName { get; init; }
        public long ExpectedSize { get; init; }
        public string? ContentType { get; init; }
    }

    [Route("/api/story-editor/epics/{epicId}/marketplace-cover/request-upload")]
    [Authorize]
    public static async Task<Results<
        Ok<RequestUploadResponse>,
        BadRequest<string>,
        NotFound,
        UnauthorizedHttpResult,
        ForbidHttpResult>> HandlePost(
            [FromRoute] string epicId,
            [FromBody] EpicMarketplaceCoverUploadRequest request,
            [FromServices] RequestEpicMarketplaceCoverUploadEndpoint ep,
            CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(epicId))
        {
            return TypedResults.BadRequest("Epic ID is required.");
        }

        var craft = await ep._context.StoryEpicCrafts
            .FirstOrDefaultAsync(c => c.Id == epicId, ct);

        Guid? ownerUserId = null;
        if (craft != null)
        {
            ownerUserId = craft.OwnerUserId;
        }
        else
        {
            var definition = await ep._context.StoryEpicDefinitions
                .FirstOrDefaultAsync(d => d.Id == epicId, ct);
            if (definition != null)
            {
                ownerUserId = definition.OwnerUserId;
            }
        }

        if (ownerUserId == null)
        {
            return TypedResults.NotFound();
        }

        var isOwner = ownerUserId.Value == user.Id;
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
        var blobPath = $"epics/{emailEsc}/{epicId}/marketplace-cover/{fileName}";

        var putUri = await ep._sas.GetPutSasAsync(ep._sas.DraftContainer, blobPath, contentType, TimeSpan.FromMinutes(10), ct);
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
