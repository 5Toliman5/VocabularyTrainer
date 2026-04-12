using Common.Extensions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	/// <summary>Implements word training session business logic.</summary>
	public class WordTrainerService : IWordTrainerService
	{
		private readonly IWordRepository _repository;
		private readonly IWordsShuffleService _wordsShuffleService;
		private readonly int _maxWeight;

		private TrainingSession? _session;
		private TrainingSession CurrentSession => _session ?? throw new InvalidOperationException("Current user is not set.");

		/// <summary>Initializes a new instance with the specified weight cap, repository, and shuffle strategy.</summary>
		public WordTrainerService(int maxWeight, IWordRepository repository, IWordsShuffleService wordsShuffleService)
		{
			_repository = repository;
			_maxWeight = maxWeight;
			_wordsShuffleService = wordsShuffleService;
		}

		/// <inheritdoc/>
		public void SetUser(UserModel user)
		{
			_session = new TrainingSession(user);
		}

		/// <inheritdoc/>
		public void SetDictionary(int? dictionaryId)
		{
			var session = CurrentSession;

			if (session.DictionaryId == dictionaryId) return;

			session.DictionaryId = dictionaryId;
			session.Words.Clear();
			session.CurrentWord = null;
		}

        /// <inheritdoc/>
        public int GetWordsCount()
        {
            return CurrentSession.Words.Count;
        }

        /// <inheritdoc/>
        public async Task LoadWordsAsync()
		{
			var session = CurrentSession;
			var dbWords = await _repository.GetAllAsync(session.User.Id, session.DictionaryId);
			var shuffledWords = _wordsShuffleService.Shuffle(dbWords);
			session.Words.Clear();
			session.Words.AddRange(shuffledWords);
		}

		/// <inheritdoc/>
		public WordDto? GetCurrentWord()
		{
			var currentWord = CurrentSession.CurrentWord;
			return currentWord is null
				? null
				: new WordDto(currentWord.Value, currentWord.Translation);
		}

		/// <inheritdoc/>
		public async Task<WordDto?> GetNewWordAsync()
		{
			var session = CurrentSession;

			if (session.CurrentWord is not null) session.Words.Remove(session.CurrentWord);

			if (session.Words.IsNullOrEmpty()) await LoadWordsAsync();

			if (session.Words.IsNullOrEmpty()) return null;

			session.CurrentWord = session.Words[0];

			return session.CurrentWord;
		}

		/// <inheritdoc/>
		public async Task AddWordAsync(WordDto word, int dictionaryId)
		{
			var session = CurrentSession;

			var existingWord = GetExistingWord(session, word, dictionaryId);
			if (existingWord is not null)
			{
				session.Words.Remove(existingWord);
				await _repository.DeleteAsync(new UserWordKey(existingWord.Id, session.User.Id, existingWord.DictionaryId));
			}

			await _repository.AddAsync(new AddWordRequest(word.Value, word.Translation, session.User.Id, dictionaryId));

			if (session.DictionaryId is null || session.DictionaryId == dictionaryId)
				session.Words.Clear();
		}

		/// <inheritdoc/>
		public async Task UpdateCurrentWordAsync(UpdateWeightType updateWeightType)
		{
			var session = CurrentSession;
			var word = session.CurrentWord;
			if (!NeedToUpdateWordWeight(updateWeightType, word)) return;

			await _repository.UpdateWeightAsync(new UpdateWordWeightRequest(word!.Id, session.User.Id, word.DictionaryId, updateWeightType));
		}

		/// <inheritdoc/>
		public async Task DeleteCurrentWordAsync()
		{
			var session = CurrentSession;
			if (session.CurrentWord is null) return;

			session.Words.Remove(session.CurrentWord);

			await _repository.DeleteAsync(new UserWordKey(session.CurrentWord.Id, session.User.Id, session.CurrentWord.DictionaryId));

			session.CurrentWord = null;
		}

		private static WordDto? GetExistingWord(TrainingSession session, WordDto newWord, int dictionaryId)
		{
			return session.Words.FirstOrDefault(x =>
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
	}
}
