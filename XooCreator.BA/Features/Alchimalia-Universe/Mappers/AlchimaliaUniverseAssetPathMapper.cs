using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.AlchimaliaUniverse.DTOs;

namespace XooCreator.BA.Features.AlchimaliaUniverse.Mappers;

public static class AlchimaliaUniverseAssetPathMapper
{
    public record AssetInfo(string Filename, AssetType Type, string? Lang);

    public enum AssetType
    {
        Image,
        Audio
    }

    public enum EntityType
    {
        Hero,
        Animal
    }

    public static List<AssetInfo> CollectFromHeroCraft(HeroDefinitionCraft craft)
    {
         var results = new List<AssetInfo>();
         if (!string.IsNullOrWhiteSpace(craft.Image))
         {
             var imageRef = craft.Image.StartsWith("draft/", StringComparison.OrdinalIgnoreCase)
                 ? craft.Image
                 : Path.GetFileName(craft.Image);
             results.Add(new AssetInfo(imageRef, AssetType.Image, null));
         }
         
         // AudioUrl in translations?
         foreach (var t in craft.Translations)
         {
             if (!string.IsNullOrWhiteSpace(t.AudioUrl))
             {
                 var audioRef = t.AudioUrl.StartsWith("draft/", StringComparison.OrdinalIgnoreCase)
                     ? t.AudioUrl
                     : Path.GetFileName(t.AudioUrl);
                 results.Add(new AssetInfo(audioRef, AssetType.Audio, t.LanguageCode));
             }
         }
         
         return results;
    }

    public static List<AssetInfo> CollectFromAnimalCraft(AnimalCraft craft)
    {
        var results = new List<AssetInfo>();
        if (!string.IsNullOrWhiteSpace(craft.Src))
        {
             var imageRef = craft.Src.StartsWith("draft/", StringComparison.OrdinalIgnoreCase)
                 ? craft.Src
                 : Path.GetFileName(craft.Src);
             results.Add(new AssetInfo(imageRef, AssetType.Image, null));
        }

        foreach (var t in craft.Translations)
        {
             if (!string.IsNullOrWhiteSpace(t.AudioUrl))
             {
                 var audioRef = t.AudioUrl.StartsWith("draft/", StringComparison.OrdinalIgnoreCase)
                     ? t.AudioUrl
                     : Path.GetFileName(t.AudioUrl);
                 results.Add(new AssetInfo(audioRef, AssetType.Audio, t.LanguageCode));
             }
        }
        
        return results;
    }

    public static string BuildDraftPath(AssetInfo asset, string userEmail, string entityId, EntityType entityType)
    {
        if (!string.IsNullOrWhiteSpace(asset.Filename) &&
            asset.Filename.StartsWith("draft/", StringComparison.OrdinalIgnoreCase))
        {
            return asset.Filename.TrimStart('/');
        }

        var emailEsc = Uri.EscapeDataString(userEmail);
        var typeSegment = entityType == EntityType.Hero ? "heroes" : "animals";
        var basePath = $"draft/u/{emailEsc}/alchimalia-universe/{typeSegment}/{entityId}";

        if (asset.Type == AssetType.Image)
        {
            return $"{basePath}/{asset.Filename}";
        }

        if (!string.IsNullOrWhiteSpace(asset.Lang))
        {
            return $"{basePath}/{asset.Lang}/{asset.Filename}";
        }

        return $"{basePath}/{asset.Filename}";
    }

    public static string BuildPublishedPath(AssetInfo asset, string userEmail, string entityId, EntityType entityType)
    {
        // Structure: 
        // images/alchimalia-universe/{heroes|animals}/{userEmail}/{entityId}/{filename}
        // audio/alchimalia-universe/{heroes|animals}/{userEmail}/{entityId}/{lang}/{filename}
        
        var category = asset.Type == AssetType.Audio ? "audio" : "images";
        var typeSegment = entityType == EntityType.Hero ? "heroes" : "animals";
        var sanitizedEmail = SanitizeEmailForFolder(userEmail);
        
        var basePath = $"{category}/alchimalia-universe/{typeSegment}/{sanitizedEmail}/{entityId}";

        if (asset.Type == AssetType.Audio && !string.IsNullOrWhiteSpace(asset.Lang))
        {
            basePath = $"{basePath}/{asset.Lang}";
        }

        var filename = Path.GetFileName(asset.Filename);
        return $"{basePath}/{filename}";
    }

    public static string SanitizeEmailForFolder(string email)
    {
        if (string.IsNullOrWhiteSpace(email)) return "unknown";

        var trimmed = email.Trim().ToLowerInvariant();
        var chars = trimmed.Select(ch =>
            char.IsLetterOrDigit(ch) || ch == '.' || ch == '_' || ch == '-' || ch == '@' ? ch : '-'
        ).ToArray();

        var cleaned = new string(chars);
        while (cleaned.Contains("--"))
        {
            cleaned = cleaned.Replace("--", "-");
        }

        return cleaned.Trim('-');
    }
}
