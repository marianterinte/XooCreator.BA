using Microsoft.EntityFrameworkCore;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Data.Enums;
using XooCreator.BA.Features.StoryEditor.Services.Content;

namespace XooCreator.BA.Features.StoryEditor.Services;

/// <summary>
/// Creates a private StoryDefinition + tiles from an EditableStoryDto (e.g. after AI generation with published paths).
/// </summary>
public interface IPrivateStoryCreationService
{
    Task<Guid> CreateFromDtoAsync(
        EditableStoryDto dto,
        string storyId,
        Guid ownerUserId,
        string ownerEmail,
        string languageCode,
        CancellationToken ct = default);
}

public class PrivateStoryCreationService : IPrivateStoryCreationService
{
    private readonly XooDbContext _db;

    public PrivateStoryCreationService(XooDbContext db)
    {
        _db = db;
    }

    public async Task<Guid> CreateFromDtoAsync(
        EditableStoryDto dto,
        string storyId,
        Guid ownerUserId,
        string ownerEmail,
        string languageCode,
        CancellationToken ct = default)
    {
        var lang = (languageCode ?? "ro-ro").Trim().ToLowerInvariant();
        var def = new StoryDefinition
        {
            Id = Guid.NewGuid(),
            StoryId = storyId,
            Title = dto.Title ?? "My Story",
            CoverImageUrl = string.IsNullOrWhiteSpace(dto.CoverImageUrl) ? null : dto.CoverImageUrl,
            Summary = dto.Summary,
            StoryType = StoryType.Indie,
            Status = StoryStatus.Published,
            IsActive = true,
            IsPrivate = true,
            Version = 1,
            SortOrder = 0,
            PriceInCredits = 0,
            CreatedBy = ownerUserId,
            UpdatedBy = ownerUserId,
            AudioLanguages = new List<string> { lang }
        };
        _db.StoryDefinitions.Add(def);

        _db.StoryDefinitionTranslations.Add(new StoryDefinitionTranslation
        {
            Id = Guid.NewGuid(),
            StoryDefinitionId = def.Id,
            LanguageCode = lang,
            Title = def.Title
        });

        var sortOrder = 0;
        foreach (var tileDto in dto.Tiles ?? new List<EditableTileDto>())
        {
            var tile = new StoryTile
            {
                Id = Guid.NewGuid(),
                StoryDefinitionId = def.Id,
                TileId = tileDto.Id ?? $"p{sortOrder + 1}",
                Type = string.IsNullOrWhiteSpace(tileDto.Type) ? "page" : tileDto.Type,
                SortOrder = sortOrder++,
                Text = tileDto.Text,
                ImageUrl = string.IsNullOrWhiteSpace(tileDto.ImageUrl) ? null : tileDto.ImageUrl,
                Caption = tileDto.Caption
            };
            _db.StoryTiles.Add(tile);

            _db.StoryTileTranslations.Add(new StoryTileTranslation
            {
                Id = Guid.NewGuid(),
                StoryTileId = tile.Id,
                LanguageCode = lang,
                Text = tileDto.Text,
                Caption = tileDto.Caption,
                AudioUrl = string.IsNullOrWhiteSpace(tileDto.AudioUrl) ? null : tileDto.AudioUrl
            });
        }

        _db.UserCreatedStories.Add(new UserCreatedStories
        {
            Id = Guid.NewGuid(),
            UserId = ownerUserId,
            StoryDefinitionId = def.Id,
            CreatedAt = def.CreatedAt,
            PublishedAt = def.CreatedAt,
            IsPublished = true,
            CreationNotes = "Private story (your-story)"
        });

        await _db.SaveChangesAsync(ct);
        return def.Id;
    }
}
