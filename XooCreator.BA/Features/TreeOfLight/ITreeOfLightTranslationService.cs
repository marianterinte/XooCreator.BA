
using XooCreator.BA.Data;

namespace XooCreator.BA.Features.TreeOfLight.Services;

public interface ITreeOfLightTranslationService
{
    Task<Dictionary<string, string>> GetTranslationsAsync(string locale);
}
