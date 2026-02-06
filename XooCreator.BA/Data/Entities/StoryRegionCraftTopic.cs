using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between StoryRegionCraft and StoryTopic
/// </summary>
public class StoryRegionCraftTopic
{
    public string StoryRegionCraftId { get; set; } = string.Empty;
    public Guid StoryTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StoryRegionCraft StoryRegionCraft { get; set; } = null!;
    public StoryTopic StoryTopic { get; set; } = null!;
}
