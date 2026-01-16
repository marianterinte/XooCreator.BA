using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

/// <summary>
/// Version snapshot for HeroDefinition
/// </summary>
public class HeroDefinitionVersion
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public string HeroDefinitionId { get; set; } = string.Empty;
    public int Version { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? SnapshotJson { get; set; } // JSON snapshot al versiunii

    public HeroDefinition HeroDefinition { get; set; } = null!;
}
