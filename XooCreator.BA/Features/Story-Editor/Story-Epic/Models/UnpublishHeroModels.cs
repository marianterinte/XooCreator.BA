namespace XooCreator.BA.Features.StoryEditor.StoryEpic.Models;

public record UnpublishHeroRequest
{
    public string ConfirmHeroId { get; init; } = string.Empty;
    public string Reason { get; init; } = string.Empty;
}

public record UnpublishHeroResponse
{
    public bool Ok { get; init; } = true;
    public string Status { get; init; } = "unpublished";
}

