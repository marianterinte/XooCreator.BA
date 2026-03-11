using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Repairs story text to fix consistency issues using AI.
/// </summary>
public sealed class StoryRepairService : IStoryRepairService
{
    private readonly IGoogleTextService _googleTextService;
    private readonly ILogger<StoryRepairService> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public StoryRepairService(
        IGoogleTextService googleTextService,
        ILogger<StoryRepairService> logger)
    {
        _googleTextService = googleTextService;
        _logger = logger;
    }

    public async Task<string> RepairAsync(
        StoryBible bible,
        string storyText,
        StoryValidationResult validationResult,
        string apiKey,
        string? model = null,
        CancellationToken ct = default)
    {
        if (validationResult.IsValid || validationResult.Issues.Count == 0)
        {
            _logger.LogInformation("No repairs needed, story is valid");
            return storyText;
        }

        var issuesSummary = string.Join("\n", validationResult.Issues.Select(i => 
            $"- {i.Type}: {i.Description}" + (i.SuggestedFix != null ? $" (Fix: {i.SuggestedFix})" : "")));

        var systemPrompt = @"You are a story editor. Fix the story text to match the Story Bible while preserving the narrative flow.
Keep the same story structure (pages marked with ###P1, ###P2, etc.).
Make minimal changes - only fix what's necessary to address the issues.
Return ONLY the corrected story text, nothing else.";

        var userContent = $@"STORY BIBLE:
{JsonSerializer.Serialize(bible, JsonOptions)}

ISSUES TO FIX:
{issuesSummary}

ORIGINAL STORY TEXT:
{storyText}

Fix the issues and return the corrected story text. Keep the ###T, ###S, ###P markers intact.";

        _logger.LogInformation("Repairing story with {IssueCount} issues", validationResult.Issues.Count);

        var repairedText = await _googleTextService.GenerateContentAsync(
            systemPrompt,
            userContent,
            apiKey,
            model,
            responseMimeType: "text/plain",
            ct);

        _logger.LogInformation("Story repaired successfully");
        return repairedText.Trim();
    }
}
