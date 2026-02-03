using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Co-author of a story (published). Either UserId (Alchimalia user) or DisplayName (free text).
/// </summary>
public class StoryDefinitionCoAuthor
{
    public Guid Id { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public Guid? UserId { get; set; }
    public string? DisplayName { get; set; }
    public int SortOrder { get; set; }

    public StoryDefinition StoryDefinition { get; set; } = null!;
    public AlchimaliaUser? User { get; set; }
}
