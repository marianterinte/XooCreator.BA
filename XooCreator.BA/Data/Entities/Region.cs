namespace XooCreator.BA.Data;

public class Region
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty; // e.g., Sahara, Jungle, Farm

    public ICollection<Animal> Animals { get; set; } = new List<Animal>();
}
