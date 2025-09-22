using System.ComponentModel.DataAnnotations;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents a hero definition in the Tree of Heroes
/// Contains the static definition of heroes (costs, prerequisites, rewards)
/// </summary>
public class HeroDefinition
{
    [Key]
    public string Id { get; set; } = string.Empty;
    
    [Required]
    public string Name { get; set; } = string.Empty;
    
    public string Description { get; set; } = string.Empty;
    
    [Required]
    public string Type { get; set; } = string.Empty; // 'node', 'hero'
    
    // Token costs
    public int CourageCost { get; set; }
    public int CuriosityCost { get; set; }
    public int ThinkingCost { get; set; }
    public int CreativityCost { get; set; }
    public int SafetyCost { get; set; }
    
    // JSON strings for complex data
    public string PrerequisitesJson { get; set; } = string.Empty; // JSON array of prerequisite IDs
    public string RewardsJson { get; set; } = string.Empty; // JSON array of reward strings
    
    // Position data for rendering (prevents overlaps)
    public double PositionX { get; set; } = 0.0;
    public double PositionY { get; set; } = 0.0;
    
    // Metadata
    public bool IsUnlocked { get; set; } = false; // Default state
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}
