using System.Text;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Builds consistent illustration prompts from Story Bible and scene definitions.
/// No AI calls - purely local prompt construction.
/// </summary>
public sealed class IllustrationPromptBuilder : IIllustrationPromptBuilder
{
    public IllustrationPrompt Build(StoryBible bible, SceneDefinition scene)
    {
        var characterAnchors = BuildCharacterAnchors(bible, scene);
        var promptText = BuildPromptText(bible, scene, characterAnchors);

        return new IllustrationPrompt
        {
            SceneId = scene.SceneId,
            PromptText = promptText,
            StyleNotes = bible.VisualStyle,
            CharacterAnchors = characterAnchors,
            NegativePrompt = "text, words, letters, watermark, signature, blurry, distorted"
        };
    }

    public List<IllustrationPrompt> BuildAll(StoryBible bible, List<SceneDefinition> scenes)
    {
        return scenes.Select(scene => Build(bible, scene)).ToList();
    }

    public string GetPromptText(IllustrationPrompt prompt)
    {
        return prompt.PromptText;
    }

    private static List<string> BuildCharacterAnchors(StoryBible bible, SceneDefinition scene)
    {
        var anchors = new List<string>();

        foreach (var characterId in scene.CharactersPresent)
        {
            var character = bible.Characters.FirstOrDefault(c => 
                c.Id.Equals(characterId, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Equals(characterId, StringComparison.OrdinalIgnoreCase));

            if (character != null)
            {
                anchors.Add(FormatCharacterDescription(character));
            }
        }

        // If no characters specified in scene, include main character
        if (anchors.Count == 0)
        {
            var mainCharacter = bible.Characters.FirstOrDefault(c => c.Role == "main") 
                ?? bible.Characters.FirstOrDefault();
            
            if (mainCharacter != null)
            {
                anchors.Add(FormatCharacterDescription(mainCharacter));
            }
        }

        return anchors;
    }

    private static string FormatCharacterDescription(CharacterProfile character)
    {
        var primaryColor = ToUpperInvariantSafe(character.Visual.PrimaryColor);
        var secondaryColor = ToUpperInvariantSafe(character.Visual.SecondaryColor);
        var accessories = character.Visual.Accessories
            .Where(a => !string.IsNullOrWhiteSpace(a))
            .Select(ToUpperInvariantSafe)
            .ToList();
        var features = character.Visual.Features
            .Where(f => !string.IsNullOrWhiteSpace(f))
            .ToList();

        var sb = new StringBuilder();
        sb.Append($"CHARACTER \"{character.Name}\" = {primaryColor} {character.Visual.Size} {character.Species}.");
        if (features.Count > 0)
        {
            sb.Append($" Features: {string.Join(", ", features)}.");
        }
        if (accessories.Count > 0)
        {
            sb.Append($" Unique marker: {string.Join(", ", accessories)}.");
        }
        if (!string.IsNullOrWhiteSpace(secondaryColor))
        {
            sb.Append($" Accent color: {secondaryColor}.");
        }
        sb.Append($" {character.Name} is ALWAYS {primaryColor}.");

        return sb.ToString();
    }

    private static string BuildPromptText(
        StoryBible bible,
        SceneDefinition scene,
        List<string> characterAnchors)
    {
        var sb = new StringBuilder();

        // Core style
        sb.AppendLine("Children's book illustration. Colorful, friendly. No text. Portrait, vertical composition, 4:5 aspect ratio.");
        
        // Visual style from Bible
        sb.AppendLine($"Visual style: {bible.VisualStyle}");
        
        // Setting
        sb.AppendLine($"Setting: {bible.Setting.Place} during {bible.Setting.Time}");
        sb.AppendLine($"Environment: {scene.Environment}");
        
        // Characters - THE MOST IMPORTANT PART FOR CONSISTENCY
        sb.AppendLine();
        sb.AppendLine("Characters in scene (maintain EXACT appearance throughout the story):");
        foreach (var anchor in characterAnchors)
        {
            sb.AppendLine($"- {anchor}");
        }
        
        // Scene specifics
        sb.AppendLine();
        sb.AppendLine($"Scene: {scene.VisualFocus}");
        sb.AppendLine($"Emotional tone: {scene.Emotion}");
        
        // Anti-drift rules
        sb.AppendLine();
        sb.AppendLine("CONSISTENCY RULES:");
        sb.AppendLine("- Do not change character colors, sizes, or species.");
        sb.AppendLine("- Do not remove or alter unique markers/accessories.");
        sb.AppendLine("- Keep recurring characters visually identical across pages 1-10.");
        sb.AppendLine("- If user explicitly described a trait, preserve it exactly.");
        AppendAntiSwapRules(sb, bible, scene);

        return sb.ToString();
    }

    private static void AppendAntiSwapRules(StringBuilder sb, StoryBible bible, SceneDefinition scene)
    {
        var presentCharacters = scene.CharactersPresent
            .Select(id => bible.Characters.FirstOrDefault(c =>
                c.Id.Equals(id, StringComparison.OrdinalIgnoreCase) ||
                c.Name.Equals(id, StringComparison.OrdinalIgnoreCase)))
            .Where(c => c != null)
            .Cast<CharacterProfile>()
            .ToList();

        var sameSpeciesGroups = presentCharacters
            .GroupBy(c => c.Species, StringComparer.OrdinalIgnoreCase)
            .Where(g => g.Count() > 1)
            .ToList();

        if (sameSpeciesGroups.Count == 0)
            return;

        sb.AppendLine();
        sb.AppendLine("ANTI-SWAP RULES (MANDATORY):");
        foreach (var group in sameSpeciesGroups)
        {
            foreach (var character in group)
            {
                var primaryColor = ToUpperInvariantSafe(character.Visual.PrimaryColor);
                var accessories = character.Visual.Accessories
                    .Where(a => !string.IsNullOrWhiteSpace(a))
                    .Select(ToUpperInvariantSafe)
                    .ToList();
                var marker = accessories.Count > 0
                    ? string.Join(", ", accessories)
                    : "NO ACCESSORY";
                sb.AppendLine($"- {character.Name} is ALWAYS the {primaryColor} {character.Species} with {marker}.");
            }
            sb.AppendLine("- NEVER interchange colors, accessories, or identities between these same-species characters.");
        }
    }

    private static string ToUpperInvariantSafe(string? value)
    {
        return (value ?? string.Empty).Trim().ToUpperInvariant();
    }
}
