using Microsoft.EntityFrameworkCore;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.StoryEditor.Services;

public interface IStoryIdGenerator
{
    Task<string> GenerateNextAsync(Guid ownerUserId, string firstName, string lastName, CancellationToken ct);
}

/// <summary>
/// Centralized logic for generating unique storyId per owner.
/// Uses pattern {firstname}{lastname}-s{yymmddhhmmss} to avoid exposing email addresses
/// while maintaining uniqueness through timestamp.
/// </summary>
public class StoryIdGenerator : IStoryIdGenerator
{
    private readonly XooDbContext _db;
    private readonly ILogger<StoryIdGenerator> _logger;

    public StoryIdGenerator(XooDbContext db, ILogger<StoryIdGenerator> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task<string> GenerateNextAsync(Guid ownerUserId, string firstName, string lastName, CancellationToken ct)
    {
        if (ownerUserId == Guid.Empty)
        {
            throw new ArgumentException("Owner id is required", nameof(ownerUserId));
        }

        if (string.IsNullOrWhiteSpace(firstName) && string.IsNullOrWhiteSpace(lastName))
        {
            throw new ArgumentException("At least first name or last name is required", nameof(firstName));
        }

        // Sanitize names: remove diacritics, spaces, special chars, convert to lowercase
        var sanitizedFirstName = SanitizeName(firstName ?? string.Empty);
        var sanitizedLastName = SanitizeName(lastName ?? string.Empty);
        
        // Combine names (if both are empty after sanitization, use "user" as fallback)
        var namePart = string.IsNullOrWhiteSpace(sanitizedFirstName) && string.IsNullOrWhiteSpace(sanitizedLastName)
            ? "user"
            : $"{sanitizedFirstName}{sanitizedLastName}";

        // Generate timestamp: yymmddhhmmss (12 digits)
        var now = DateTime.UtcNow;
        var timestamp = now.ToString("yyMMddHHmmss", CultureInfo.InvariantCulture);

        // Check for collisions (very unlikely but possible)
        var baseId = $"{namePart}-s{timestamp}";
        var finalId = await EnsureUniqueIdAsync(baseId, ownerUserId, ct);

        _logger.LogDebug("Generated storyId {StoryId} for owner {OwnerId} (firstName={FirstName}, lastName={LastName})", 
            finalId, ownerUserId, firstName, lastName);
        return finalId;
    }

    /// <summary>
    /// Sanitizes a name by:
    /// - Removing diacritics (ă -> a, î -> i, etc.)
    /// - Removing spaces and special characters
    /// - Converting to lowercase
    /// - Limiting length to prevent overly long IDs
    /// </summary>
    private static string SanitizeName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        // Remove diacritics
        var normalized = name.Normalize(NormalizationForm.FormD);
        var builder = new StringBuilder();
        foreach (var c in normalized)
        {
            var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
            if (unicodeCategory != UnicodeCategory.NonSpacingMark)
            {
                builder.Append(c);
            }
        }
        var withoutDiacritics = builder.ToString().Normalize(NormalizationForm.FormC);

        // Remove spaces, special characters, keep only alphanumeric
        var sanitized = Regex.Replace(withoutDiacritics, @"[^a-zA-Z0-9]", string.Empty, RegexOptions.None, TimeSpan.FromMilliseconds(100));

        // Convert to lowercase and limit length (max 20 chars per name part)
        return sanitized.ToLowerInvariant().Substring(0, Math.Min(sanitized.Length, 20));
    }

    /// <summary>
    /// Ensures the generated ID is unique. If collision occurs (extremely rare),
    /// appends milliseconds or a small counter.
    /// </summary>
    private async Task<string> EnsureUniqueIdAsync(string baseId, Guid ownerUserId, CancellationToken ct)
    {
        // Check if ID exists in crafts or published stories
        var existsInCrafts = await _db.StoryCrafts
            .AnyAsync(c => c.StoryId == baseId, ct);

        var existsInPublished = await _db.StoryDefinitions
            .AnyAsync(d => d.StoryId == baseId, ct);

        if (!existsInCrafts && !existsInPublished)
        {
            return baseId;
        }

        // Collision detected - append milliseconds
        var now = DateTime.UtcNow;
        var timestampWithMs = now.ToString("yyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var namePart = baseId.Split("-s")[0];
        var candidateId = $"{namePart}-s{timestampWithMs}";

        // Double-check the new candidate
        var stillExists = await _db.StoryCrafts
            .AnyAsync(c => c.StoryId == candidateId, ct) ||
            await _db.StoryDefinitions
            .AnyAsync(d => d.StoryId == candidateId, ct);

        if (!stillExists)
        {
            _logger.LogWarning("StoryId collision detected for {BaseId}, using {CandidateId} instead", baseId, candidateId);
            return candidateId;
        }

        // If still collision (extremely rare), append a random suffix
        var randomSuffix = new Random().Next(100, 999);
        var finalCandidate = $"{namePart}-s{timestampWithMs}{randomSuffix}";
        _logger.LogWarning("Multiple StoryId collisions detected, using {FinalCandidate}", finalCandidate);
        return finalCandidate;
    }
}

