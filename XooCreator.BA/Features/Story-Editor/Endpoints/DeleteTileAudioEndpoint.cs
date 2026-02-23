using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class DeleteTileAudioEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IAuth0UserService _auth0;
    private readonly IBlobSasService _sas;
    private readonly XooDbContext _db;

    public DeleteTileAudioEndpoint(
        IStoryCraftsRepository crafts,
        IAuth0UserService auth0,
        IBlobSasService sas,
        XooDbContext db)
    {
        _crafts = crafts;
        _auth0 = auth0;
        _sas = sas;
        _db = db;
    }

    public sealed record DeleteTileAudioResponse(
        bool Success,
        bool Removed,
        string TileId,
        string LanguageCode,
        string? FileName);

    [Route("/api/stories/{storyId}/tiles/{tileId}/audio")]
    [Authorize]
    public static async Task<Results<Ok<DeleteTileAudioResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleDelete(
        [FromRoute] string storyId,
        [FromRoute] string tileId,
        [FromQuery] string? lang,
        [FromServices] DeleteTileAudioEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var locale = (lang ?? string.Empty).Trim().ToLowerInvariant();
        if (string.IsNullOrWhiteSpace(locale))
        {
            return TypedResults.BadRequest("Query parameter 'lang' is required.");
        }

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null) return TypedResults.NotFound();

        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isAdmin && !ep._auth0.HasRole(user, UserRole.Creator))
        {
            return TypedResults.Forbid();
        }

        if (craft.OwnerUserId != user.Id && !isAdmin)
        {
            return TypedResults.Forbid();
        }

        var status = (craft.Status ?? "draft").ToLowerInvariant();
        if (status is "sent_for_approval" or "in_review" or "approved" or "published")
        {
            return TypedResults.Forbid();
        }

        var tile = craft.Tiles.FirstOrDefault(t => string.Equals(t.TileId, tileId, StringComparison.OrdinalIgnoreCase));
        if (tile == null) return TypedResults.NotFound();

        var translation = tile.Translations.FirstOrDefault(t =>
            !string.IsNullOrWhiteSpace(t.LanguageCode) &&
            t.LanguageCode.Equals(locale, StringComparison.OrdinalIgnoreCase));

        var existingAudio = translation?.AudioUrl?.Trim();
        if (string.IsNullOrWhiteSpace(existingAudio))
        {
            return TypedResults.Ok(new DeleteTileAudioResponse(true, false, tile.TileId, locale, null));
        }

        var ownerEmail = await ep._db.Set<AlchimaliaUser>()
            .AsNoTracking()
            .Where(u => u.Id == craft.OwnerUserId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync(ct);

        if (!string.IsNullOrWhiteSpace(ownerEmail))
        {
            var asset = new StoryAssetPathMapper.AssetInfo(existingAudio, StoryAssetPathMapper.AssetType.Audio, locale);
            var blobPath = StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, craft.StoryId);
            var blobClient = ep._sas.GetBlobClient(ep._sas.DraftContainer, blobPath);
            await blobClient.DeleteIfExistsAsync(cancellationToken: ct);
        }

        translation!.AudioUrl = null;
        tile.UpdatedAt = DateTime.UtcNow;
        craft.UpdatedAt = DateTime.UtcNow;
        await ep._db.SaveChangesAsync(ct);

        return TypedResults.Ok(new DeleteTileAudioResponse(true, true, tile.TileId, locale, existingAudio));
    }
}
