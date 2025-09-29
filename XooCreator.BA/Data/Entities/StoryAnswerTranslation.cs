
namespace XooCreator.BA.Data;

public class StoryAnswerTranslation
{
    public Guid Id { get; set; }
    public Guid StoryAnswerId { get; set; }
    public string LanguageCode { get; set; } = "ro-ro";
    public string Text { get; set; } = string.Empty;

    public StoryAnswer StoryAnswer { get; set; } = null!;
}
