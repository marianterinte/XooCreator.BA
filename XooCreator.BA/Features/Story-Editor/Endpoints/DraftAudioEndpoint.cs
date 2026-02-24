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
/// Request SAS URLs to upload audio directly to draft (no job, no tile assignment).
/// Client uploads to blob; mapping to tiles is done later in the UI.
/// </summary>
[Endpoint]
public class DraftAudioEndpoint
{
    private const int MaxBatchFiles = 100;
    private static readonly HashSet<string> AllowedAudioExtensions = new(StringComparer.OrdinalIgnoreCase) { ".wav", ".mp3", ".m4a" };

    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly IStoryCraftsRepository _crafts;
    private readonly ILogger<DraftAudioEndpoint> _logger;
    private readonly IConfiguration _config;
    private readonly int _sasValidityMinutes;

    private const int DefaultSasValidityMinutes = 60;

    public DraftAudioEndpoint(
        XooDbContext db,
        IAuth0UserService auth0,
        IBlobSasService sas,
        IStoryCraftsRepository crafts,
        ILogger<DraftAudioEndpoint> logger,
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

    public record DraftAudioFileRequest(string? ClientFileId, string FileName, long? ExpectedSize, string? ContentType);
    public record DraftAudioBatchRequest(List<DraftAudioFileRequest> Files);
    public record DraftAudioPutUrlResponse(string ClientFileId, string PutUrl, string BlobPath, string FileName);
    public record DraftAudioBatchResponse(List<DraftAudioPutUrlResponse> PutUrls);
    public record DraftAudioErrorResponse(List<string> Errors);

    [Route("/api/{locale}/stories/{storyId}/draft-audio/request-upload-batch")]
    [Authorize]
    public static async Task<Results<Ok<DraftAudioBatchResponse>, BadRequest<DraftAudioErrorResponse>, UnauthorizedHttpResult, ForbidHttpResult, NotFound, ProblemHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] DraftAudioBatchRequest body,
        [FromServices] DraftAudioEndpoint ep,
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
            ep._logger.LogWarning("Draft audio request-upload-batch forbidden: userId={UserId} storyId={StoryId} not admin", user.Id, storyId);
            return TypedResults.Forbid();
        }

        var files = body?.Files ?? new List<DraftAudioFileRequest>();
        if (files.Count == 0)
        {
            return TypedResults.BadRequest(new DraftAudioErrorResponse(new List<string> { "At least one file is required." }));
        }
        if (files.Count > MaxBatchFiles)
        {
            return TypedResults.BadRequest(new DraftAudioErrorResponse(new List<string> { $"Too many files. Maximum is {MaxBatchFiles}." }));
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
            return TypedResults.BadRequest(new DraftAudioErrorResponse(new List<string> { "Story has no page, quiz, or dialog tiles." }));
        }

        long maxAudioBytes = ep._config.GetValue<long?>("Uploads:MaxAudioBytes") ?? 100 * 1024 * 1024;

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
        var putUrls = new List<DraftAudioPutUrlResponse>();

        var normalizedLocale = (locale ?? string.Empty).Trim().ToLowerInvariant();
        var langForPath = string.IsNullOrWhiteSpace(normalizedLocale) ? null : normalizedLocale;

        foreach (var file in files)
        {
            if (string.IsNullOrWhiteSpace(file?.FileName))
                continue;

            var ext = Path.GetExtension(file.FileName);
            if (!AllowedAudioExtensions.Contains(ext))
                continue;

            if (file.ExpectedSize.HasValue && file.ExpectedSize.Value > maxAudioBytes)
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

            var asset = new StoryAssetPathMapper.AssetInfo(finalFileName, StoryAssetPathMapper.AssetType.Audio, langForPath);
            var blobPath = StoryAssetPathMapper.BuildDraftPath(asset, emailToUse, storyId);

            var contentType = ext.ToLowerInvariant() switch
            {
                ".wav" => "audio/wav",
                ".mp3" => "audio/mpeg",
                ".m4a" => "audio/mp4",
                _ => file.ContentType ?? "application/octet-stream"
            };

            var putUrl = await ep._sas.GetPutSasAsync(
                ep._sas.DraftContainer,
                blobPath,
                contentType,
                TimeSpan.FromMinutes(ep._sasValidityMinutes),
                ct);

            var clientFileId = !string.IsNullOrWhiteSpace(file.ClientFileId) ? file.ClientFileId : Guid.NewGuid().ToString("N");
            putUrls.Add(new DraftAudioPutUrlResponse(clientFileId, putUrl.ToString(), blobPath, finalFileName));
        }

        return TypedResults.Ok(new DraftAudioBatchResponse(putUrls));
    }
}
