using System.ComponentModel.DataAnnotations;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;

namespace XooCreator.BA.Data;

/// <summary>
/// Represents a story definition in the system
/// </summary>
public class StoryDefinition
{
    public Guid Id { get; set; }
    public string StoryId { get; set; } = string.Empty; // e.g., "root-s1", "intro-pufpuf"
    public string Title { get; set; } = string.Empty;
    public string? CoverImageUrl { get; set; }
    public string? Summary { get; set; }
    public string? StoryTopic { get; set; } // e.g., "main", "intro", "special" - topic/theme of the story
    public string? AuthorName { get; set; } // Name of the author/writer if the story has an author (for "Other" option)
    public Guid? ClassicAuthorId { get; set; } // Reference to ClassicAuthor if a classic author is selected
    public StoryType StoryType { get; set; } = StoryType.AlchimaliaEpic; // Type of story (Epic vs Indie)
    public StoryStatus Status { get; set; } = StoryStatus.Published; // Publication status
    public int SortOrder { get; set; }
    public bool IsActive { get; set; } = true;
    public int Version { get; set; } = 1; // Global version, increments on publish
    public int LastPublishedVersion { get; set; } = 0; // Draft version that produced the current publish
    public double PriceInCredits { get; set; } = 0; // Price in credits for purchasing the story
    public bool IsEvaluative { get; set; } = false; // If true, this story contains quizzes that should be evaluated
    public bool IsPartOfEpic { get; set; } = false; // If true, this story is part of an epic (draft or published) and should not appear as independent story
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public Guid? CreatedBy { get; set; } // User who created the story
    public Guid? UpdatedBy { get; set; } // User who last updated the story
    
    // Navigation
    public List<StoryTile> Tiles { get; set; } = new();
    public List<StoryDefinitionTranslation> Translations { get; set; } = new();
    public List<StoryDefinitionTopic> Topics { get; set; } = new();
    public List<StoryDefinitionAgeGroup> AgeGroups { get; set; } = new();
    public List<StoryDefinitionCoAuthor> CoAuthors { get; set; } = new();
    public List<StoryDefinitionUnlockedHero> UnlockedHeroes { get; set; } = new();
    public ClassicAuthor? ClassicAuthor { get; set; }
}

public class StoryDefinitionTranslation
{
    public Guid Id { get; set; }
    public Guid StoryDefinitionId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro"; // normalized lower-case
    public string Title { get; set; } = string.Empty;

    public StoryDefinition StoryDefinition { get; set; } = null!;
}

/// <summary>
/// Many-to-many relationship between StoryDefinition and Unlocked Heroes (stored as string IDs)
/// Similar to StoryCraftUnlockedHero but for published stories
/// </summary>
public class StoryDefinitionUnlockedHero
{
    public Guid StoryDefinitionId { get; set; }
    
    [MaxLength(100)]
    public required string HeroId { get; set; } // e.g., "puf-puf", "linkaro", "grubot"
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    public StoryDefinition StoryDefinition { get; set; } = null!;
}
