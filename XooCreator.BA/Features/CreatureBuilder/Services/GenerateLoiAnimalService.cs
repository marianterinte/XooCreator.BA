using System.Text.Json;
using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;
using XooCreator.BA.Features.CreatureBuilder;
using XooCreator.BA.Features.User.DTOs;
using XooCreator.BA.Features.User.Services;
using XooCreator.BA.Infrastructure.Services.Blob;

namespace XooCreator.BA.Features.CreatureBuilder.Services;

public sealed class GenerateLoiAnimalService : IGenerateLoiAnimalService
{
    private readonly XooDbContext _db;
    private readonly IUserProfileService _userProfile;
    private readonly IBlobSasService _blobSas;

    private static readonly byte[] PlaceholderPng = Convert.FromBase64String(
        "iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8z8BQDwAEhQGAhKmMIQAAAABJRU5ErkJggg=="); // 1x1 pixel

    public GenerateLoiAnimalService(XooDbContext db, IUserProfileService userProfile, IBlobSasService blobSas)
    {
        _db = db;
        _userProfile = userProfile;
        _blobSas = blobSas;
    }

    public async Task<GenerateGenerativeAnimalResponseDto> GenerateAsync(Guid userId, GenerateGenerativeAnimalRequestDto request, CancellationToken ct = default)
    {
        var spendResult = await _userProfile.SpendGenerativeCreditsAsync(userId, new SpendCreditsRequest { Amount = 1, Reference = "loi-generation" });
        if (!spendResult.Success)
            return new GenerateGenerativeAnimalResponseDto(false, null, null, null, null, spendResult.ErrorMessage ?? "Insufficient generative credits", spendResult.NewBalance);

        var id = Guid.NewGuid();
        var combination = request.Combination;
        var head = combination.Head?.Trim() ?? "—";
        var body = combination.Body?.Trim() ?? "—";
        var arms = combination.Arms?.Trim() ?? "—";
        var name = $"{NormalizePart(head)}-{NormalizePart(body)}-{NormalizePart(arms)}";
        var storyText = $"Un animal magic născut din imaginația ta: cap de {head}, trup de {body}, membre de {arms}. Acesta este creatura ta unică din Laboratorul Imaginației.";

        var blobPath = $"loi-generative/{userId:N}/{id}.png";
        var container = _blobSas.DraftContainer;
        await EnsureContainerAndUploadAsync(container, blobPath, PlaceholderPng, "image/png", ct);

        var partsSnapshot = JsonSerializer.Serialize(new { head, body, arms });

        var entity = new UserGeneratedLoiAnimal
        {
            Id = id,
            UserId = userId,
            ImageBlobPath = blobPath,
            StoryText = storyText,
            PartsSnapshotJson = partsSnapshot,
            Name = name,
            CreatedAtUtc = DateTime.UtcNow
        };
        _db.UserGeneratedLoiAnimals.Add(entity);
        await _db.SaveChangesAsync(ct);

        var readSas = await _blobSas.GetReadSasAsync(container, blobPath, TimeSpan.FromHours(1), ct);
        var imageUrl = readSas.ToString();

        return new GenerateGenerativeAnimalResponseDto(
            true,
            id,
            name,
            imageUrl,
            storyText,
            null,
            spendResult.NewBalance
        );
    }

    private static string NormalizePart(string part) => part == "—" ? "None" : part;

    private async Task EnsureContainerAndUploadAsync(string container, string blobPath, byte[] data, string contentType, CancellationToken ct)
    {
        var client = _blobSas.GetBlobClient(container, blobPath);
        await using var stream = new MemoryStream(data);
        await client.UploadAsync(stream, overwrite: true, ct);
    }
}
