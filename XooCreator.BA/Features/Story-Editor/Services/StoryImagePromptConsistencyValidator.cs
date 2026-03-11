using System.Text;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Local prompt validator that patches missing character anchors and immutable rules.
/// </summary>
public sealed class StoryImagePromptConsistencyValidator : IStoryImagePromptConsistencyValidator
{
    public (string PromptText, int PatchedCount) ValidateAndPatch(
        string promptText,
        StoryBible bible,
        SceneDefinition? scene,
        IReadOnlyList<string> characterAnchors)
    {
        var patchedCount = 0;
        var sb = new StringBuilder(promptText);

        if (characterAnchors.Count > 0)
        {
            var missing = characterAnchors
                .Where(a => !promptText.Contains(a, StringComparison.OrdinalIgnoreCase))
                .ToList();

            if (missing.Count > 0)
            {
                sb.AppendLine();
                sb.AppendLine("Missing character anchors (patched):");
                foreach (var anchor in missing)
                {
                    sb.Append("- ").AppendLine(anchor);
                    patchedCount++;
                }
            }
        }

        if (!promptText.Contains("Do not change character colors", StringComparison.OrdinalIgnoreCase))
        {
            sb.AppendLine();
            sb.AppendLine("IMMUTABLE RULES:");
            sb.AppendLine("- Do not change character colors.");
            sb.AppendLine("- Do not change species.");
            sb.AppendLine("- Do not remove unique marker/accessory.");
            sb.AppendLine("- Maintain relative character sizes.");
            patchedCount++;
        }

        return (sb.ToString(), patchedCount);
    }
}

