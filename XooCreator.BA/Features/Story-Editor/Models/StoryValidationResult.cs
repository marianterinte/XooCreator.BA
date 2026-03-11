namespace XooCreator.BA.Features.StoryEditor.Models;

/// <summary>
/// Result of validating story text against the Story Bible.
/// </summary>
public sealed class StoryValidationResult
{
    public required bool IsValid { get; init; }
    public List<string> Warnings { get; init; } = [];
    public List<string> Errors { get; init; } = [];
    public List<ValidationIssue> Issues { get; init; } = [];
}

/// <summary>
/// A specific validation issue found.
/// </summary>
public sealed class ValidationIssue
{
    public required string Type { get; init; }
    public required string Description { get; init; }
    public string? SuggestedFix { get; init; }
    public string? Location { get; init; }
}
