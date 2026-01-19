namespace XooCreator.BA.Data;

public class AnimalHybridCraftPart
{
    public Guid Id { get; set; }
    public Guid AnimalCraftId { get; set; }
    public Guid SourceAnimalId { get; set; }
    public string BodyPartKey { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public AnimalCraft AnimalCraft { get; set; } = null!;
    public AnimalDefinition SourceAnimal { get; set; } = null!;
    public BodyPart BodyPart { get; set; } = null!;
}
