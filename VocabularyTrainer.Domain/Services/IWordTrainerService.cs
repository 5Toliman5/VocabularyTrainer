using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
	public interface IWordTrainerService
	{
		int GetWordsCount();
		void SetUser(UserModel user);
		// Returns false, without touching the current word/session, when scope is unchanged.
		bool SetScope(DictionaryScope scope);

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
