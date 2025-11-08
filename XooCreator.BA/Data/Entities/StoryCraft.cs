using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data;

namespace XooCreator.BA.Data.Entities;

public class StoryCraft
{
    public Guid Id { get; set; }

    [MaxLength(200)]
    public required string StoryId { get; set; }

    public Guid OwnerUserId { get; set; }

    public LanguageCode Lang { get; set; } = LanguageCode.RoRo;

    [MaxLength(20)]
    public required string Status { get; set; } = "draft"; // draft | in_review | approved | published | archived

    public required string Json { get; set; } = "{}";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
}

