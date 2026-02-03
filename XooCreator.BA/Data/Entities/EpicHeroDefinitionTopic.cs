using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between EpicHeroDefinition and StoryTopic
/// </summary>
public class EpicHeroDefinitionTopic
{
    public string EpicHeroDefinitionId { get; set; } = string.Empty;
    public Guid StoryTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public EpicHeroDefinition EpicHeroDefinition { get; set; } = null!;
    public StoryTopic StoryTopic { get; set; } = null!;
}
