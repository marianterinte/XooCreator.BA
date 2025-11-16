using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GenerateImageEndpoint
{
    private readonly IGoogleImageService _googleImageService;
    private readonly IAuth0UserService _auth0;

    public GenerateImageEndpoint(IGoogleImageService googleImageService, IAuth0UserService auth0)
    {
        _googleImageService = googleImageService;
        _auth0 = auth0;
    }

    public record GenerateImageRequest(
        string StoryJson,              // Full story JSON (serialized)
        string TileText,              // Current tile text
        string LanguageCode,
        string? StyleComment = null,   // Optional comment/instructions
        string? ReferenceImageBase64 = null,  // Optional reference image (base64)
        string? ReferenceImageMimeType = null  // Optional MIME type
    );

    public record GenerateImageResponse(
        string ImageData,  // Base64-encoded image
        string MimeType,   // Usually "image/png"
        string PreviewUrl  // Data URL for immediate preview
    );

    [Route("/api/{locale}/story-editor/generate-image")]
    [Authorize]
    public static async Task<Results<Ok<GenerateImageResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] GenerateImageEndpoint ep,
        [FromBody] GenerateImageRequest request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        if (string.IsNullOrWhiteSpace(request.StoryJson))
        {
            return TypedResults.BadRequest("StoryJson is required");
        }

        if (string.IsNullOrWhiteSpace(request.TileText))
        {
            return TypedResults.BadRequest("TileText is required");
        }

        if (string.IsNullOrWhiteSpace(request.LanguageCode))
        {
            return TypedResults.BadRequest("LanguageCode is required");
        }

        try
        {
            // Convert reference image from base64 if provided
            byte[]? referenceImageBytes = null;
            if (!string.IsNullOrWhiteSpace(request.ReferenceImageBase64))
            {
                try
                {
                    referenceImageBytes = Convert.FromBase64String(request.ReferenceImageBase64);
                }
                catch (FormatException)
                {
                    return TypedResults.BadRequest("Invalid base64 reference image data");
                }
            }

            var (imageData, mimeType) = await ep._googleImageService.GenerateStoryImageAsync(
                request.StoryJson,
                request.TileText,
                request.LanguageCode,
                request.StyleComment,
                referenceImageBytes,
                request.ReferenceImageMimeType,
                ct);

            var base64Image = Convert.ToBase64String(imageData);
            var previewUrl = $"data:{mimeType};base64,{base64Image}";

            return TypedResults.Ok(new GenerateImageResponse(
                ImageData: base64Image,
                MimeType: mimeType,
                PreviewUrl: previewUrl
            ));
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest($"Failed to generate image: {ex.Message}");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"An error occurred: {ex.Message}");
        }
    }
}

