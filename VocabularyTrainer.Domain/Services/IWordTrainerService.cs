using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IWordTrainerService
	{
		int GetWordsCount();
		void SetUser(UserModel user);
		void SetScope(DictionaryScope scope);

		Task LoadWordsAsync();
		WordDto? GetCurrentWord();
		Task<WordDto?> GetNewWordAsync();

		Task<Result> AddWordAsync(AddWordRequest request);
		Task ReviewCurrentWordAsync(ReviewGrade grade);
		Task DeleteCurrentWordAsync();

		// Null when training scope is not a single dictionary (no single grade set).
		Task<IReadOnlyList<ReviewGrade>?> GetSupportedGradesAsync();
	}
}
