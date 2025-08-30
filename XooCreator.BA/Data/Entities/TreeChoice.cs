namespace XooCreator.BA.Data;

public class TreeChoice
{
    public Guid Id { get; set; }
    public Guid TreeId { get; set; }
    public Tree Tree { get; set; } = null!;
    public int Tier { get; set; }
    public string BranchType { get; set; } = string.Empty; // thin|thick|balanced
    public string TraitAwarded { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
