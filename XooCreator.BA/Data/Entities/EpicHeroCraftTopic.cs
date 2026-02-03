using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between EpicHeroCraft and StoryTopic
/// </summary>
public class EpicHeroCraftTopic
{
    public string EpicHeroCraftId { get; set; } = string.Empty;
    public Guid StoryTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EpicHeroCraft EpicHeroCraft { get; set; } = null!;
    public StoryTopic StoryTopic { get; set; } = null!;
}
