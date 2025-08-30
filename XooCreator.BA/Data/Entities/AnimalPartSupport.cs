namespace XooCreator.BA.Data;

public class AnimalPartSupport
{
    public Guid AnimalId { get; set; }
    public Animal Animal { get; set; } = null!;

    public string PartKey { get; set; } = string.Empty;
    public BodyPart Part { get; set; } = null!;
}
