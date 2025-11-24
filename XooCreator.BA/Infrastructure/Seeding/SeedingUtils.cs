namespace XooCreator.BA.Infrastructure.Seeding;

public static class SeedingUtils
{
    /// <summary>
    /// Generates a URL-friendly ID from an author name by normalizing diacritics and special characters.
    /// </summary>
    public static string GenerateAuthorId(string authorName)
    {
        // Convert to lowercase, replace spaces and special characters with hyphens
        var authorId = authorName.ToLowerInvariant()
            .Replace(" ", "-")
            // Romanian diacritics
            .Replace("ă", "a")
            .Replace("â", "a")
            .Replace("î", "i")
            .Replace("ș", "s")
            .Replace("ț", "t")
            // Hungarian diacritics
            .Replace("á", "a")
            .Replace("é", "e")
            .Replace("í", "i")
            .Replace("ó", "o")
            .Replace("ö", "o")
            .Replace("ő", "o")
            .Replace("ú", "u")
            .Replace("ü", "u")
            .Replace("ű", "u")
            // Remove punctuation
            .Replace(".", "")
            .Replace(",", "")
            .Replace("'", "")
            .Replace("\"", "")
            .Replace("(", "")
            .Replace(")", "");
        
        // Remove multiple consecutive hyphens
        while (authorId.Contains("--"))
        {
            authorId = authorId.Replace("--", "-");
        }
        
        // Remove leading/trailing hyphens
        authorId = authorId.Trim('-');
        
        return authorId;
    }

    /// <summary>
    /// Generates a URL-friendly slug from any string.
    /// </summary>
    public static string GenerateSlug(string text)
    {
        return GenerateAuthorId(text); // Same logic for now
    }
}
