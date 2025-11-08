using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using System.Text.RegularExpressions;
using XooCreator.BA.Infrastructure.Endpoints;
using XooCreator.BA.Infrastructure;
using XooCreator.BA.Infrastructure.Services;
using XooCreator.BA.Features.StoryEditor.Repositories;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.Endpoints;

public record CreateStoryRequest
{
    public string? StoryId { get; init; }
    public string? Lang { get; init; }
    public string? Title { get; init; } // reserved for future
    public int? StoryType { get; init; } // reserved for future
}

public record CreateStoryResponse
{
    public required string StoryId { get; init; }
}

[Endpoint]
public class CreateStoryEndpoint
{
    private readonly IStoryCraftsRepository _crafts;
    private readonly IUserContextService _userContext;
    private readonly IAuth0UserService _auth0;

    public CreateStoryEndpoint(IStoryCraftsRepository crafts, IUserContextService userContext, IAuth0UserService auth0)
    {
        _crafts = crafts;
        _userContext = userContext;
        _auth0 = auth0;
    }

    [Route("/api/{locale}/stories")]
    [Authorize]
    public static async Task<Results<Ok<CreateStoryResponse>, BadRequest<string>, UnauthorizedHttpResult>> HandlePost(
        [FromRoute] string locale,
        [FromServices] CreateStoryEndpoint ep,
        [FromBody] CreateStoryRequest req,
        CancellationToken ct)
    {
        var user = await ep._auth0.GetCurrentUserAsync(ct);
        if (user == null) return TypedResults.Unauthorized();

        var langTag = string.IsNullOrWhiteSpace(req.Lang) ? ep._userContext.GetRequestLocaleOrDefault("ro-ro") : req.Lang!;
        var lang = ToLanguageCode(langTag);
        string storyId = (req.StoryId ?? string.Empty).Trim();
        if (string.IsNullOrWhiteSpace(storyId))
        {
            return TypedResults.BadRequest("storyId is required (format: ends with -sN).");
        }

        // basic validation: ends with -sN
        if (!Regex.IsMatch(storyId, "-s[1-9]\\d*$", RegexOptions.IgnoreCase))
        {
            return TypedResults.BadRequest("storyId must end with -s1, -s2, ...");
        }

        var existing = await ep._crafts.GetAsync(storyId, lang, ct);
        if (existing != null)
        {
            return TypedResults.Ok(new CreateStoryResponse { StoryId = storyId });
        }

        await ep._crafts.CreateAsync(user.Id, storyId, lang, "draft", "{}", ct);
        return TypedResults.Ok(new CreateStoryResponse { StoryId = storyId });
    }

    private static LanguageCode ToLanguageCode(string tag)
    {
        var t = (tag ?? "ro-ro").ToLowerInvariant();
        return t switch
        {
            "en-us" => LanguageCode.EnUs,
            "hu-hu" => LanguageCode.HuHu,
            _ => LanguageCode.RoRo
        };
    }
}


