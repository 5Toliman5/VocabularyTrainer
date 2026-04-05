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
		private int? _currentDictionaryId;

		public WordTrainerService(int maxWeight, IWordRepository repository, IWordsShuffleService wordsShuffleService)
		{
			_repository = repository;
			_words = [];
			_maxWeight = maxWeight;
			_wordsShuffleService = wordsShuffleService;
		}

		public void SetUser(UserModel user)
		{
			_currentUser = user;
			_currentDictionaryId = null;
			_words.Clear();
			_currentWord = null;
		}

		public void SetDictionary(int? dictionaryId)
		{
			if (_currentDictionaryId == dictionaryId) return;
			_currentDictionaryId = dictionaryId;
			_words.Clear();
			_currentWord = null;
		}

		public int GetWordsCount() => _words.Count;

		public async Task LoadWordsAsync()
		{
			var dbWords = await _repository.GetAllAsync(CurrentUserId, _currentDictionaryId);
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

			return _currentWord;
		}

		public async Task AddWordAsync(WordDto word, int dictionaryId)
		{
			if (_currentUser is null) throw new InvalidOperationException("Current user is not set.");

			var existingWord = GetExistingWord(word, dictionaryId);
			if (existingWord is not null)
			{
				_words.Remove(existingWord);
				await _repository.DeleteAsync(new EditWordRequest(existingWord.Id, CurrentUserId, existingWord.DictionaryId));
			}

			await _repository.AddAsync(new AddWordRequest(new Word(word.Value, word.Translation), CurrentUserId, dictionaryId));

			if (_currentDictionaryId is null || _currentDictionaryId == dictionaryId)
				_words.Clear();
		}

		public async Task UpdateCurrentWordAsync(UpdateWeightType updateWeightType)
		{
			var word = _currentWord;
			if (!NeedToUpdateWordWeight(updateWeightType, word)) return;

			await _repository.UpdateWeightAsync(new UpdateWordWeightRequest(word!.Id, CurrentUserId, word.DictionaryId, updateWeightType));
		}

		public async Task DeleteCurrentWordAsync()
		{
			if (_currentWord is null) return;

			_words.Remove(_currentWord);

			await _repository.DeleteAsync(new EditWordRequest(_currentWord.Id, CurrentUserId, _currentWord.DictionaryId));

			_currentWord = null;
		}

		private WordDto? GetExistingWord(WordDto newWord, int dictionaryId)
		{
			return _words.FirstOrDefault(x =>
				x.Value.Equals(newWord.Value, StringComparison.CurrentCultureIgnoreCase) &&
				x.DictionaryId == dictionaryId);
		}

		private bool NeedToUpdateWordWeight(UpdateWeightType updateWeightType, WordDto? word)
		{
			if (word is null) return false;

			return updateWeightType switch
			{
				UpdateWeightType.Increase => word.Weight < _maxWeight,
				UpdateWeightType.Decrease => word.Weight > 0,
				_ => throw new ArgumentException($"{updateWeightType} is not valid")
			};
		}

		private int CurrentUserId => _currentUser?.Id ?? throw new InvalidOperationException("Current user is not set.");
	}
}
