using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data.Entities;

public class StoryCraft
{
    public Guid Id { get; set; }

    [MaxLength(200)]
    public required string StoryId { get; set; }

    public Guid OwnerUserId { get; set; }

    public LanguageCode Lang { get; set; } = LanguageCode.RoRo;

    [MaxLength(20)]
    public required string Status { get; set; } = StoryStatus.Draft.ToDb(); // draft | in_review | approved | published | archived

    public required string Json { get; set; } = "{}";

    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Review workflow fields
    public Guid? AssignedReviewerUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ReviewEndedAt { get; set; }
}

