using System.Text;
using XooCreator.BA.Features.StoryEditor.Models;

namespace XooCreator.BA.Features.StoryEditor.GenerateFullStoryDraft;

internal static class StoryBibleTextPromptBuilder
{
    public static string BuildGoogleSystemInstruction(int numberOfPages, string languageCode, StoryBible bible)
    {
        var characterList = string.Join("\n", bible.Characters.Select(c =>
            $"- {c.Name}: {c.Visual.PrimaryColor} {c.Species}, {c.Visual.Size}. Features: {string.Join(", ", c.Visual.Features)}. Personality: {string.Join(", ", c.Personality)}"));

        var sb = new StringBuilder();
        sb.AppendLine("You are a children's story writer. Write a story based on the Story Bible below.");
        sb.AppendLine();
        sb.AppendLine("STORY BIBLE:");
        sb.AppendLine($"Title: {bible.Title}");
        sb.AppendLine($"Tone: {bible.Tone}");
        sb.AppendLine($"Setting: {bible.Setting.Place}, {bible.Setting.Time}");
        sb.AppendLine($"Plot: {bible.Plot.Problem} → {bible.Plot.Resolution}. Moral: {bible.Plot.Moral}");
        sb.AppendLine();
        sb.AppendLine("CHARACTERS (mention their colors/features naturally in the story):");
        sb.AppendLine(characterList);
        sb.AppendLine();
        sb.AppendLine("SCENE SKELETON:");
        for (var i = 0; i < bible.SceneSkeleton.Count && i < numberOfPages; i++)
        {
            sb.AppendLine($"- Page {i + 1}: {bible.SceneSkeleton[i]}");
        }
        sb.AppendLine();
        sb.AppendLine("OUTPUT FORMAT (strict):");
        sb.AppendLine("###T");
        sb.AppendLine("[Story title on one line]");
        sb.AppendLine("###S");
        sb.AppendLine("[Story summary in one short paragraph]");
        for (var i = 1; i <= numberOfPages; i++)
        {
            sb.AppendLine($"###P{i}");
            sb.AppendLine($"[Page {i} text - follow the scene skeleton]");
        }
        sb.AppendLine($"Language: {languageCode}. Generate exactly {numberOfPages} pages.");
        sb.AppendLine();
        sb.AppendLine("IMPORTANT: Naturally mention character visual details (colors, features) throughout the story.");
        return sb.ToString();
    }

    public static string BuildGoogleUserContent(StoryBible bible, string instructions, int numberOfPages)
    {
        var sb = new StringBuilder();
        sb.AppendLine($"Write the story '{bible.Title}' following the Story Bible.");
        if (!string.IsNullOrWhiteSpace(instructions))
        {
            sb.AppendLine();
            sb.AppendLine("Additional instructions:");
            sb.AppendLine(instructions);
        }
        sb.AppendLine();
        sb.AppendLine($"Generate exactly {numberOfPages} pages, one for each scene in the skeleton.");
        sb.AppendLine("Make sure to mention character visual details naturally in the narrative.");
        return sb.ToString();
    }
}
