using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

/// <summary>
/// Version snapshot for Animal
/// </summary>
public class AnimalVersion
{
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    public int Version { get; set; }
    
    [MaxLength(50)]
    public string Status { get; set; } = string.Empty;
    public Guid? CreatedByUserId { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public string? SnapshotJson { get; set; } // JSON snapshot al versiunii

    public Animal Animal { get; set; } = null!;
}
