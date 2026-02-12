using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.TreeOfLight;
using XooCreator.BA.Features.TreeOfLight.DTOs;

namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Represents a story craft (draft/working copy) in the editor
/// Similar to StoryDefinition but for drafts/editor working copies
/// </summary>
public class StoryCraft
{
    public Guid Id { get; set; }

    [MaxLength(200)]
    public required string StoryId { get; set; }

    public Guid OwnerUserId { get; set; }

    [MaxLength(20)]
    public required string Status { get; set; } = StoryStatus.Draft.ToDb(); // draft | in_review | approved | published | archived

    public string? CoverImageUrl { get; set; }
    
    public string? StoryTopic { get; set; } // e.g., "Matematică", "Literatură" - topic/theme of the story
    
    [MaxLength(200)]
    public string? AuthorName { get; set; } // Name of the author/writer if the story has an author (for "Other" option)
    public Guid? ClassicAuthorId { get; set; } // Reference to ClassicAuthor if a classic author is selected
    
    public StoryType StoryType { get; set; } = StoryType.AlchimaliaEpic; // Type of story (Epic vs Indie)
    
    public double PriceInCredits { get; set; } = 0; // Price in credits for purchasing the story
    public bool IsEvaluative { get; set; } = false; // If true, this story contains quizzes that should be evaluated
    public bool IsPartOfEpic { get; set; } = false; // If true, this story is part of an epic (draft or published) and should not appear as independent story
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

    // Review workflow fields
    public Guid? AssignedReviewerUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ReviewEndedAt { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }

    // Versioning reference: the published version from which this draft originated
    public int BaseVersion { get; set; } = 0;
    
    // Incremental counter for changes performed in the editor (consumed by delta publish)
    public int LastDraftVersion { get; set; } = 0;
    
    // Navigation
    public List<StoryCraftTranslation> Translations { get; set; } = new();
    public List<StoryCraftTile> Tiles { get; set; } = new();
    public List<StoryCraftTopic> Topics { get; set; } = new();
    public List<StoryCraftAgeGroup> AgeGroups { get; set; } = new();
    public List<StoryCraftCoAuthor> CoAuthors { get; set; } = new();
    public List<StoryCraftUnlockedHero> UnlockedHeroes { get; set; } = new();
    public List<StoryCraftDialogParticipant> DialogParticipants { get; set; } = new();
    public ClassicAuthor? ClassicAuthor { get; set; }
}

/// <summary>
/// Many-to-many relationship between StoryCraft and Unlocked Heroes (stored as string IDs)
/// </summary>
public class StoryCraftUnlockedHero
{
    public Guid StoryCraftId { get; set; }
    
    [MaxLength(100)]
    public required string HeroId { get; set; } // e.g., "puf-puf", "linkaro", "grubot"
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryCraft StoryCraft { get; set; } = null!;
}

/// <summary>
/// Translation for StoryCraft (Title, Summary per language)
/// </summary>
public class StoryCraftTranslation
{
    public Guid Id { get; set; }
    public Guid StoryCraftId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case
    public string Title { get; set; } = string.Empty;
    public string? Summary { get; set; }

    public StoryCraft StoryCraft { get; set; } = null!;
}

/// <summary>
/// Represents a tile (page or quiz) within a story craft
/// </summary>
public class StoryCraftTile
{
    public Guid Id { get; set; }
    public Guid StoryCraftId { get; set; }
    public string TileId { get; set; } = string.Empty; // e.g., "p1", "q1"
    public string Type { get; set; } = string.Empty; // "page", "quiz", or "video"
    public int SortOrder { get; set; }
    [MaxLength(100)]
    public string? BranchId { get; set; }
    
    // Non-translatable fields (same for all languages)
    // Image is common for all languages
    public string? ImageUrl { get; set; }
    // Audio and Video are now language-specific (moved to StoryCraftTileTranslation)
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public StoryCraft StoryCraft { get; set; } = null!;
    public List<StoryCraftAnswer> Answers { get; set; } = new();
    public List<StoryCraftTileTranslation> Translations { get; set; } = new();
    public StoryCraftDialogTile? DialogTile { get; set; }
}

/// <summary>
/// Translation for StoryCraftTile (Caption, Text, Question, Audio, Video per language)
/// </summary>
public class StoryCraftTileTranslation
{
    public Guid Id { get; set; }
    public Guid StoryCraftTileId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro";
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? Question { get; set; }
    
    // Language-specific media (filename only in draft)
    public string? AudioUrl { get; set; }
    public string? VideoUrl { get; set; }

    public StoryCraftTile StoryCraftTile { get; set; } = null!;
}

/// <summary>
/// Represents a quiz answer option in a story craft
/// </summary>
public class StoryCraftAnswer
{
    public Guid Id { get; set; }
    public Guid StoryCraftTileId { get; set; }
    public string AnswerId { get; set; } = string.Empty; // e.g., "a", "b", "c"
    public bool IsCorrect { get; set; } = false; // True if this is the correct answer for the quiz
    public int SortOrder { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public StoryCraftTile StoryCraftTile { get; set; } = null!;
    public List<StoryCraftAnswerToken> Tokens { get; set; } = new();
    public List<StoryCraftAnswerTranslation> Translations { get; set; } = new();
}

/// <summary>
/// Translation for StoryCraftAnswer (Text per language)
/// </summary>
public class StoryCraftAnswerTranslation
{
    public Guid Id { get; set; }
    public Guid StoryCraftAnswerId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro";
    public string Text { get; set; } = string.Empty;

    public StoryCraftAnswer StoryCraftAnswer { get; set; } = null!;
}

/// <summary>
/// Token for StoryCraftAnswer (non-translatable, same for all languages)
/// </summary>
public class StoryCraftAnswerToken
{
    public Guid Id { get; set; }
    public Guid StoryCraftAnswerId { get; set; }

    public required string Type { get; set; } = TokenFamily.Personality.ToString();
    public required string Value { get; set; } = string.Empty;
    public required int Quantity { get; set; }

    // Navigation
    public StoryCraftAnswer StoryCraftAnswer { get; set; } = null!;
}
