namespace XooCreator.BA.Features.StoryEditor.Services.Content;

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
    public List<string>? UnlockedStoryHeroes { get; set; } // List of hero IDs that are unlocked when this story is completed
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
}

public class EditableTileDto
{
    public string Type { get; set; } = "page";
    public string Id { get; set; } = string.Empty;
    public string? Caption { get; set; }
    public string? Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public string? VideoUrl { get; set; }
    public string? Question { get; set; }
    public List<EditableAnswerDto> Answers { get; set; } = new();
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
