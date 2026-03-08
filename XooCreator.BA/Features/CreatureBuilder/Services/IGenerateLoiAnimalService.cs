using XooCreator.BA.Features.CreatureBuilder;

namespace XooCreator.BA.Features.CreatureBuilder.Services;

public interface IGenerateLoiAnimalService
{
    /// <summary>Spends 1 generative credit, generates (placeholder or AI) image + story, saves and returns the result.</summary>
    Task<GenerateGenerativeAnimalResponseDto> GenerateAsync(Guid userId, GenerateGenerativeAnimalRequestDto request, CancellationToken ct = default);
}
