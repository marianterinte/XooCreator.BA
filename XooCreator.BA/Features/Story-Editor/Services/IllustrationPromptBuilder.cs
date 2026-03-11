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
        var parts = new List<string>
        {
            character.Name,
            character.Visual.PrimaryColor,
            character.Visual.Size,
            character.Species
        };

        if (character.Visual.Features.Count > 0)
        {
            parts.Add(string.Join(", ", character.Visual.Features));
        }

        if (!string.IsNullOrEmpty(character.Visual.SecondaryColor))
        {
            parts.Add($"with {character.Visual.SecondaryColor} accents");
        }

        return string.Join(" ", parts.Where(p => !string.IsNullOrWhiteSpace(p)));
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
        sb.AppendLine("CONSISTENCY RULES: Do not change character colors, sizes, or species. Maintain exact visual appearance as described above.");

        return sb.ToString();
    }
}
