using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text.Json;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.StoryEpic.DTOs;
using XooCreator.BA.Features.StoryEditor.StoryEpic.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Endpoints;

[Endpoint]
public class PreviewEpicImportEndpoint
{
    private readonly IEpicImportService _importService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<PreviewEpicImportEndpoint> _logger;
    private const long MaxZipSizeBytes = 500 * 1024 * 1024; // 500MB

    public PreviewEpicImportEndpoint(
        IEpicImportService importService,
        IAuth0UserService auth0,
        ILogger<PreviewEpicImportEndpoint> logger)
    {
        _importService = importService;
        _auth0 = auth0;
        _logger = logger;
    }

    [Route("/api/story-editor/epics/import-preview")]
    [Authorize]
    [RequestSizeLimit(MaxZipSizeBytes)]
    public static async Task<Results<Ok<EpicImportPreviewResponse>, BadRequest<string>, UnauthorizedHttpResult, ForbidHttpResult>> HandlePost(
        [FromServices] PreviewEpicImportEndpoint ep,
        HttpRequest request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);

        if (!isAdmin && !isCreator)
        {
            return TypedResults.Forbid();
        }

        if (!request.HasFormContentType)
        {
            return TypedResults.BadRequest("Request must be multipart/form-data");
        }

        var form = await request.ReadFormAsync(ct);
        var file = form.Files.GetFile("file");
        if (file == null || file.Length == 0)
        {
            return TypedResults.BadRequest("No file provided");
        }

        if (file.Length > MaxZipSizeBytes)
        {
            return TypedResults.BadRequest($"File size exceeds maximum allowed size of {MaxZipSizeBytes / (1024 * 1024)}MB");
        }

        if (!file.FileName.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
        {
            return TypedResults.BadRequest("File must be a ZIP archive");
        }

        try
        {
            await using var zipStream = file.OpenReadStream();
            
            // Preview import (this will throw NotImplementedException until EpicImportService is implemented)
            var preview = await ep._importService.PreviewImportAsync(zipStream, user.Id, ct);

            return TypedResults.Ok(preview);
        }
        catch (NotImplementedException)
        {
            return TypedResults.BadRequest("Preview functionality is not yet implemented. Please use import-full endpoint instead.");
        }
        catch (Exception ex)
        {
            ep._logger.LogError(ex, "Failed to preview epic import: fileName={FileName}", file.FileName);
            return TypedResults.BadRequest($"Failed to preview import: {ex.Message}");
        }
    }
}

