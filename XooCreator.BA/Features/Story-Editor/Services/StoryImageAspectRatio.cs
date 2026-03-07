using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Hardcoded aspect ratio for all story page images (backend and UI).
/// Every generated story illustration is cropped to 4:5; the frontend displays them at 4:5.
/// </summary>
public static class StoryImageAspectRatio
{
    /// <summary>Width part of the aspect ratio (4:5).</summary>
    public const int Width = 4;

    /// <summary>Height part of the aspect ratio (4:5).</summary>
    public const int Height = 5;

    /// <summary>
    /// Crops image bytes to 4:5 aspect ratio (center crop). Use for all story image generation outputs.
    /// </summary>
    /// <param name="imageBytes">Raw image bytes (e.g. PNG/JPEG from OpenAI or Gemini).</param>
    /// <returns>Cropped image as PNG bytes. Returns original bytes if crop fails.</returns>
    public static byte[] CropTo4x5(byte[] imageBytes)
    {
        if (imageBytes == null || imageBytes.Length == 0)
            return imageBytes;

        try
        {
            using var originalImage = Image.FromStream(new MemoryStream(imageBytes));
            var originalWidth = originalImage.Width;
            var originalHeight = originalImage.Height;

            var targetHeight = originalHeight;
            var targetWidth = (int)(targetHeight * (double)Width / Height);
            if (targetWidth > originalWidth)
            {
                targetWidth = originalWidth;
                targetHeight = (int)(targetWidth * (double)Height / Width);
            }

            var cropX = (originalWidth - targetWidth) / 2;
            var cropY = (originalHeight - targetHeight) / 2;

            using var croppedBitmap = new Bitmap(targetWidth, targetHeight);
            using (var graphics = Graphics.FromImage(croppedBitmap))
            {
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(originalImage,
                    new Rectangle(0, 0, targetWidth, targetHeight),
                    new Rectangle(cropX, cropY, targetWidth, targetHeight),
                    GraphicsUnit.Pixel);
            }

            using var ms = new MemoryStream();
            croppedBitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
        catch
        {
            return imageBytes;
        }
    }
}
