namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Models;

public record UnpublishStoryEpicRequest
{
    public string ConfirmEpicId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public record UnpublishStoryEpicResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "unpublished";
}


