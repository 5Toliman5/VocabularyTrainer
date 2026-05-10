using Common.Wrappers;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
	public class ApiTrainingService(ITrainingService trainingService) : IApiTrainingService
	{
		public Task<Result<List<WordDto>>> GetCandidatesAsync(int userId, DictionaryScope scope, int limit)
			=> trainingService.GetCandidatesAsync(userId, scope, limit);

		public Task<Result> ApplyReviewAsync(ReviewWordRequest request)
			=> trainingService.ApplyReviewAsync(request);

		public Task<Result<IReadOnlyList<ReviewGrade>>> GetSupportedGradesAsync(int dictionaryId, int userId)
			=> trainingService.GetSupportedGradesAsync(dictionaryId, userId);
	}
}

