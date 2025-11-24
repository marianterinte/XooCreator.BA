using XooCreator.BA.Data;
using XooCreator.BA.Data.Entities;

namespace XooCreator.BA.Features.StoryEditor.Services.Content;

public interface IStoryAnswerUpdater
{
    Task UpdateAnswersAsync(StoryCraftTile tile, List<EditableAnswerDto> answers, string languageCode, CancellationToken ct = default);
}

public class StoryAnswerUpdater : IStoryAnswerUpdater
{
    private readonly XooDbContext _context;

    public StoryAnswerUpdater(XooDbContext context)
    {
        _context = context;
    }

    public async Task UpdateAnswersAsync(StoryCraftTile tile, List<EditableAnswerDto> answers, string languageCode, CancellationToken ct = default)
    {
        var existingAnswers = tile.Answers.ToList();
        var answerDict = existingAnswers.ToDictionary(a => a.AnswerId);
        
        for (int i = 0; i < answers.Count; i++)
        {
            var answerDto = answers[i];
            var answer = answerDict.GetValueOrDefault(answerDto.Id);
            
            if (answer == null)
            {
                answer = new StoryCraftAnswer
                {
                    Id = Guid.NewGuid(),
                    StoryCraftTileId = tile.Id,
                    AnswerId = answerDto.Id,
                    SortOrder = i,
                    CreatedAt = DateTime.UtcNow
                };
                _context.StoryCraftAnswers.Add(answer);
                tile.Answers.Add(answer);
            }
            else
            {
                answer.SortOrder = i;
            }
            
            // Update or create answer translation
            var answerTranslation = answer.Translations.FirstOrDefault(t => t.LanguageCode == languageCode);
            if (answerTranslation == null)
            {
                answerTranslation = new StoryCraftAnswerTranslation
                {
                    Id = Guid.NewGuid(),
                    StoryCraftAnswerId = answer.Id,
                    LanguageCode = languageCode,
                    Text = answerDto.Text ?? string.Empty
                };
                _context.StoryCraftAnswerTranslations.Add(answerTranslation);
            }
            else
            {
                answerTranslation.Text = answerDto.Text ?? string.Empty;
            }
            
            // Update tokens (non-translatable)
            var existingTokens = answer.Tokens.ToList();
            _context.StoryCraftAnswerTokens.RemoveRange(existingTokens);
            
            foreach (var tokenDto in answerDto.Tokens ?? new())
            {
                var token = new StoryCraftAnswerToken
                {
                    Id = Guid.NewGuid(),
                    StoryCraftAnswerId = answer.Id,
                    Type = tokenDto.Type ?? "Personality",
                    Value = tokenDto.Value ?? string.Empty,
                    Quantity = tokenDto.Quantity
                };
                _context.StoryCraftAnswerTokens.Add(token);
            }
        }
        
        // Remove answers that are no longer in DTO
        var dtoAnswerIds = new HashSet<string>(answers.Select(a => a.Id));
        var answersToRemove = existingAnswers.Where(a => !dtoAnswerIds.Contains(a.AnswerId)).ToList();
        foreach (var answerToRemove in answersToRemove)
        {
            _context.StoryCraftAnswers.Remove(answerToRemove);
        }
    }
}
