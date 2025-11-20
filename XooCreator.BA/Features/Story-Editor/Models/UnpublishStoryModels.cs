namespace XooCreator.BA.Features.StoryEditor.Models;

public record UnpublishStoryRequest
{
    public string ConfirmStoryId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public record UnpublishStoryResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "archived";
}

