using System.Globalization;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Formats;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using SixLabors.ImageSharp.Processing;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Infrastructure.Services.Images;

public sealed class ImageCompressionService : IImageCompressionService
{
    private static readonly double[] DefaultAllowedRatios = { 4d / 5d, 7d / 9d }; // 4:5 and 7:9

    private readonly IBlobSasService _sas;
    private readonly IOptions<ImageCompressionOptions> _options;
    private readonly ILogger<ImageCompressionService> _logger;

    public ImageCompressionService(
        IBlobSasService sas,
        IOptions<ImageCompressionOptions> options,
        ILogger<ImageCompressionService> logger)
    {
        _sas = sas;
        _options = options;
        _logger = logger;
    }

    public async Task<ImageCompressionResult> EnsureStorySizeVariantsAsync(
        string sourceBlobPath,
        string targetBasePath,
        string filename,
        bool overwriteExisting,
        CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(sourceBlobPath))
        {
            return new ImageCompressionResult { Success = false, ErrorMessage = "sourceBlobPath is required." };
        }
        if (string.IsNullOrWhiteSpace(targetBasePath))
        {
            return new ImageCompressionResult { Success = false, ErrorMessage = "targetBasePath is required." };
        }
        if (string.IsNullOrWhiteSpace(filename))
        {
            return new ImageCompressionResult { Success = false, ErrorMessage = "filename is required." };
        }

        try
        {
            var opt = _options.Value;

            var sourceClient = _sas.GetBlobClient(_sas.PublishedContainer, sourceBlobPath);
            var props = await sourceClient.GetPropertiesAsync(cancellationToken: ct);
            var originalSize = props.Value.ContentLength;

            await using var srcStream = (await sourceClient.DownloadStreamingAsync(cancellationToken: ct)).Value.Content;
            using var image = await Image.LoadAsync(srcStream, ct);

            var ratio = image.Width / (double)image.Height;
            var allowed = (opt.AllowedAspectRatios is { Length: > 0 } ? opt.AllowedAspectRatios : DefaultAllowedRatios);
            var bestMatch = allowed
                .Select(r => new { Ratio = r, Delta = Math.Abs(ratio - r) })
                .OrderBy(x => x.Delta)
                .First();

            if (bestMatch.Delta > opt.FourByFiveTolerance)
            {
                _logger.LogInformation(
                    "Image variant generation skipped (unsupported aspect ratio): path={Path} w={W} h={H} ratio={Ratio} bestMatch={BestMatch} delta={Delta} tol={Tol}",
                    sourceBlobPath,
                    image.Width,
                    image.Height,
                    ratio.ToString("0.####", CultureInfo.InvariantCulture),
                    bestMatch.Ratio.ToString("0.####", CultureInfo.InvariantCulture),
                    bestMatch.Delta.ToString("0.####", CultureInfo.InvariantCulture),
                    opt.FourByFiveTolerance.ToString("0.####", CultureInfo.InvariantCulture));

                return new ImageCompressionResult
                {
                    Success = true,
                    SkippedBecauseNotFourByFive = true,
                    OriginalSizeBytes = originalSize
                };
            }

            var smallPath = $"{targetBasePath.TrimEnd('/')}/s/{filename}";
            var mediumPath = $"{targetBasePath.TrimEnd('/')}/m/{filename}";

            long smallSize = 0;
            long mediumSize = 0;

            if (overwriteExisting || !await _sas.GetBlobClient(_sas.PublishedContainer, smallPath).ExistsAsync(ct))
            {
                smallSize = await SaveResizedVariantAsync(image, filename, smallPath, maxWidth: opt.SmallMaxWidth, quality: opt.SmallQuality, ct);
            }

            if (overwriteExisting || !await _sas.GetBlobClient(_sas.PublishedContainer, mediumPath).ExistsAsync(ct))
            {
                mediumSize = await SaveResizedVariantAsync(image, filename, mediumPath, maxWidth: opt.MediumMaxWidth, quality: opt.MediumQuality, ct);
            }

            return new ImageCompressionResult
            {
                Success = true,
                OriginalSizeBytes = originalSize,
                SmallSizeBytes = smallSize,
                MediumSizeBytes = mediumSize,
                SmallPath = smallPath,
                MediumPath = mediumPath
            };
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Image variant generation failed: source={Source} base={Base} filename={Filename}", sourceBlobPath, targetBasePath, filename);
            return new ImageCompressionResult { Success = false, ErrorMessage = ex.Message };
        }
    }

    private async Task<long> SaveResizedVariantAsync(
        Image source,
        string filename,
        string targetBlobPath,
        int maxWidth,
        int quality,
        CancellationToken ct)
    {
        maxWidth = Math.Max(1, maxWidth);
        quality = Math.Clamp(quality, 1, 100);

        var targetWidth = Math.Min(source.Width, maxWidth);
        var targetHeight = (int)Math.Round(source.Height * (targetWidth / (double)source.Width));
        targetHeight = Math.Max(1, targetHeight);

        using var resized = source.Clone(ctx => ctx.Resize(targetWidth, targetHeight));

        var (encoder, contentType) = ResolveEncoder(filename, quality);
        await using var ms = new MemoryStream();
        await resized.SaveAsync(ms, encoder, ct);
        ms.Position = 0;

        var client = _sas.GetBlobClient(_sas.PublishedContainer, targetBlobPath);
        await client.UploadAsync(ms, overwrite: true, cancellationToken: ct);

        // Set content type for correct serving/caching
        await client.SetHttpHeadersAsync(new Azure.Storage.Blobs.Models.BlobHttpHeaders
        {
            ContentType = contentType
        }, cancellationToken: ct);

        return ms.Length;
    }

    private static (IImageEncoder Encoder, string ContentType) ResolveEncoder(string filename, int quality)
    {
        var ext = Path.GetExtension(filename).Trim().ToLowerInvariant();
        return ext switch
        {
            ".jpg" or ".jpeg" => (new JpegEncoder { Quality = quality }, "image/jpeg"),
            ".png" => (new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression }, "image/png"),
            ".webp" => (new SixLabors.ImageSharp.Formats.Webp.WebpEncoder { Quality = quality }, "image/webp"),
            _ => (new PngEncoder { CompressionLevel = PngCompressionLevel.BestCompression }, "image/png")
        };
    }
}


