using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
    public class WordTrainerService : IWordTrainerService
	{
		private readonly IWordRepository _repository;
		private readonly IWordsShuffleService _wordsShuffleService;

		private readonly List<WordDto> _words;
		private readonly int _maxWeight;

        private WordDto? _currentWord;
        private UserModel? _currentUser;

		public WordTrainerService(int maxWeight, IWordRepository repository, IWordsShuffleService wordsShuffleService)
		{
			_repository = repository;
			_words = [];
			_maxWeight = maxWeight;
			_wordsShuffleService = wordsShuffleService;
		}

		public void SetUser(UserModel user) => _currentUser = user;

		public int GetWordsCount() => _words.Count;

		public async Task LoadWordsAsync()
		{
			var dbWords = await _repository.GetAllAsync(CurrentUserId);
			var shuffledWords = _wordsShuffleService.Shuffle(dbWords);
			_words.AddRange(shuffledWords);
		}

		public WordDto? GetCurrentWord()
		{
			return _currentWord is null
				? null
				: new WordDto(_currentWord.Value, _currentWord.Translation);
		}

		public async Task<WordDto?> GetNewWordAsync()
		{
			if (_currentWord is not null) _words.Remove(_currentWord);

			if (_words.Count == 0) await LoadWordsAsync();

			if (_words.Count == 0) return null;

			_currentWord = _words[0];

			return new WordDto(_currentWord.Value, _currentWord.Translation);
		}

		public async Task AddWordAsync(WordDto word)
		{
			if (_currentUser is null) throw new InvalidOperationException("Current user is not set.");

			var existingWord = GetExistingWord(word);

			if (existingWord is not null)
			{
				_words.Remove(existingWord);
				await _repository.DeleteAsync(new EditWordRequest(existingWord.Id, CurrentUserId));
			}

			var newWord = new WordDto(word.Value, word.Translation);
			_words.Add(newWord);

			await _repository.AddAsync(new AddWordRequest(new Word(word.Value, word.Translation), CurrentUserId));
		}

		public async Task UpdateCurrentWordAsync(UpdateWeightType updateWeightType)
		{
			if (!NeedToUpdateWordWeight(updateWeightType)) return;

			await _repository.UpdateWeightAsync(new UpdateWordWeightRequest(_currentWord!.Id, CurrentUserId, updateWeightType));
		}

		public async Task DeleteCurrentWordAsync()
		{
			if (_currentWord is null) return;

			_words.Remove(_currentWord);

			await _repository.DeleteAsync(new EditWordRequest(_currentWord.Id, CurrentUserId));

			_currentWord = null;
		}

		private WordDto? GetExistingWord(WordDto newWord)
		{
			return _words.FirstOrDefault(x => x.Value.Equals(newWord.Value, StringComparison.CurrentCultureIgnoreCase));
		}

		private bool NeedToUpdateWordWeight(UpdateWeightType updateWeightType)
		{
			if (_currentWord is null) return false;

			return updateWeightType switch
			{
				UpdateWeightType.Increase => _currentWord.Weight < _maxWeight,
				UpdateWeightType.Decrease => _currentWord.Weight > 0,
				_ => throw new ArgumentException($"{updateWeightType} is not valid")
			};
		}

        private int CurrentUserId => _currentUser?.Id ?? throw new InvalidOperationException("Current user is not set.");
    }
}
