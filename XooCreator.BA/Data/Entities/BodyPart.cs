namespace XooCreator.BA.Data;

public class BodyPart
{
    public string Key { get; set; } = string.Empty; // e.g., head
    public string Name { get; set; } = string.Empty; // e.g., Head
    public string Image { get; set; } = string.Empty; // e.g., images/bodyparts/face.webp
    public bool IsBaseLocked { get; set; }
    public List<BodyPartTranslation> Translations { get; set; } = new();
}

public class BodyPartTranslation
{
    public Guid Id { get; set; }
    public string BodyPartKey { get; set; } = string.Empty; // FK to BodyPart.Key
    public string LanguageCode { get; set; } = "ro-ro";
    public string Name { get; set; } = string.Empty;

    public BodyPart BodyPart { get; set; } = null!;
}
