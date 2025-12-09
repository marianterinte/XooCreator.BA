namespace XooCreator.BA.Data;

public class PlatformSetting
{
    public string Key { get; set; } = string.Empty;
    public bool BoolValue { get; set; }
    public string? StringValue { get; set; }
    public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    public string? UpdatedBy { get; set; }
}

