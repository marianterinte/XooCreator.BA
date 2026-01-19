using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between StoryEpicCraft and StoryAgeGroup
/// </summary>
public class StoryEpicCraftAgeGroup
{
    public string StoryEpicCraftId { get; set; } = string.Empty;
    public Guid StoryAgeGroupId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicCraft StoryEpicCraft { get; set; } = null!;
    public StoryAgeGroup StoryAgeGroup { get; set; } = null!;
}
