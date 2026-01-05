using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace XooCreator.BA.Features.StoryEditor.Services;

internal static class HashHelper
{
    public static string ComputeHash(object data)
    {
        var json = JsonSerializer.Serialize(data);
        using var sha = SHA256.Create();
        var hashBytes = sha.ComputeHash(Encoding.UTF8.GetBytes(json));
        return Convert.ToHexString(hashBytes);
    }
}

