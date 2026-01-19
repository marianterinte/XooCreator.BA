using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between StoryEpicDefinition and StoryTopic
/// </summary>
public class StoryEpicDefinitionTopic
{
    public string StoryEpicDefinitionId { get; set; } = string.Empty;
    public Guid StoryTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicDefinition StoryEpicDefinition { get; set; } = null!;
    public StoryTopic StoryTopic { get; set; } = null!;
}
