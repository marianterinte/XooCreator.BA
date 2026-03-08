namespace XooCreator.BA.Data.Entities;

/// <summary>
/// Configurable bundle of exclusive story/epic IDs per Supporter Pack plan (07).
/// One row per plan; lists are JSON arrays of string IDs. Configurable from admin tab Supporter Packs.
/// </summary>
public class SupporterPackPlanExclusive
{
    public string PlanId { get; set; } = string.Empty;
    /// <summary>JSON array of story IDs (e.g. ["story-martin-salata"]).</summary>
    public string? ExclusiveStoryIdsJson { get; set; }
    /// <summary>JSON array of epic IDs (e.g. ["epic-pufpuf-sylvaria"]).</summary>
    public string? ExclusiveEpicIdsJson { get; set; }
}
