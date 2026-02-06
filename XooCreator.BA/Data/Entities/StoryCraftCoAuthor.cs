namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Co-author of a story (draft). Either UserId (Alchimalia user) or DisplayName (free text).
/// </summary>
public class StoryCraftCoAuthor
{
    public Guid Id { get; set; }
    public Guid StoryCraftId { get; set; }
    public Guid? UserId { get; set; }
    public string? DisplayName { get; set; }
    public int SortOrder { get; set; }

    public StoryCraft StoryCraft { get; set; } = null!;
    public AlchimaliaUser? User { get; set; }
}
