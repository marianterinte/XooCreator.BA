using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GenerateFullStoryEndpoint
{
    private readonly IGoogleFullStoryService _fullStoryService;
    private readonly IAuth0UserService _auth0;

    public GenerateFullStoryEndpoint(
        IGoogleFullStoryService fullStoryService,
        IAuth0UserService auth0)
    {
        _fullStoryService = fullStoryService;
        _auth0 = auth0;
    }

    public record GenerateFullStoryRequest(
        string Title,
        string Summary,
        string LanguageCode,
        List<string>? AgeGroupIds = null,
        List<string>? TopicIds = null,
        int NumberOfPages = 5,
        string? StoryInstructions = null,
        bool GenerateImages = false,
        bool GenerateAudio = false
    );

    public record GeneratedPageDto(
        int PageNumber,
        string Text,
        string Caption,
        string? ImageBase64,
        string ImageMimeType,
        string? AudioBase64,
        string AudioFormat
    );

    public record GenerateFullStoryResponse(
        List<GeneratedPageDto> Pages
    );

    [Route("/api/{locale}/story-editor/generate-full-story")]
    [Authorize]
    public static async Task<Results<Ok<GenerateFullStoryResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] GenerateFullStoryEndpoint ep,
        [FromBody] GenerateFullStoryRequest request,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
        {
            return TypedResults.Unauthorized();
        }

        // Check if user is PremiumCreator
        if (!ep._auth0.HasRole(user, UserRole.PremiumCreator))
        {
            return TypedResults.BadRequest("This feature is only available for Premium Creator users.");
        }

        if (string.IsNullOrWhiteSpace(request.Title))
        {
            return TypedResults.BadRequest("Title is required");
        }

        if (string.IsNullOrWhiteSpace(request.Summary))
        {
            return TypedResults.BadRequest("Summary is required");
        }

        if (string.IsNullOrWhiteSpace(request.LanguageCode))
        {
            return TypedResults.BadRequest("LanguageCode is required");
        }

        if (request.NumberOfPages < 1 || request.NumberOfPages > 10)
        {
            return TypedResults.BadRequest("NumberOfPages must be between 1 and 10");
        }

        if (request.StoryInstructions != null && request.StoryInstructions.Length > 3000)
        {
            return TypedResults.BadRequest("StoryInstructions must not exceed 3000 characters");
        }

        try
        {
            var generatedPages = await ep._fullStoryService.GenerateFullStoryAsync(
                request.Title,
                request.Summary,
                request.LanguageCode,
                request.AgeGroupIds,
                request.TopicIds,
                request.NumberOfPages,
                request.StoryInstructions,
                request.GenerateImages,
                request.GenerateAudio,
                ct);

            // Convert to DTO with base64 images and audio
            var pageDtos = generatedPages.Select(page => new GeneratedPageDto(
                PageNumber: page.PageNumber,
                Text: page.Text,
                Caption: page.Caption,
                ImageBase64: page.ImageData != null ? Convert.ToBase64String(page.ImageData) : null,
                ImageMimeType: page.ImageMimeType,
                AudioBase64: page.AudioData != null ? Convert.ToBase64String(page.AudioData) : null,
                AudioFormat: page.AudioFormat
            )).ToList();

            return TypedResults.Ok(new GenerateFullStoryResponse(
                Pages: pageDtos
            ));
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest($"Failed to generate story: {ex.Message}");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"An error occurred: {ex.Message}");
        }
    }
}

