namespace XooCreator.BA.Data;

public class BodyPart
{
    public string Key { get; set; } = string.Empty; // e.g., head
    public string Name { get; set; } = string.Empty; // e.g., Head
    public string Image { get; set; } = string.Empty; // e.g., images/bodyparts/face.webp
    public bool IsBaseLocked { get; set; }
}
