using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Api.BusinessLogic.Services.Abstractions
{
    public interface IApiTrainingService
    {
        Task<Result<List<WordDto>>> GetCandidatesAsync(int userId, DictionaryScope scope, int limit);
        Task<Result> ApplyReviewAsync(ReviewWordRequest request);
        Task<Result<IReadOnlyList<ReviewGrade>>> GetSupportedGradesAsync(int dictionaryId, int userId);
    }
}
