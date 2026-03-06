namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Single-row table for Welcome Flow config (entry point + next story per age group and gender).
/// Created by script V00122__add_welcome_flow_config.sql.
/// </summary>
public class WelcomeFlowConfig
{
    public int Id { get; set; }
    public string EntryPointStoryId { get; set; } = string.Empty;
    public string KindergartenGirl { get; set; } = string.Empty;
    public string KindergartenBoy { get; set; } = string.Empty;
    public string PrimaryGirl { get; set; } = string.Empty;
    public string PrimaryBoy { get; set; } = string.Empty;
    public string OlderGirl { get; set; } = string.Empty;
    public string OlderBoy { get; set; } = string.Empty;
}
