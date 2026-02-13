namespace XooCreator.BA.Features.StoryEditor.Services.Content;

public record StoryCoAuthorDto
{
    public Guid? UserId { get; init; }
    public string DisplayName { get; init; } = string.Empty;
}

public class EditableStoryDto
{
    public string Id { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string CoverImageUrl { get; set; } = string.Empty;
    public string? Summary { get; set; }
    public string? StoryTopic { get; set; } // DEPRECATED: Use TopicIds instead. Kept for backward compatibility.
    public List<string>? TopicIds { get; set; } // List of topic IDs (e.g., ["edu_math", "fun_adventure"])
    public List<string>? AgeGroupIds { get; set; } // List of age group IDs (e.g., ["preschool_3_5", "early_school_6_8"])
    public string? AuthorName { get; set; } // Name of the author/writer if the story has an author (for "Other" option)
    public Guid? ClassicAuthorId { get; set; } // Reference to ClassicAuthor if a classic author is selected
    public double PriceInCredits { get; set; } = 0; // Price in credits for purchasing the story
    public int StoryType { get; set; } = 0; // 0 = AlchimaliaEpic (Tree Of Light), 1 = Indie (Independent)
    public bool IsEvaluative { get; set; } = false; // If true, this story contains quizzes that should be evaluated
    public bool IsPartOfEpic { get; set; } = false; // If true, this story is part of an epic (draft or published) and should not appear as independent story
    public string? Status { get; set; } // 'draft' | 'in-review' | 'approved' | 'published' (FE semantic)
    public string? Language { get; set; } // Language code for the story (standardized: use "language" instead of "languageCode")
    public List<string>? AvailableLanguages { get; set; } // Available language codes for this story
    public List<string>? AudioLanguages { get; set; } // Languages that have audio support
    public List<string>? UnlockedStoryHeroes { get; set; } // List of hero IDs that are unlocked when this story is completed
    public List<string>? DialogParticipants { get; set; } // Hero IDs available as story dialog participants
    public List<StoryCoAuthorDto>? CoAuthors { get; set; } // Co-authors (user or free text)
    public List<EditableTileDto> Tiles { get; set; } = new();

    // Reviewer/Audit fields (optional)
    public Guid? AssignedReviewerUserId { get; set; }
    public Guid? ReviewedByUserId { get; set; }
    public Guid? ApprovedByUserId { get; set; }
    public string? ReviewNotes { get; set; }
    public DateTime? ReviewStartedAt { get; set; }
    public DateTime? ReviewEndedAt { get; set; }

    // Version reference
    public int? BaseVersion { get; set; }
    
    // Owner email (for admin editing another user's draft)
    public string? OwnerEmail { get; set; }
}

public class EditableTileDto
{
    public string Type { get; set; } = "page";
    public string Id { get; set; } = string.Empty;
    public string? BranchId { get; set; }
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? Question { get; set; }
    public List<EditableAnswerDto> Answers { get; set; } = new();
    public string? DialogRootNodeId { get; set; }
    public List<EditableDialogNodeDto> DialogNodes { get; set; } = new();
    // Character selection specific: selected heroes for this tile (subset of dialogParticipants)
    public List<string>? AvailableHeroIds { get; set; }
}

public class EditableDialogNodeDto
{
    public string NodeId { get; set; } = string.Empty;
    public string SpeakerType { get; set; } = "reader"; // reader | hero
    public string? SpeakerHeroId { get; set; }
    public string Text { get; set; } = string.Empty;
    public List<EditableDialogOptionDto> Options { get; set; } = new();
    /// <summary>Saved X position for tree rendering in editor and reading mode.</summary>
    public int? X { get; set; }
    /// <summary>Saved Y position for tree rendering in editor and reading mode.</summary>
    public int? Y { get; set; }
}

public class EditableDialogOptionDto
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public string NextNodeId { get; set; } = string.Empty;
    public string? JumpToTileId { get; set; }
    public string? SetBranchId { get; set; }
    public List<EditableTokenDto> Tokens { get; set; } = new();
}

public class EditableAnswerDto
{
    public string Id { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
    public bool IsCorrect { get; set; } = false; // True if this is the correct answer for the quiz
    public List<EditableTokenDto> Tokens { get; set; } = new();
}

public class EditableTokenDto
{
    public string? Type { get; set; }
    public string? Value { get; set; }
    public int Quantity { get; set; }
}
