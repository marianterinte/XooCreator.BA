using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Reflection;
using System.Text;
using System.Text.Json;
using OpenAI.Images;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Service for generating story images using OpenAI DALL-E.
/// </summary>
public interface IOpenAIImageService
{
    /// <summary>
    /// Generates a single illustration image for the current story page/tile.
    /// </summary>
    Task<(byte[] ImageData, string MimeType)> GenerateStoryImageAsync(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions = null,
        byte[]? referenceImage = null,
        string? referenceImageMimeType = null,
        CancellationToken ct = default);
}

/// <summary>
/// Generates story illustrations using OpenAI DALL-E.
/// </summary>
public class OpenAIImageService : IOpenAIImageService
{
    private readonly ImageClient _imageClient;
    private readonly HttpClient _httpClient;
    private readonly string _apiKey;
    private readonly string _organizationId;
    private readonly string _textModel;
    private readonly string _textBaseUrl;
    private readonly string _model;
    private readonly GeneratedImageSize _imageSize;
    private readonly ILogger<OpenAIImageService> _logger;

    public OpenAIImageService(
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory,
        ILogger<OpenAIImageService> logger)
    {
        _httpClient = httpClientFactory.CreateClient();
        _apiKey = configuration["OpenAI:ApiKey"]
            ?? throw new InvalidOperationException("OpenAI:ApiKey is not configured in appsettings.json");
        _organizationId = configuration["OpenAI:OrganizationId"] ?? string.Empty;
        _textModel = configuration["OpenAI:Text:Model"] ?? "gpt-4o-mini";
        _textBaseUrl = configuration["OpenAI:Text:BaseUrl"] ?? "https://api.openai.com/v1";
        
        _model = configuration["OpenAI:Image:Model"] ?? "gpt-image-1";
        var sizeConfig = configuration["OpenAI:Image:Size"] ?? "1024x1024";
        _imageSize = ParseImageSize(sizeConfig);
        
        // gpt-image-1 doesn't support quality and style parameters (unlike DALL-E 3)
        // Create ImageClient with model and API key
        _imageClient = new ImageClient(_model, _apiKey);
        _logger = logger;
    }
    
    private static GeneratedImageSize ParseImageSize(string size)
    {
        return size switch
        {
            // DALL-E 2 sizes
            "256x256" => GeneratedImageSize.W256xH256,
            "512x512" => GeneratedImageSize.W512xH512,
            // DALL-E 3 sizes
            "1024x1024" => GeneratedImageSize.W1024xH1024,
            "1024x1792" => GeneratedImageSize.W1024xH1792, // Portrait 4:7
            "1792x1024" => GeneratedImageSize.W1792xH1024, // Landscape 7:4
            _ => GeneratedImageSize.W1024xH1024 // Default for gpt-image-1 and DALL-E 3
        };
    }

    public async Task<(byte[] ImageData, string MimeType)> GenerateStoryImageAsync(
        string storyJson,
        string tileText,
        string languageCode,
        string? extraInstructions = null,
        byte[]? referenceImage = null,
        string? referenceImageMimeType = null,
        CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(storyJson))
            throw new ArgumentException("Story JSON cannot be empty", nameof(storyJson));

        if (string.IsNullOrWhiteSpace(tileText))
            throw new ArgumentException("Tile text cannot be empty", nameof(tileText));

        var optimizedJson = ExtractOptimizedStoryContext(storyJson);
        var prompt = await BuildImagePromptAsync(optimizedJson, tileText, languageCode, extraInstructions, ct);

        // gpt-image-1 doesn't support reference images, so we ignore them
        if (referenceImage != null)
        {
            _logger.LogWarning("gpt-image-1 does not support reference images, ignoring reference image parameter");
        }

        try
        {
            _logger.LogInformation(
                "Calling OpenAI Image API. Language: {LanguageCode}, Model: {Model}, Size: {Size}",
                languageCode, _model, _imageSize);

            // Configure image generation options
            // gpt-image-1 only supports: model, prompt, size, n
            // It does NOT support quality, style, or response_format in SDK (those are DALL-E 3 only)
            var options = new ImageGenerationOptions
            {
                Size = _imageSize,
                // Note: gpt-image-1 SDK returns bytes by default, no need to specify ResponseFormat
            };

            // Generate image using official OpenAI SDK
            var result = await _imageClient.GenerateImageAsync(prompt, options, ct);
            var generatedImage = result.Value;

            if (generatedImage?.ImageBytes == null || generatedImage.ImageBytes.Length == 0)
            {
                throw new InvalidOperationException("OpenAI returned empty image data");
            }

            var imageBytes = generatedImage.ImageBytes.ToArray();
            
            // Crop image to 4:5 aspect ratio (gpt-image-1 generates squares by default, crop to 4:5 for mobile/tablet optimization)
            var croppedImage = CropToAspectRatio(imageBytes, 4, 5);
            
            return (croppedImage, "image/png");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error while generating story image with OpenAI.");
            throw;
        }
    }

    private static string ExtractOptimizedStoryContext(string fullStoryJson)
    {
        try
        {
            using var doc = JsonDocument.Parse(fullStoryJson);
            var root = doc.RootElement;

            var title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
            var summary = root.TryGetProperty("summary", out var s) ? s.GetString() : null;

            var allTiles = new List<JsonElement>();
            if (root.TryGetProperty("tiles", out var tiles) && tiles.ValueKind == JsonValueKind.Array)
            {
                foreach (var tile in tiles.EnumerateArray())
                {
                    if (tile.TryGetProperty("type", out var type) &&
                        type.GetString()?.Equals("page", StringComparison.OrdinalIgnoreCase) == true)
                    {
                        allTiles.Add(tile);
                    }
                }
            }

            var lastPages = allTiles
                .OrderBy(t => t.TryGetProperty("sortOrder", out var so) ? so.GetInt32() : int.MaxValue)
                .TakeLast(2)
                .ToList();

            var context = new StringBuilder();
            if (!string.IsNullOrEmpty(title))
                context.AppendLine($"Title: {title}");
            if (!string.IsNullOrEmpty(summary))
                context.AppendLine($"Summary: {summary}");

            foreach (var tile in lastPages)
            {
                if (tile.TryGetProperty("text", out var txt))
                {
                    context.AppendLine($"Previous page: {txt.GetString()}");
                }
            }

            return context.ToString();
        }
        catch (Exception)
        {
            return fullStoryJson;
        }
    }

    private async Task<string> BuildImagePromptAsync(
        string optimizedJson,
        string tileText,
        string languageCode,
        string? extraInstructions,
        CancellationToken ct)
    {
        // DALL-E 2 has a maximum prompt length of 1000 characters
        // DALL-E 3 has a maximum prompt length of 4000 characters, but we'll keep 1000 for cost optimization
        const int maxPromptLength = 1000;
        
        var sb = new StringBuilder();
        
        // Extract title and summary from optimizedJson if available
        string? title = null;
        string? summary = null;
        try
        {
            using var doc = JsonDocument.Parse(optimizedJson);
            var root = doc.RootElement;
            title = root.TryGetProperty("title", out var t) ? t.GetString() : null;
            summary = root.TryGetProperty("summary", out var s) ? s.GetString() : null;
        }
        catch
        {
            // If parsing fails, ignore
        }
        
        // Build prompt with priority: tileText > summary > title > extraInstructions
        sb.Append("Create a colorful, child-friendly illustration for a children's story. ");
        
        // Add title if available
        if (!string.IsNullOrWhiteSpace(title))
        {
            sb.Append($"Story: {title}. ");
        }
        
        // Add summary (truncated if too long)
        if (!string.IsNullOrWhiteSpace(summary))
        {
            var summaryText = summary.Length > 200 ? summary.Substring(0, 200) + "..." : summary;
            sb.Append($"Context: {summaryText}. ");
        }
        
        // Add current page text (most important - this is what we're illustrating)
        if (!string.IsNullOrWhiteSpace(tileText))
        {
            sb.Append($"Illustrate this scene: {tileText}. ");
        }
        
        // Add style instructions
        sb.Append("Style: warm, friendly, suitable for children. Bright colors, simple shapes, expressive characters.");
        
        // Add extra instructions if there's space
        if (!string.IsNullOrWhiteSpace(extraInstructions))
        {
            sb.Append($" {extraInstructions}");
        }
        
        var initialPrompt = sb.ToString();
        
        // If prompt exceeds limit, use AI to summarize it while preserving important information
        if (initialPrompt.Length > maxPromptLength)
        {
            _logger.LogInformation(
                "Image prompt is {Length} characters (exceeds {MaxLength} limit). Summarizing with AI...",
                initialPrompt.Length, maxPromptLength);
            
            try
            {
                var summarizedPrompt = await SummarizePromptForImageAsync(initialPrompt, maxPromptLength, ct);
                return summarizedPrompt;
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Failed to summarize prompt with AI, falling back to truncation");
                // Fallback to simple truncation
                return initialPrompt.Substring(0, maxPromptLength);
            }
        }
        
        return initialPrompt;
    }
    
    /// <summary>
    /// Uses OpenAI to summarize a prompt while preserving important information for image generation.
    /// </summary>
    private async Task<string> SummarizePromptForImageAsync(string longPrompt, int maxLength, CancellationToken ct)
    {
        var systemMessage = "You are a prompt optimizer for image generation. Summarize the given prompt to under " +
                           $"{maxLength} characters while preserving ALL important visual details: characters, " +
                           "actions, objects, setting, colors, and style. Keep the scene description complete and vivid.";
        
        var userMessage = $"Summarize this image generation prompt to under {maxLength} characters, " +
                         "preserving all visual details:\n\n" + longPrompt;
        
        var requestBody = new
        {
            model = _textModel,
            messages = new[]
            {
                new { role = "system", content = systemMessage },
                new { role = "user", content = userMessage }
            },
            temperature = 0.3, // Lower temperature for more consistent summarization
            max_tokens = 300 // Enough for a summarized prompt
        };
        
        var json = JsonSerializer.Serialize(requestBody);
        using var content = new StringContent(json, Encoding.UTF8, "application/json");
        
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{_textBaseUrl}/chat/completions")
        {
            Content = content
        };
        
        request.Headers.Add("Authorization", $"Bearer {_apiKey}");
        if (!string.IsNullOrWhiteSpace(_organizationId))
        {
            request.Headers.Add("OpenAI-Organization", _organizationId);
        }
        
        using var response = await _httpClient.SendAsync(request, ct);
        var responseContent = await response.Content.ReadAsStringAsync(ct);
        
        if (!response.IsSuccessStatusCode)
        {
            _logger.LogError(
                "OpenAI summarization API returned {StatusCode}: {Body}",
                (int)response.StatusCode,
                responseContent);
            response.EnsureSuccessStatusCode();
        }
        
        // Extract summarized text from response
        using var doc = JsonDocument.Parse(responseContent);
        var root = doc.RootElement;
        
        if (root.TryGetProperty("choices", out var choices) && choices.ValueKind == JsonValueKind.Array)
        {
            var firstChoice = choices.EnumerateArray().FirstOrDefault();
            if (firstChoice.TryGetProperty("message", out var message) &&
                message.TryGetProperty("content", out var contentProp))
            {
                var summarized = contentProp.GetString() ?? string.Empty;
                
                // Ensure it's under maxLength (should be, but double-check)
                if (summarized.Length > maxLength)
                {
                    summarized = summarized.Substring(0, maxLength);
                }
                
                return summarized.Trim();
            }
        }
        
        throw new InvalidOperationException("Failed to extract summarized prompt from OpenAI response");
    }

    /// <summary>
    /// Crops an image to the specified aspect ratio (width:height).
    /// For 4:5 aspect ratio, crops from center horizontally.
    /// </summary>
    private byte[] CropToAspectRatio(byte[] imageBytes, int widthRatio, int heightRatio)
    {
        try
        {
            using var originalImage = Image.FromStream(new MemoryStream(imageBytes));
            
            var originalWidth = originalImage.Width;
            var originalHeight = originalImage.Height;
            
            // Calculate target dimensions for aspect ratio widthRatio:heightRatio
            // We keep the height and crop the width
            var targetHeight = originalHeight;
            var targetWidth = (int)(targetHeight * (double)widthRatio / heightRatio);
            
            // If calculated width is larger than original, keep width and crop height instead
            if (targetWidth > originalWidth)
            {
                targetWidth = originalWidth;
                targetHeight = (int)(targetWidth * (double)heightRatio / widthRatio);
            }
            
            // Calculate crop position (center)
            var cropX = (originalWidth - targetWidth) / 2;
            var cropY = (originalHeight - targetHeight) / 2;
            
            // Create cropped image
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
            
            // Convert to bytes
            using var ms = new MemoryStream();
            croppedBitmap.Save(ms, ImageFormat.Png);
            return ms.ToArray();
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to crop image to aspect ratio {WidthRatio}:{HeightRatio}, returning original image", widthRatio, heightRatio);
            return imageBytes; // Return original if crop fails
        }
    }

}

