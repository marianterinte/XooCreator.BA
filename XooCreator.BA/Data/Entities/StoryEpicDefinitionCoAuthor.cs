namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Co-author of an epic (published). Either UserId (Alchimalia user) or DisplayName (free text).
/// </summary>
public class StoryEpicDefinitionCoAuthor
{
    public Guid Id { get; set; }
    public string StoryEpicDefinitionId { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? DisplayName { get; set; }
    public int SortOrder { get; set; }

    public StoryEpicDefinition StoryEpicDefinition { get; set; } = null!;
    public AlchimaliaUser? User { get; set; }
}
