using XooCreator.BA.Features.TreeOfLight;

namespace XooCreator.BA.Data;

public class StoryAnswerToken
{
    public Guid Id { get; set; }
    public Guid StoryAnswerId { get; set; }

    // Store as string in DB for simplicity; map to TokenFamily in code as needed
    public required string Type { get; set; } = TokenFamily.Personality.ToString();
    public required string Value { get; set; } = string.Empty;
    public required int Quantity { get; set; }

    // Navigation
    public StoryAnswer StoryAnswer { get; set; } = null!;
}


