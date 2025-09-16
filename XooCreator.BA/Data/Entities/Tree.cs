namespace XooCreator.BA.Data;

public class Tree
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public UserAlchimalia User { get; set; } = null!;
    public string RootType { get; set; } = string.Empty; // color or type
    public string? Location { get; set; }
    public int CurrentTier { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public ICollection<TreeChoice> Choices { get; set; } = new List<TreeChoice>();
}
