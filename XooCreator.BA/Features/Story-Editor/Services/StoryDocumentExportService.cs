using System.Globalization;
using System.IO;
using System.Linq;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using Microsoft.EntityFrameworkCore;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.StoryEditor.Mappers;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Generates printable documents (PDF/DOCX) for a story craft/definition.
/// Note: "IncludeQuizAnswers" is expected to be enforced at endpoint level (owner/admin only).
/// </summary>
public class StoryDocumentExportService : IStoryDocumentExportService
{
    private readonly XooDbContext _db;
    private readonly IStoryCraftsRepository _crafts;
    private readonly IBlobSasService _sas;
    private readonly ILogger<StoryDocumentExportService> _logger;

    static StoryDocumentExportService()
    {
        // Required by QuestPDF 2024+.
        // If you need a different license type, update accordingly.
        QuestPDF.Settings.License = LicenseType.Community;
    }

    public StoryDocumentExportService(
        XooDbContext db,
        IStoryCraftsRepository crafts,
        IBlobSasService sas,
        ILogger<StoryDocumentExportService> logger)
    {
        _db = db;
        _crafts = crafts;
        _sas = sas;
        _logger = logger;
    }

    public async Task<DocumentExportResult> ExportAsync(StoryDocumentExportJob job, CancellationToken ct = default)
    {
        var locale = NormalizeLocale(job.Locale);
        var format = (job.Format ?? StoryDocumentExportFormat.Pdf).Trim().ToLowerInvariant();

        var model = job.IsDraft
            ? await LoadDraftModelAsync(job.StoryId, locale, ct)
            : await LoadPublishedModelAsync(job.StoryId, locale, ct);

        // Apply job options
        model = model with
        {
            IncludeCover = job.IncludeCover,
            IncludeQuizAnswers = job.IncludeQuizAnswers,
            PaperSize = job.PaperSize,
            UseMobileImageLayout = job.UseMobileImageLayout
        };

        return format switch
        {
            StoryDocumentExportFormat.Pdf => await RenderPdfAsync(model, ct),
            StoryDocumentExportFormat.Docx => await RenderDocxAsync(model, ct),
            _ => throw new InvalidOperationException($"Unsupported format: {format}")
        };
    }

    private static string NormalizeLocale(string? locale)
    {
        return (locale ?? "ro-ro").Trim().ToLowerInvariant();
    }

    private sealed record StoryDocModel(
        string StoryId,
        string Locale,
        string Title,
        int? Version,
        Guid? StoryOwnerUserId,
        string? StoryOwnerEmail,
        bool IsDraft,
        bool IncludeCover,
        bool IncludeQuizAnswers,
        string PaperSize,
        bool UseMobileImageLayout,
        string? CoverImageBlobPath,
        List<StoryDocTile> Tiles);

    private sealed record StoryDocTile(
        int Index,
        string Type,
        string? Caption,
        string? Text,
        string? Question,
        string? VideoUrl,
        string? ImageBlobPath,
        List<StoryDocAnswer> Answers);

    private sealed record StoryDocAnswer(string Text, bool IsCorrect);

    private async Task<StoryDocModel> LoadPublishedModelAsync(string storyId, string locale, CancellationToken ct)
    {
        var def = await _db.StoryDefinitions
            .Include(d => d.Tiles).ThenInclude(t => t.Answers).ThenInclude(a => a.Tokens)
            .Include(d => d.Tiles).ThenInclude(t => t.Translations)
            .Include(d => d.Translations)
            .FirstOrDefaultAsync(d => d.StoryId == storyId && d.IsActive, ct);

        if (def == null)
            throw new InvalidOperationException($"StoryDefinition not found: {storyId}");

        var ownerUserId = def.CreatedBy;
        string? ownerEmail = null;
        if (ownerUserId.HasValue)
        {
            ownerEmail = await _db.AlchimaliaUsers
                .AsNoTracking()
                .Where(u => u.Id == ownerUserId.Value)
                .Select(u => u.Email)
                .FirstOrDefaultAsync(ct);
        }

        var title = def.Translations.FirstOrDefault(t => t.LanguageCode == locale)?.Title
                    ?? def.Translations.FirstOrDefault()?.Title
                    ?? def.Title
                    ?? storyId;

        // cover and tile image urls in published are typically full published paths. Fallback to build path if just filename.
        var coverBlobPath = ResolvePublishedImageBlobPath(def.CoverImageUrl, ownerEmail, storyId);

        var tiles = def.Tiles
            .OrderBy(t => t.SortOrder)
            .Select((t, i) =>
            {
                var tr = t.Translations.FirstOrDefault(x => x.LanguageCode == locale)
                         ?? t.Translations.FirstOrDefault();

                var imgBlobPath = ResolvePublishedImageBlobPath(t.ImageUrl, ownerEmail, storyId);
                var answers = (t.Answers ?? new List<StoryAnswer>())
                    .OrderBy(a => a.SortOrder)
                    .Select(a => new StoryDocAnswer(a.Text ?? string.Empty, a.IsCorrect))
                    .ToList();

                return new StoryDocTile(
                    Index: i + 1,
                    Type: t.Type ?? "page",
                    Caption: tr?.Caption,
                    Text: tr?.Text,
                    Question: tr?.Question,
                    VideoUrl: tr?.VideoUrl,
                    ImageBlobPath: imgBlobPath,
                    Answers: answers
                );
            })
            .ToList();

        return new StoryDocModel(
            StoryId: storyId,
            Locale: locale,
            Title: title,
            Version: def.Version,
            StoryOwnerUserId: ownerUserId,
            StoryOwnerEmail: ownerEmail,
            IsDraft: false,
            IncludeCover: true,
            IncludeQuizAnswers: false,
            PaperSize: "A4",
            UseMobileImageLayout: true,
            CoverImageBlobPath: coverBlobPath,
            Tiles: tiles
        );
    }

    private async Task<StoryDocModel> LoadDraftModelAsync(string storyId, string locale, CancellationToken ct)
    {
        var craft = await _crafts.GetAsync(storyId, ct);
        if (craft == null)
            throw new InvalidOperationException($"StoryCraft not found: {storyId}");

        var ownerEmail = await _db.AlchimaliaUsers
            .AsNoTracking()
            .Where(u => u.Id == craft.OwnerUserId)
            .Select(u => u.Email)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrWhiteSpace(ownerEmail))
            throw new InvalidOperationException($"Owner email not found for userId: {craft.OwnerUserId}");

        var title = craft.Translations.FirstOrDefault(t => t.LanguageCode == locale)?.Title
                    ?? craft.Translations.FirstOrDefault()?.Title
                    ?? storyId;

        var coverBlobPath = ResolveDraftImageBlobPath(craft.CoverImageUrl, ownerEmail, storyId);

        var tiles = craft.Tiles
            .OrderBy(t => t.SortOrder)
            .Select((t, i) =>
            {
                var tr = t.Translations.FirstOrDefault(x => x.LanguageCode == locale)
                         ?? t.Translations.FirstOrDefault();

                var imgBlobPath = ResolveDraftImageBlobPath(t.ImageUrl, ownerEmail, storyId);
                var answers = (t.Answers ?? new List<StoryCraftAnswer>())
                    .OrderBy(a => a.SortOrder)
                    .Select(a =>
                    {
                        // Try translation first for draft answers.
                        var at = (a.Translations ?? new List<StoryCraftAnswerTranslation>())
                            .FirstOrDefault(x => x.LanguageCode == locale)
                                 ?? (a.Translations ?? new List<StoryCraftAnswerTranslation>()).FirstOrDefault();
                        var text = at?.Text ?? a.AnswerId ?? string.Empty;
                        return new StoryDocAnswer(text, a.IsCorrect);
                    })
                    .ToList();

                return new StoryDocTile(
                    Index: i + 1,
                    Type: t.Type ?? "page",
                    Caption: tr?.Caption,
                    Text: tr?.Text,
                    Question: tr?.Question,
                    VideoUrl: tr?.VideoUrl,
                    ImageBlobPath: imgBlobPath,
                    Answers: answers
                );
            })
            .ToList();

        return new StoryDocModel(
            StoryId: storyId,
            Locale: locale,
            Title: title,
            Version: null,
            StoryOwnerUserId: craft.OwnerUserId,
            StoryOwnerEmail: ownerEmail,
            IsDraft: true,
            IncludeCover: true,
            IncludeQuizAnswers: false,
            PaperSize: "A4",
            UseMobileImageLayout: true,
            CoverImageBlobPath: coverBlobPath,
            Tiles: tiles
        );
    }

    private static string? ResolveDraftImageBlobPath(string? stored, string ownerEmail, string storyId)
    {
        if (string.IsNullOrWhiteSpace(stored)) return null;
        var trimmed = stored.Trim().TrimStart('/');

        // If already a blob path, keep it.
        if (trimmed.StartsWith("draft/", StringComparison.OrdinalIgnoreCase))
            return trimmed;

        // Otherwise treat it as a filename stored in DB.
        var asset = new StoryAssetPathMapper.AssetInfo(trimmed, StoryAssetPathMapper.AssetType.Image, null);
        return StoryAssetPathMapper.BuildDraftPath(asset, ownerEmail, storyId);
    }

    private static string? ResolvePublishedImageBlobPath(string? stored, string? ownerEmail, string storyId)
    {
        if (string.IsNullOrWhiteSpace(stored)) return null;
        var trimmed = stored.Trim().TrimStart('/');

        // Most published assets are already stored as full published paths like "images/..."
        if (trimmed.Contains('/'))
            return trimmed;

        // Fallback: treat as filename and build published path if we have ownerEmail.
        if (string.IsNullOrWhiteSpace(ownerEmail))
            return trimmed;

        var ownerFolder = StoryAssetPathMapper.SanitizeEmailForFolder(ownerEmail);
        var asset = new StoryAssetPathMapper.AssetInfo(trimmed, StoryAssetPathMapper.AssetType.Image, null);
        return StoryAssetPathMapper.BuildPublishedPath(asset, ownerFolder, storyId);
    }

    private async Task<byte[]?> TryDownloadImageAsync(string container, string blobPath, CancellationToken ct)
    {
        try
        {
            var client = _sas.GetBlobClient(container, blobPath);
            if (!await client.ExistsAsync(ct))
            {
                _logger.LogWarning("Image blob missing: container={Container} path={Path}", container, blobPath);
                return null;
            }

            var download = await client.DownloadStreamingAsync(cancellationToken: ct);
            await using var ms = new MemoryStream();
            await download.Value.Content.CopyToAsync(ms, ct);
            return ms.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to download image: container={Container} path={Path}", container, blobPath);
            return null;
        }
    }

    private async Task<DocumentExportResult> RenderPdfAsync(StoryDocModel model, CancellationToken ct)
    {
        var fileName = model.IsDraft
            ? $"{model.StoryId}-draft-{model.Locale}.pdf"
            : $"{model.StoryId}-v{model.Version ?? 1}-{model.Locale}.pdf";

        var container = model.IsDraft ? _sas.DraftContainer : _sas.PublishedContainer;

        var coverBytes = model.IncludeCover && !string.IsNullOrWhiteSpace(model.CoverImageBlobPath)
            ? await TryDownloadImageAsync(container, model.CoverImageBlobPath!, ct)
            : null;

        // Pre-fetch tile image bytes (best-effort). This keeps rendering simple and deterministic.
        var tileImages = new Dictionary<int, byte[]>();
        foreach (var tile in model.Tiles)
        {
            if (string.IsNullOrWhiteSpace(tile.ImageBlobPath)) continue;
            var img = await TryDownloadImageAsync(container, tile.ImageBlobPath!, ct);
            if (img != null) tileImages[tile.Index] = img;
        }

        var paper = (model.PaperSize ?? "A4").Trim().ToUpperInvariant();
        var pageSize = paper == "LETTER" ? PageSizes.Letter : PageSizes.A4;

        var doc = QuestPDF.Fluent.Document.Create(container =>
        {
            if (model.IncludeCover)
            {
                container.Page(page =>
                {
                    page.Size(pageSize);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(14));
                    page.Content().Column(col =>
                    {
                        col.Spacing(10);
                        col.Item().Text(model.Title).SemiBold().FontSize(22);
                        col.Item().Text($"StoryId: {model.StoryId}").FontSize(10).FontColor(Colors.Grey.Darken1);
                        col.Item().Text($"Language: {model.Locale}").FontSize(10).FontColor(Colors.Grey.Darken1);

                        if (coverBytes != null)
                        {
                            col.Item().Image(coverBytes).FitWidth();
                        }
                        else if (!string.IsNullOrWhiteSpace(model.CoverImageBlobPath))
                        {
                            col.Item().Text("(Cover image missing)").FontSize(10).FontColor(Colors.Grey.Darken2);
                        }
                    });
                });
            }

            foreach (var tile in model.Tiles)
            {
                container.Page(page =>
                {
                    page.Size(pageSize);
                    page.Margin(30);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));
                    page.Content().Column(col =>
                    {
                        col.Spacing(6);

                        // Image first (book-like layout) so text sits beneath it.
                        if (tileImages.TryGetValue(tile.Index, out var imgBytes))
                        {
                            if (model.UseMobileImageLayout)
                            {
                                // Center image at ~60% width (6/10), leaving space for text like in mobile view.
                                col.Item().Row(row =>
                                {
                                    row.RelativeItem(2);
                                    row.RelativeItem(6).Image(imgBytes).FitWidth();
                                    row.RelativeItem(2);
                                });
                            }
                            else
                            {
                                col.Item().Image(imgBytes).FitWidth();
                            }
                        }

                        col.Item().Text($"Tile {tile.Index} ({tile.Type})").SemiBold();

                        if (!string.IsNullOrWhiteSpace(tile.Caption))
                        {
                            foreach (var line in SplitPdfLines(tile.Caption))
                            {
                                col.Item().Text(line).Italic();
                            }
                        }

                        if (tile.Type.Equals("quiz", StringComparison.OrdinalIgnoreCase))
                        {
                            if (!string.IsNullOrWhiteSpace(tile.Question))
                            {
                                foreach (var line in SplitPdfLines(tile.Question))
                                {
                                    col.Item().Text(line);
                                }
                            }

                            if (tile.Answers.Count > 0)
                            {
                                col.Item().Text("Answers:").SemiBold().FontSize(11);
                                foreach (var ans in tile.Answers)
                                {
                                    var prefix = model.IncludeQuizAnswers && ans.IsCorrect ? "✅ " : "- ";
                                    foreach (var line in SplitPdfLines(prefix + (ans.Text ?? string.Empty)))
                                    {
                                        col.Item().Text(line);
                                    }
                                }
                            }
                        }
                        else
                        {
                            if (!string.IsNullOrWhiteSpace(tile.Text))
                            {
                                foreach (var line in SplitPdfLines(tile.Text))
                                {
                                    col.Item().Text(line);
                                }
                            }
                        }

                        if (!string.IsNullOrWhiteSpace(tile.VideoUrl))
                        {
                            col.Item().Text($"Video: {tile.VideoUrl}").FontSize(10).FontColor(Colors.Blue.Darken1);
                        }
                    });
                });
            }
        });

        var pdfBytes = doc.GeneratePdf();
        return new DocumentExportResult(pdfBytes, fileName);
    }

    /// <summary>
    /// QuestPDF will try to shape every character into a glyph; control chars like \r and \n (U+000D/U+000A)
    /// are not renderable glyphs and can crash the render with "Could not find an appropriate font fallback".
    /// We normalize and split into separate lines/paragraph items.
    /// </summary>
    private static IEnumerable<string> SplitPdfLines(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return Array.Empty<string>();

        var normalized = input
            .Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');

        // Keep it simple: omit empty lines, but preserve paragraph flow via column spacing.
        return normalized
            .Split('\n')
            .Select(x => x.Trim())
            .Where(x => !string.IsNullOrWhiteSpace(x))
            .ToArray();
    }

    private async Task<DocumentExportResult> RenderDocxAsync(StoryDocModel model, CancellationToken ct)
    {
        var fileName = model.IsDraft
            ? $"{model.StoryId}-draft-{model.Locale}.docx"
            : $"{model.StoryId}-v{model.Version ?? 1}-{model.Locale}.docx";

        var container = model.IsDraft ? _sas.DraftContainer : _sas.PublishedContainer;

        using var ms = new MemoryStream();
        using (var wordDoc = WordprocessingDocument.Create(ms, WordprocessingDocumentType.Document, true))
        {
            var mainPart = wordDoc.AddMainDocumentPart();
            mainPart.Document = new DocumentFormat.OpenXml.Wordprocessing.Document(new Body());
            var body = mainPart.Document.Body!;

            body.AppendChild(new Paragraph(new Run(new Text(model.Title))) { ParagraphProperties = new ParagraphProperties(new Justification { Val = JustificationValues.Center }) });
            body.AppendChild(new Paragraph(new Run(new Text($"StoryId: {model.StoryId}"))));
            body.AppendChild(new Paragraph(new Run(new Text($"Language: {model.Locale}"))));

            if (model.IncludeCover && !string.IsNullOrWhiteSpace(model.CoverImageBlobPath))
            {
                var coverBytes = await TryDownloadImageAsync(container, model.CoverImageBlobPath!, ct);
                if (coverBytes != null)
                {
                    AppendImage(mainPart, body, coverBytes, model.CoverImageBlobPath!);
                }
            }

            foreach (var tile in model.Tiles)
            {
                body.AppendChild(new Paragraph(new Run(new Text($"Tile {tile.Index} ({tile.Type})"))) { ParagraphProperties = new ParagraphProperties(new SpacingBetweenLines { After = "120" }) });

                if (!string.IsNullOrWhiteSpace(tile.Caption))
                    body.AppendChild(new Paragraph(new Run(new Text(tile.Caption))));

                if (tile.Type.Equals("quiz", StringComparison.OrdinalIgnoreCase))
                {
                    if (!string.IsNullOrWhiteSpace(tile.Question))
                        body.AppendChild(new Paragraph(new Run(new Text(tile.Question))));

                    foreach (var ans in tile.Answers)
                    {
                        var prefix = model.IncludeQuizAnswers && ans.IsCorrect ? "✅ " : "- ";
                        body.AppendChild(new Paragraph(new Run(new Text(prefix + ans.Text))));
                    }
                }
                else
                {
                    if (!string.IsNullOrWhiteSpace(tile.Text))
                        body.AppendChild(new Paragraph(new Run(new Text(tile.Text))));
                }

                if (!string.IsNullOrWhiteSpace(tile.VideoUrl))
                    body.AppendChild(new Paragraph(new Run(new Text($"Video: {tile.VideoUrl}"))));

                if (!string.IsNullOrWhiteSpace(tile.ImageBlobPath))
                {
                    var img = await TryDownloadImageAsync(container, tile.ImageBlobPath!, ct);
                    if (img != null)
                    {
                        AppendImage(mainPart, body, img, tile.ImageBlobPath!);
                    }
                }

                // Visual separator
                body.AppendChild(new Paragraph(new Run(new Text(" "))) { ParagraphProperties = new ParagraphProperties(new SpacingBetweenLines { After = "240" }) });
            }

            mainPart.Document.Save();
        }

        return new DocumentExportResult(ms.ToArray(), fileName);
    }

    private static void AppendImage(MainDocumentPart mainPart, Body body, byte[] bytes, string blobPath)
    {
        // Basic content type inference
        var ext = Path.GetExtension(blobPath).ToLowerInvariant();
        var imagePartType = ext switch
        {
            ".jpg" or ".jpeg" => ImagePartType.Jpeg,
            ".gif" => ImagePartType.Gif,
            ".bmp" => ImagePartType.Bmp,
            ".tiff" => ImagePartType.Tiff,
            _ => ImagePartType.Png
        };

        var imagePart = mainPart.AddImagePart(imagePartType);
        using (var stream = new MemoryStream(bytes))
        {
            imagePart.FeedData(stream);
        }

        var relId = mainPart.GetIdOfPart(imagePart);

        // Fixed sizing for now (6 inches wide). Word uses EMUs.
        const long cx = 6L * 914400L;
        const long cy = 4L * 914400L;

        var element =
            new Drawing(
                new DocumentFormat.OpenXml.Drawing.Wordprocessing.Inline(
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.Extent { Cx = cx, Cy = cy },
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.EffectExtent
                    {
                        LeftEdge = 0L,
                        TopEdge = 0L,
                        RightEdge = 0L,
                        BottomEdge = 0L
                    },
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.DocProperties
                    {
                        Id = (UInt32Value)1U,
                        Name = "Picture"
                    },
                    new DocumentFormat.OpenXml.Drawing.Wordprocessing.NonVisualGraphicFrameDrawingProperties(
                        new DocumentFormat.OpenXml.Drawing.GraphicFrameLocks { NoChangeAspect = true }),
                    new DocumentFormat.OpenXml.Drawing.Graphic(
                        new DocumentFormat.OpenXml.Drawing.GraphicData(
                            new DocumentFormat.OpenXml.Drawing.Pictures.Picture(
                                new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureProperties(
                                    new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualDrawingProperties
                                    {
                                        Id = (UInt32Value)0U,
                                        Name = "Image"
                                    },
                                    new DocumentFormat.OpenXml.Drawing.Pictures.NonVisualPictureDrawingProperties()),
                                new DocumentFormat.OpenXml.Drawing.Pictures.BlipFill(
                                    new DocumentFormat.OpenXml.Drawing.Blip
                                    {
                                        Embed = relId,
                                        CompressionState = DocumentFormat.OpenXml.Drawing.BlipCompressionValues.Print
                                    },
                                    new DocumentFormat.OpenXml.Drawing.Stretch(new DocumentFormat.OpenXml.Drawing.FillRectangle())),
                                new DocumentFormat.OpenXml.Drawing.Pictures.ShapeProperties(
                                    new DocumentFormat.OpenXml.Drawing.Transform2D(
                                        new DocumentFormat.OpenXml.Drawing.Offset { X = 0L, Y = 0L },
                                        new DocumentFormat.OpenXml.Drawing.Extents { Cx = cx, Cy = cy }),
                                    new DocumentFormat.OpenXml.Drawing.PresetGeometry(
                                        new DocumentFormat.OpenXml.Drawing.AdjustValueList())
                                    { Preset = DocumentFormat.OpenXml.Drawing.ShapeTypeValues.Rectangle })))
                        { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }))
                {
                    DistanceFromTop = 0U,
                    DistanceFromBottom = 0U,
                    DistanceFromLeft = 0U,
                    DistanceFromRight = 0U
                });

        var para = new Paragraph(new Run(element));
        body.AppendChild(para);
    }
}


