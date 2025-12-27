namespace XooCreator.BA.Infrastructure.Services.Images;

public sealed class ImageCompressionOptions
{
    public const string SectionName = "ImageCompression";

    /// <summary>
    /// Only used by the admin backfill endpoint to limit work per request.
    /// </summary>
    public int ProcessBatchSize { get; set; } = 10;

    /// <summary>
    /// How close width/height must be to 4/5 to be considered valid (e.g. 0.01 = 1%).
    /// </summary>
    public double FourByFiveTolerance { get; set; } = 0.01;

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


