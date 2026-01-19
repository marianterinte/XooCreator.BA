using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Many-to-many relationship between StoryEpicDefinition and StoryAgeGroup
/// </summary>
public class StoryEpicDefinitionAgeGroup
{
    public string StoryEpicDefinitionId { get; set; } = string.Empty;
    public Guid StoryAgeGroupId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryEpicDefinition StoryEpicDefinition { get; set; } = null!;
    public StoryAgeGroup StoryAgeGroup { get; set; } = null!;
}
