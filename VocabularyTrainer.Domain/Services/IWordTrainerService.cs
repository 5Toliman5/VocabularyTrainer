using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.Domain.Services
{
    public interface IWordTrainerService
    {
        int GetWordsCount();

		Task AddWordAsync(WordDto word);

        Task DeleteCurrentWordAsync();

        WordDto? GetCurrentWord();

        Task<WordDto?> GetNewWordAsync();

		Task LoadWordsAsync();

        void SetUser(UserModel user);

        Task UpdateCurrentWordAsync(UpdateWeightType updateWeightType);
    }
}