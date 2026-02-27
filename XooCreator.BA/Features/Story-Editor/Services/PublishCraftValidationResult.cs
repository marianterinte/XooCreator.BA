namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Single validation failure with entity context for copyable diagnostics.
/// </summary>
public sealed record PublishCraftValidationFailure(
    string Entity,
    string Key,
    string ConstraintType,
    string Message);

/// <summary>
/// Result of pre-publish craft validation. Use for fail-fast with actionable diagnostics.
/// </summary>
public sealed class PublishCraftValidationResult
{
    private const int MaxDiagnosticMessageLength = 1900;

    public bool IsValid => Failures.Count == 0;
    public IReadOnlyList<PublishCraftValidationFailure> Failures { get; }

    private readonly List<PublishCraftValidationFailure> _failures = new();

    private PublishCraftValidationResult()
    {
        Failures = _failures;
    }

    public static PublishCraftValidationResult Valid() => new();

    public static PublishCraftValidationResult Create() => new();

    public void Add(string entity, string key, string constraintType, string message)
    {
        _failures.Add(new PublishCraftValidationFailure(entity, key, constraintType, message));
    }

    /// <summary>
    /// Compact, copyable message for job ErrorMessage (max 2000 chars in DB).
    /// </summary>
    public string ToDiagnosticMessage()
    {
        if (IsValid) return string.Empty;
        var lines = _failures.Select(f => $"[{f.Entity}|{f.Key}] {f.ConstraintType}: {f.Message}");
        var joined = string.Join("; ", lines);
        return joined.Length > MaxDiagnosticMessageLength ? joined[..MaxDiagnosticMessageLength] + "…" : joined;
    }
}
