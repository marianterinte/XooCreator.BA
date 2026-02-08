using Microsoft.AspNetCore.Authorization;
using System.Linq;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Features.StoryEditor.Services;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure.Services;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

[Endpoint]
public class TranslateStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IStoryTranslationService _translationService;
    private readonly IAuth0UserService _auth0;
    private readonly ILogger<TranslateStoryEndpoint> _logger;

    public TranslateStoryEndpoint(
        IStoryCraftsRepository crafts,
        IStoryTranslationService translationService,
        IAuth0UserService auth0,
        ILogger<TranslateStoryEndpoint> logger)
    {
        _crafts = crafts;
        _translationService = translationService;
        _auth0 = auth0;
        _logger = logger;
    }

    public record TranslateStoryRequest
    {
        public string? ReferenceLanguage { get; init; }
        public List<string>? TargetLanguages { get; init; }
        public string? ApiKey { get; init; }
        public string? Provider { get; init; }
    }

    public record TranslateStoryResponse
    {
        public bool Ok { get; init; } = true;
        public List<string> UpdatedLanguages { get; init; } = new();
        public int FieldsTranslated { get; init; }
        public int FieldsSkipped { get; init; }
    }

    [Route("/api/{locale}/stories/{storyId}/translate")]
    [Authorize]
    public static async Task<Results<Ok<TranslateStoryResponse>, BadRequest<string>, NotFound, UnauthorizedHttpResult, ForbidHttpResult, Conflict<string>>> HandlePost(
        [FromRoute] string locale,
        [FromRoute] string storyId,
        [FromBody] TranslateStoryRequest request,
        [FromServices] TranslateStoryEndpoint ep,
        CancellationToken ct)
    {
        _ = locale;
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null)
            return TypedResults.Unauthorized();

        var isCreator = ep._auth0.HasRole(user, UserRole.Creator);
        var isAdmin = ep._auth0.HasRole(user, UserRole.Admin);
        if (!isCreator && !isAdmin)
        {
            ep._logger.LogWarning("Translate forbidden: userId={UserId} not creator/admin", user.Id);
            return TypedResults.Forbid();
        }

        if (request == null)
            return TypedResults.BadRequest("Request body is required.");

        if (string.IsNullOrWhiteSpace(request.ReferenceLanguage))
            return TypedResults.BadRequest("ReferenceLanguage is required.");

        if (request.TargetLanguages == null || request.TargetLanguages.Count == 0)
            return TypedResults.BadRequest("TargetLanguages is required.");

        if (string.IsNullOrWhiteSpace(request.ApiKey))
            return TypedResults.BadRequest("ApiKey is required.");

        if (string.IsNullOrWhiteSpace(storyId))
            return TypedResults.BadRequest("StoryId is required.");

        var craft = await ep._crafts.GetAsync(storyId, ct);
        if (craft == null)
            return TypedResults.NotFound();

        // Only owner can edit (unless Admin)
        if (craft.OwnerUserId != user.Id && !isAdmin)
        {
            ep._logger.LogWarning("Translate forbidden: userId={UserId} storyId={StoryId} not owner", user.Id, storyId);
            return TypedResults.Forbid();
        }

        // Disallow edits in read-only states
        var status = (craft.Status ?? "draft").ToLowerInvariant();
        if (status is "sent_for_approval" or "in_review" or "approved" or "published")
        {
            ep._logger.LogWarning("Translate read-only: storyId={StoryId} status={Status}", storyId, status);
            return TypedResults.Conflict("Story is read-only in the current status.");
        }

        try
        {
            var result = await ep._translationService.TranslateStoryAsync(
                storyId,
                request.ReferenceLanguage!,
                request.TargetLanguages!,
                request.ApiKey!,
                ct);

            return TypedResults.Ok(new TranslateStoryResponse
            {
                UpdatedLanguages = result.UpdatedLanguages.ToList(),
                FieldsTranslated = result.FieldsTranslated,
                FieldsSkipped = result.FieldsSkipped
            });
        }
        catch (ArgumentException ex)
        {
            return TypedResults.BadRequest(ex.Message);
        }
        catch (InvalidOperationException ex)
        {
            return TypedResults.BadRequest($"Failed to translate: {ex.Message}");
        }
    }
}
