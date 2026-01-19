using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between StoryEpicCraft and StoryTopic
/// </summary>
public class StoryEpicCraftTopic
{
    public string StoryEpicCraftId { get; set; } = string.Empty;
    public Guid StoryTopicId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicCraft StoryEpicCraft { get; set; } = null!;
    public StoryTopic StoryTopic { get; set; } = null!;
}
