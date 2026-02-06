using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between StoryRegionDefinition and StoryTopic
/// </summary>
public class StoryRegionDefinitionTopic
{
    public string StoryRegionDefinitionId { get; set; } = string.Empty;
    public Guid StoryTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public StoryRegionDefinition StoryRegionDefinition { get; set; } = null!;
    public StoryTopic StoryTopic { get; set; } = null!;
}
