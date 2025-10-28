namespace XooCreator.BA.Data.Enums;

/// <summary>
/// Represents the publication status of a story
/// </summary>
public enum StoryStatus
{
    /// <summary>
    /// Story is in draft state, not yet published
    /// </summary>
    Draft = 0,
    
    /// <summary>
    /// Story is published and available to users
    /// </summary>
    Published = 1,
    
    /// <summary>
    /// Story has been retracted/unpublished
    /// </summary>
    Retreated = 2
}
