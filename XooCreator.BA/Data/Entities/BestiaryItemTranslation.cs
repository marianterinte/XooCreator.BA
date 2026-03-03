namespace XooCreator.BA.Data;

public class BestiaryItemTranslation
{
    public Guid Id { get; set; }
    public Guid BestiaryItemId { get; set; }
    public BestiaryItem BestiaryItem { get; set; } = null!;
    public string LanguageCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Story { get; set; } = string.Empty;
}
