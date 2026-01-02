namespace XooCreator.BA.Infrastructure.Services.Images;

public sealed class ImageCompressionOptions
{
    public const string SectionName = "ImageCompression";

    /// <summary>
    /// Only used by the admin backfill endpoint to limit work per request.
    /// </summary>
    public int ProcessBatchSize { get; set; } = 10;

    /// <summary>
    /// How close width/height must be to one of the allowed aspect ratios to be considered valid (e.g. 0.01 = 1%).
    /// Backward compatible name kept as-is, but it now applies to all allowed ratios.
    /// </summary>
    public double FourByFiveTolerance { get; set; } = 0.01;

    /// <summary>
    /// Allowed width/height aspect ratios for which we generate s/m variants.
    /// Default includes 4:5 (0.8) and 7:9 (~0.7778).
    /// </summary>
    public double[] AllowedAspectRatios { get; set; } = { 4d / 5d, 7d / 9d };

    /// <summary>
    /// Max width (px) for the "s" variant. Height is derived from aspect ratio.
    /// </summary>
    public int SmallMaxWidth { get; set; } = 480;

    /// <summary>
    /// Max width (px) for the "m" variant. Height is derived from aspect ratio.
    /// </summary>
    public int MediumMaxWidth { get; set; } = 768;

    /// <summary>
    /// JPEG/WebP quality for "s" (0-100). Used only for lossy formats.
    /// </summary>
    public int SmallQuality { get; set; } = 65;

    /// <summary>
    /// JPEG/WebP quality for "m" (0-100). Used only for lossy formats.
    /// </summary>
    public int MediumQuality { get; set; } = 80;
}


