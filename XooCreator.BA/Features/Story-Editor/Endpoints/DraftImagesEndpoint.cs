using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

/// <summary>
/// Request SAS URLs to upload images directly to draft (no job, no tile assignment).
/// Client uploads to blob; mapping to tiles is done later in the UI.
/// </summary>
[Endpoint]
public class DraftImagesEndpoint
{
    private const int MaxBatchFiles = 100;
    private static readonly HashSet<string> AllowedImageExtensions = new(StringComparer.OrdinalIgnoreCase) { ".png", ".jpg", ".jpeg", ".webp" };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly IStoryCraftsRepository _crafts;
    private readonly ILogger<DraftImagesEndpoint> _logger;
    private readonly IConfiguration _config;
    private readonly int _sasValidityMinutes;

    private const int DefaultSasValidityMinutes = 60;

    public DraftImagesEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IBlobSasService sas,
        IStoryCraftsRepository crafts,
        ILogger<DraftImagesEndpoint> logger,
        IConfiguration config)
    {
        _db = db;
        _auth0 = auth0;
        _sas = sas;
        _crafts = crafts;
        _logger = logger;
        _config = config;
        _sasValidityMinutes = config.GetValue<int?>("StoryEditor:DirectUpload:SasValidityMinutes") ?? DefaultSasValidityMinutes;
    }

    public record DraftImageFileRequest(string? ClientFileId, string FileName, long? ExpectedSize, string? ContentType);
    public record DraftImagesBatchRequest(List<DraftImageFileRequest> Files);
    public record DraftImagePutUrlResponse(string ClientFileId, string PutUrl, string BlobPath, string FileName);
    public record DraftImagesBatchResponse(List<DraftImagePutUrlResponse> PutUrls);
    public record DraftImagesErrorResponse(List<string> Errors);

    [Route("/api/{locale}/stories/{storyId}/draft-images/request-upload-batch")]
    [Authorize]
    public static async Task<Results<Ok<DraftImagesBatchResponse>, BadRequest<DraftImagesErrorResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] DraftImagesBatchRequest body,
        [FromServices] DraftImagesEndpoint ep,
        [FromServices] IDirectUploadRateLimitService rateLimit,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        if (!rateLimit.TryAcquire(user.Id, ct))
            return TypedResults.Problem("Too many request-upload requests. Try again later.", statusCode: 429);

        if (!ep._auth0.HasRole(user, UserRole.Admin))
        {
            ep._logger.LogWarning("Draft images request-upload-batch forbidden: userId={UserId} storyId={StoryId} not admin", user.Id, storyId);
            return TypedResults.Forbid();
        }

        var files = body?.Files ?? new List<DraftImageFileRequest>();
        if (files.Count == 0)
        {
            return TypedResults.BadRequest(new DraftImagesErrorResponse(new List<string> { "At least one file is required." }));
        }
        if (files.Count > MaxBatchFiles)
        {
            return TypedResults.BadRequest(new DraftImagesErrorResponse(new List<string> { $"Too many files. Maximum is {MaxBatchFiles}." }));
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
            return TypedResults.NotFound();

        var pageOrQuizTypes = new[] { "page", "quiz", "dialog" };
        var pageTiles = craft.Tiles
            .Where(t => pageOrQuizTypes.Contains(t.Type, StringComparer.OrdinalIgnoreCase))
            .OrderBy(t => t.SortOrder)
            .ToList();
        if (pageTiles.Count == 0)
        {
            return TypedResults.BadRequest(new DraftImagesErrorResponse(new List<string> { "Story has no page, quiz, or dialog tiles." }));
        }

        long maxImageBytes = ep._config.GetValue<long?>("Uploads:MaxImageBytes") ?? 10 * 1024 * 1024;

        string emailToUse = user.Email ?? string.Empty;
        if (craft.OwnerUserId != Guid.Empty && craft.OwnerUserId != user.Id)
        {
            var ownerEmail = await ep._db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Id == craft.OwnerUserId)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct);
            if (!string.IsNullOrWhiteSpace(ownerEmail))
                emailToUse = ownerEmail;
        }

        var usedNames = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var putUrls = new List<DraftImagePutUrlResponse>();

        foreach (var file in files)
        {
            if (string.IsNullOrWhiteSpace(file?.FileName))
                continue;

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedImageExtensions.Contains(ext))
                continue;

            if (file.ExpectedSize.HasValue && file.ExpectedSize.Value > maxImageBytes)
                continue;

            var baseName = Path.GetFileName(file.FileName);
            var finalFileName = baseName;
            var suffix = 0;
            while (usedNames.Contains(finalFileName))
            {
                suffix++;
                var nameWithoutExt = Path.GetFileNameWithoutExtension(baseName);
                finalFileName = $"{nameWithoutExt}_{suffix}{ext}";
            }
            usedNames.Add(finalFileName);

            var asset = new StoryAssetPathMapper.AssetInfo(finalFileName, StoryAssetPathMapper.AssetType.Image, null);
            var blobPath = StoryAssetPathMapper.BuildDraftPath(asset, emailToUse, storyId);

            var contentType = ext.ToLowerInvariant() switch
            {
                ".png" => "image/png",
                ".jpg" or ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                _ => file.ContentType ?? "application/octet-stream"
            };

            var putUrl = await ep._sas.GetPutSasAsync(
                ep._sas.DraftContainer,
                blobPath,
                contentType,
                TimeSpan.FromMinutes(ep._sasValidityMinutes),
                ct);

            var clientFileId = !string.IsNullOrWhiteSpace(file.ClientFileId) ? file.ClientFileId : Guid.NewGuid().ToString("N");
            putUrls.Add(new DraftImagePutUrlResponse(clientFileId, putUrl.ToString(), blobPath, finalFileName));
        }

        return TypedResults.Ok(new DraftImagesBatchResponse(putUrls));
    }
}
