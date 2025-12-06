namespace XooCreator.BA.Features.StoryEditor.Models;
    public record PublishResponse
    {
        public bool Ok { get; init; } = true;
        public string Status { get; init; } = "Published";
        public Guid? JobId { get; init; }
    }
