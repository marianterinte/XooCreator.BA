namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Raised when user input indicates content harmful for children.
/// </summary>
public sealed class ChildSafetyPolicyViolationException : Exception
{
    public const string DefaultUiMessage = "Nu putem genera povești care au conținut dăunător pentru copii.";

    public ChildSafetyPolicyViolationException(string? message = null)
        : base(string.IsNullOrWhiteSpace(message) ? DefaultUiMessage : message)
    {
    }
}
