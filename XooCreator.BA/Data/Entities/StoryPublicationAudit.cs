using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data.Entities;

public class StoryPublicationAudit
{
    public Guid Id { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public Guid PerformedByUserId { get; set; }
    public string PerformedByEmail { get; set; } = string.Empty;
    public StoryPublicationAction Action { get; set; }
    public string? Notes { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StoryDefinition StoryDefinition { get; set; } = null!;
}

