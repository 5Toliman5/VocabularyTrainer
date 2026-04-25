using Common.Extensions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class WordTrainerService : IWordTrainerService
	{
		private readonly IWordRepository _repository;
		private readonly IWordsShuffleService _wordsShuffleService;

		private TrainingSession? _session;
		private TrainingSession CurrentSession => _session ?? throw new InvalidOperationException("Current user is not set.");

		public WordTrainerService(IWordRepository repository, IWordsShuffleService wordsShuffleService)
		{
			_repository = repository;
			_wordsShuffleService = wordsShuffleService;
		}

		public void SetUser(UserModel user)
		{
			_session = new TrainingSession(user);
		}

		public void SetDictionary(int? dictionaryId)
		{
			var session = CurrentSession;

			if (session.DictionaryId == dictionaryId) return;

			session.DictionaryId = dictionaryId;
			session.Words.Clear();
			session.CurrentWord = null;
		}

		public int GetWordsCount()
		{
			return CurrentSession.Words.Count;
		}

		public async Task LoadWordsAsync()
		{
			var session = CurrentSession;
			var dbWords = await _repository.GetAllAsync(session.User.Id, session.DictionaryId);
			var shuffledWords = _wordsShuffleService.Shuffle(dbWords);
			session.Words.Clear();
			session.Words.AddRange(shuffledWords);
		}

		public WordDto? GetCurrentWord()
		{
			var currentWord = CurrentSession.CurrentWord;
			return currentWord is null
				? null
				: new WordDto(currentWord.Value, currentWord.Translation);
		}

		public async Task<WordDto?> GetNewWordAsync()
		{
			var session = CurrentSession;

			if (session.CurrentWord is not null) session.Words.Remove(session.CurrentWord);

			if (session.Words.IsNullOrEmpty()) await LoadWordsAsync();

			if (session.Words.IsNullOrEmpty()) return null;

			session.CurrentWord = session.Words[0];

			return session.CurrentWord;
		}

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

		public async Task UpdateCurrentWordAsync(UpdateWeightType updateWeightType)
		{
			var session = CurrentSession;
			var word = session.CurrentWord;
			if (!NeedToUpdateWordWeight(updateWeightType, word)) return;

			await _repository.UpdateWeightAsync(new UpdateWordWeightRequest(word!.Id, session.User.Id, word.DictionaryId, updateWeightType));
		}

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

		// increase is capped at MaxWeight, decrease is floored at 0 — skip the DB call when already at the boundary
		private bool NeedToUpdateWordWeight(UpdateWeightType updateWeightType, WordDto? word)
		{
			if (word is null) return false;

			return updateWeightType switch
			{
				UpdateWeightType.Increase => word.Weight < WordConstants.MaxWeight,
				UpdateWeightType.Decrease => word.Weight > 0,
				_ => throw new ArgumentException($"{updateWeightType} is not valid")
			};
		}
	}
}
