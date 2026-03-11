using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Validates story text against the Story Bible using AI and local rules.
/// </summary>
public sealed class StoryValidator : IStoryValidator
{
    private readonly IGoogleTextService _googleTextService;
    private readonly ILogger<StoryValidator> _logger;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public StoryValidator(
        IGoogleTextService googleTextService,
        ILogger<StoryValidator> logger)
    {
        _googleTextService = googleTextService;
        _logger = logger;
    }

    public async Task<StoryValidationResult> ValidateAsync(
        StoryBible bible,
        string storyText,
        string apiKey,
        string? model = null,
        CancellationToken ct = default)
    {
        var issues = new List<ValidationIssue>();
        var warnings = new List<string>();
        var errors = new List<string>();

        // Local validation (fast, no AI)
        LocalValidation(bible, storyText, issues, warnings);

        // If local validation found critical issues, skip AI validation
        if (issues.Any(i => i.Type == "critical"))
        {
            return new StoryValidationResult
            {
                IsValid = false,
                Warnings = warnings,
                Errors = errors,
                Issues = issues
            };
        }

        // AI-based validation for subtle issues
        try
        {
            var aiIssues = await AiValidationAsync(bible, storyText, apiKey, model, ct);
            issues.AddRange(aiIssues);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "AI validation failed, using local validation only");
            warnings.Add("AI validation skipped due to error");
        }

        var isValid = !issues.Any(i => i.Type is "critical" or "error");
        
        _logger.LogInformation("Story validation complete: IsValid={IsValid}, Issues={IssueCount}", 
            isValid, issues.Count);

        return new StoryValidationResult
        {
            IsValid = isValid,
            Warnings = warnings,
            Errors = errors,
            Issues = issues
        };
    }

    private static void LocalValidation(
        StoryBible bible,
        string storyText,
        List<ValidationIssue> issues,
        List<string> warnings)
    {
        var textLower = storyText.ToLowerInvariant();

        // Check if character names appear in the story
        foreach (var character in bible.Characters)
        {
            if (!textLower.Contains(character.Name.ToLowerInvariant()))
            {
                issues.Add(new ValidationIssue
                {
                    Type = "warning",
                    Description = $"Character '{character.Name}' is defined in Bible but not mentioned in story",
                    SuggestedFix = $"Add mentions of {character.Name} to the story"
                });
            }
        }

        // Check story length
        var wordCount = storyText.Split([' ', '\n', '\r'], StringSplitOptions.RemoveEmptyEntries).Length;
        if (wordCount < 50)
        {
            issues.Add(new ValidationIssue
            {
                Type = "warning",
                Description = "Story seems too short",
                SuggestedFix = "Consider expanding the narrative"
            });
        }

        // Check for unexpected characters (not in Bible)
        var commonAnimals = new[] { "bunny", "rabbit", "duck", "cat", "dog", "bear", "bird", "frog", "owl", "mouse", "fox" };
        foreach (var animal in commonAnimals)
        {
            if (textLower.Contains(animal) && !bible.Characters.Any(c => 
                c.Species.Contains(animal, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Contains(animal, StringComparison.OrdinalIgnoreCase)))
            {
                warnings.Add($"Story mentions '{animal}' which is not defined as a character in Bible");
            }
        }
    }

    private async Task<List<ValidationIssue>> AiValidationAsync(
        StoryBible bible,
        string storyText,
        string apiKey,
        string? model,
        CancellationToken ct)
    {
        var systemPrompt = @"You are a story consistency validator. Check if the story text matches the Story Bible.
Look for:
1. Character names used incorrectly
2. Character descriptions that contradict the Bible (wrong colors, wrong species)
3. Plot deviations from the Bible
4. Tone inconsistencies

OUTPUT FORMAT (JSON array):
[
  {
    ""type"": ""warning"" or ""error"",
    ""description"": ""what's wrong"",
    ""suggestedFix"": ""how to fix it"",
    ""location"": ""where in the story""
  }
]

If no issues found, return empty array: []";

        var userContent = $@"STORY BIBLE:
{JsonSerializer.Serialize(bible, JsonOptions)}

STORY TEXT:
{storyText}

Validate the story against the Bible. Return JSON array of issues.";

        var response = await _googleTextService.GenerateContentAsync(
            systemPrompt,
            userContent,
            apiKey,
            model,
            responseMimeType: "application/json",
            ct);

        try
        {
            var cleanJson = CleanJsonResponse(response);
            var aiIssues = JsonSerializer.Deserialize<List<ValidationIssue>>(cleanJson, JsonOptions);
            return aiIssues ?? [];
        }
        catch (JsonException ex)
        {
            _logger.LogWarning(ex, "Failed to parse AI validation response");
            return [];
        }
    }

    private static string CleanJsonResponse(string response)
    {
        var json = response.Trim();
        if (json.StartsWith("```json")) json = json[7..];
        else if (json.StartsWith("```")) json = json[3..];
        if (json.EndsWith("```")) json = json[..^3];
        return json.Trim();
    }
}
