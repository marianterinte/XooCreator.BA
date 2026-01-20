namespace XooCreator.BA.Data;

public class AnimalHybridDefinitionPart
{
    public Guid Id { get; set; }
    public Guid AnimalDefinitionId { get; set; }
    public Guid SourceAnimalId { get; set; }
    public string BodyPartKey { get; set; } = string.Empty;
    public int OrderIndex { get; set; }

    public AnimalDefinition AnimalDefinition { get; set; } = null!;
    public AnimalDefinition SourceAnimal { get; set; } = null!;
    public BodyPart BodyPart { get; set; } = null!;
}
