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
             results.Add(new AssetInfo(Path.GetFileName(craft.Image), AssetType.Image, null));
         }
         
         // AudioUrl in translations?
         foreach (var t in craft.Translations)
         {
             if (!string.IsNullOrWhiteSpace(t.AudioUrl))
             {
                 results.Add(new AssetInfo(Path.GetFileName(t.AudioUrl), AssetType.Audio, t.LanguageCode));
             }
         }
         
         return results;
    }

    public static List<AssetInfo> CollectFromAnimalCraft(AnimalCraft craft)
    {
        var results = new List<AssetInfo>();
        if (!string.IsNullOrWhiteSpace(craft.Src))
        {
             results.Add(new AssetInfo(Path.GetFileName(craft.Src), AssetType.Image, null));
        }

        foreach (var t in craft.Translations)
        {
             if (!string.IsNullOrWhiteSpace(t.AudioUrl))
             {
                 results.Add(new AssetInfo(Path.GetFileName(t.AudioUrl), AssetType.Audio, t.LanguageCode));
             }
        }
        
        return results;
    }

    public static string BuildDraftPath(AssetInfo asset, string userEmail, string entityId, EntityType entityType)
    {
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

        return $"{basePath}/{asset.Filename}";
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
