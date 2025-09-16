namespace XooCreator.BA.Data;

public class UserAlchimalia
{
    public Guid Id { get; set; }
    public string Auth0Sub { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
