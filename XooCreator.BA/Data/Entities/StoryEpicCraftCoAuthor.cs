namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Co-author of an epic (draft). Either UserId (Alchimalia user) or DisplayName (free text).
/// </summary>
public class StoryEpicCraftCoAuthor
{
    public Guid Id { get; set; }
    public string StoryEpicCraftId { get; set; } = string.Empty;
    public Guid? UserId { get; set; }
    public string? DisplayName { get; set; }
    public int SortOrder { get; set; }

    public StoryEpicCraft StoryEpicCraft { get; set; } = null!;
    public AlchimaliaUser? User { get; set; }
}
