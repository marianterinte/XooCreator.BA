using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class GenerateTextEndpoint
{
    private readonly IGoogleTextService _googleTextService;
    private readonly IAuth0UserService _auth0;

    public GenerateTextEndpoint(IGoogleTextService googleTextService, IAuth0UserService auth0)
    {
        _googleTextService = googleTextService;
        _auth0 = auth0;
    }

    public record GenerateTextRequest(
        string StoryJson, // Full story JSON (serialized)
        string LanguageCode,
        string? ExtraInstructions = null // Optional style instructions
    );

    public record GenerateTextResponse(
        string Text // Generated text for the next page
    );

    [Route("/api/{locale}/story-editor/generate-text")]
    [Authorize]
    public static async Task<Results<Ok<GenerateTextResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] GenerateTextEndpoint ep,
        [FromBody] GenerateTextRequest request,
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

        if (string.IsNullOrWhiteSpace(request.LanguageCode))
        {
            return TypedResults.BadRequest("LanguageCode is required");
        }

        try
        {
            var generatedText = await ep._googleTextService.GenerateNextPageAsync(
                request.StoryJson,
                request.LanguageCode,
                request.ExtraInstructions,
                ct);

            return TypedResults.Ok(new GenerateTextResponse(
                Text: generatedText
            ));
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest($"Failed to generate text: {ex.Message}");
        }
        catch (Exception ex)
        {
            return TypedResults.BadRequest($"An error occurred: {ex.Message}");
        }
    }
}

