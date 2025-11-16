namespace XooCreator.BA.Data;

/// <summary>
/// Represents user feedback for a story
/// </summary>
public class StoryFeedback
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty; // e.g., "root-s1" or "global"
    public string Email { get; set; } = string.Empty;
    public string FeedbackText { get; set; } = string.Empty; // Free text feedback
    
    // Questionnaire answers (stored as JSON array of strings)
    public List<string> WhatLiked { get; set; } = new(); // What user liked (multiple choice)
    public List<string> WhatDisliked { get; set; } = new(); // What user didn't like (multiple choice)
    public List<string> WhatCouldBeBetter { get; set; } = new(); // What could be improved (multiple choice)
    
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}

/// <summary>
/// Represents user preference for story feedback modal
/// </summary>
public class StoryFeedbackPreference
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string StoryId { get; set; } = string.Empty;
    public FeedbackPreferenceType PreferenceType { get; set; } // "later", "never", "submitted"
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime? UpdatedAt { get; set; }
    
    // Navigation
    public AlchimaliaUser User { get; set; } = null!;
}

public enum FeedbackPreferenceType
{
    Later = 0,      // User wants to give feedback later
    Never = 1,      // User doesn't want to give feedback
    Submitted = 2   // User has already submitted feedback
}

