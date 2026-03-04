namespace XooCreator.BA.Features.Stories.Configuration;

/// <summary>
/// Configuration for the welcome flow: entry point story and next-story IDs per age group and gender.
/// Bound from the "WelcomeFlow" section in appsettings.*.json.
/// </summary>
public sealed class WelcomeFlowOptions
{
    public const string SectionName = "WelcomeFlow";

    public string EntryPointStoryId { get; set; } = string.Empty;

    /// <summary>Age group: kindergarten (e.g. 3-5 years).</summary>
    public WelcomeFlowAgeGroupOptions Kindergarten { get; set; } = new();
    /// <summary>Age group: primary (e.g. 6-9 years).</summary>
    public WelcomeFlowAgeGroupOptions Primary { get; set; } = new();
    /// <summary>Age group: older (e.g. 10+ years).</summary>
    public WelcomeFlowAgeGroupOptions Older { get; set; } = new();

    /// <summary>
    /// Returns all allowed story IDs for public welcome access (entry + all next-story IDs from kindergarten/primary/older).
    /// </summary>
    public IEnumerable<string> GetAllowedStoryIds()
    {
        var ids = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        if (!string.IsNullOrWhiteSpace(EntryPointStoryId))
            ids.Add(EntryPointStoryId.Trim());
        AddFromAgeGroup(ids, Kindergarten);
        AddFromAgeGroup(ids, Primary);
        AddFromAgeGroup(ids, Older);
        return ids;
    }

    /// <summary>
    /// Default story ID when no query param is provided (e.g. first kindergarten story).
    /// </summary>
    public string GetDefaultStoryId()
    {
        if (!string.IsNullOrWhiteSpace(Kindergarten.Boy)) return Kindergarten.Boy.Trim();
        if (!string.IsNullOrWhiteSpace(Kindergarten.Girl)) return Kindergarten.Girl.Trim();
        if (!string.IsNullOrWhiteSpace(EntryPointStoryId)) return EntryPointStoryId.Trim();
        return string.Empty;
    }

    private static void AddFromAgeGroup(HashSet<string> ids, WelcomeFlowAgeGroupOptions? group)
    {
        if (group == null) return;
        if (!string.IsNullOrWhiteSpace(group.Girl)) ids.Add(group.Girl.Trim());
        if (!string.IsNullOrWhiteSpace(group.Boy)) ids.Add(group.Boy.Trim());
    }
}

public sealed class WelcomeFlowAgeGroupOptions
{
    public string Girl { get; set; } = string.Empty;
    public string Boy { get; set; } = string.Empty;
}
