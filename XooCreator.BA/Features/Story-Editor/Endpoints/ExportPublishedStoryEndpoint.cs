using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.IO.Compression;
using System.Text;
using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Infrastructure.Services.Blob;
using Microsoft.EntityFrameworkCore;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class ExportPublishedStoryEndpoint
{
    private readonly XooDbContext _db;
    private readonly IAuth0UserService _auth0;
    private readonly IUserContextService _userContext;
    private readonly IBlobSasService _sas;

    public ExportPublishedStoryEndpoint(XooDbContext db, IAuth0UserService auth0, IUserContextService userContext, IBlobSasService sas)
    {
        _db = db;
        _auth0 = auth0;
        _userContext = userContext;
        _sas = sas;
    }

    [Route("/api/{locale}/stories/{storyId}/export")]
    [Authorize]
    public static async Task<Results<FileContentHttpResult, NotFound, UnauthorizedHttpResult, ForbidHttpResult>> HandleGet(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromServices] ExportPublishedStoryEndpoint ep,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        // Admin only export (backup/seeding)
        if (!ep._auth0.HasRole(user, Data.Enums.UserRole.Admin))
        {
            return TypedResults.Forbid();
        }

        var def = await ep._db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .FirstOrDefaultAsync(d => d.StoryId == storyId, ct);
        if (def == null) return TypedResults.NotFound();

        var exportObj = BuildExportJson(def);
        var exportJson = JsonSerializer.Serialize(exportObj, new JsonSerializerOptions { WriteIndented = true });
        var fileName = $"{def.StoryId}-v{def.Version}.zip";

        using var ms = new MemoryStream();
        using (var zip = new ZipArchive(ms, ZipArchiveMode.Create, leaveOpen: true))
        {
            // Add manifest JSON
            var manifestEntry = zip.CreateEntry($"manifest/{def.StoryId}/v{def.Version}/story.json", CompressionLevel.Fastest);
            await using (var writer = new StreamWriter(manifestEntry.Open(), new UTF8Encoding(false)))
            {
                await writer.WriteAsync(exportJson);
            }

            // Collect media paths from definition (already in published layout)
            var mediaPaths = CollectPublishedMediaPaths(def);
            foreach (var path in mediaPaths)
            {
                // Download from published container and add to ZIP preserving relative path under "media/"
                var client = ep._sas.GetBlobClient(ep._sas.PublishedContainer, path);
                var entry = zip.CreateEntry($"media/{path}".Replace('\\', '/'), CompressionLevel.Fastest);
                await using var entryStream = entry.Open();
                var download = await client.DownloadStreamingAsync(cancellationToken: ct);
                await download.Value.Content.CopyToAsync(entryStream, ct);
            }
        }

        var bytes = ms.ToArray();
        return TypedResults.File(bytes, "application/zip", fileName);
    }

    private static object BuildExportJson(StoryDefinition def)
    {
        return new
        {
            id = def.StoryId,
            version = def.Version,
            title = def.Title,
            summary = def.Summary,
            storyType = def.StoryType,
            coverImageUrl = def.CoverImageUrl,
            translations = def.Translations.Select(t => new
            {
                lang = t.LanguageCode,
                title = t.Title
            }).ToList(),
            tiles = def.Tiles
                .OrderBy(t => t.SortOrder)
                .Select(t => new
                {
                    id = t.TileId,
                    type = t.Type,
                    sortOrder = t.SortOrder,
                    imageUrl = t.ImageUrl,
                    videoUrl = t.VideoUrl,
                    audioUrl = t.AudioUrl,
                    translations = t.Translations.Select(tr => new
                    {
                        lang = tr.LanguageCode,
                        caption = tr.Caption,
                        text = tr.Text,
                        question = tr.Question
                    }).ToList(),
                    answers = (t.Answers ?? new()).OrderBy(a => a.SortOrder).Select(a => new
                    {
                        id = a.AnswerId,
                        sortOrder = a.SortOrder,
                        tokens = (a.Tokens ?? new()).Select(tok => new { type = tok.Type, value = tok.Value, quantity = tok.Quantity })
                    }).ToList()
                }).ToList()
        };
    }

    private static List<string> CollectPublishedMediaPaths(StoryDefinition def)
    {
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(def.CoverImageUrl)) result.Add(Normalize(def.CoverImageUrl));
        foreach (var t in def.Tiles)
        {
            if (!string.IsNullOrWhiteSpace(t.ImageUrl)) result.Add(Normalize(t.ImageUrl));
            if (!string.IsNullOrWhiteSpace(t.VideoUrl)) result.Add(Normalize(t.VideoUrl));
            if (!string.IsNullOrWhiteSpace(t.AudioUrl)) result.Add(Normalize(t.AudioUrl));
        }
        return result.ToList();
    }

    private static string Normalize(string path)
    {
        return path.TrimStart('/').Replace('\\', '/');
    }
}


