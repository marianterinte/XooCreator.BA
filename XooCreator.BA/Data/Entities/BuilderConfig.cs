namespace XooCreator.BA.Data;

public class BuilderConfig
{
    public int Id { get; set; }
    
    /// <summary>
    /// JSON array of animal IDs that are unlocked by default for new users
    /// </summary>
    public string BaseUnlockedAnimalIds { get; set; } = "[]";
    
    /// <summary>
    /// JSON array of body part keys that are unlocked by default for new users
    /// </summary>
    public string BaseUnlockedBodyPartKeys { get; set; } = "[]";
}
