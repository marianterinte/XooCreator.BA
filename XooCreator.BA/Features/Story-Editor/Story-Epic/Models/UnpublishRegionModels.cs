namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Models;

public record UnpublishRegionRequest
{
    public string ConfirmRegionId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public record UnpublishRegionResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "unpublished";
}

