using Common.Extensions;
using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class WordTrainerService(IWordService wordService, ITrainingClient trainingClient)
		: IWordTrainerService
	{
		private TrainingSession? _session;
		private TrainingSession CurrentSession => _session ?? throw new InvalidOperationException("Current user is not set.");

		public void SetUser(UserModel user) => _session = new TrainingSession(user);

		public bool SetScope(DictionaryScope scope)
		{
			var session = CurrentSession;

			if (session.Scope == scope)
			{
				return false;
			}

			session.Scope = scope;
			session.Words.Clear();
			session.CurrentWord = null;
			return true;
		}

		public int GetWordsCount() => CurrentSession.Words.Count;

		public async Task LoadWordsAsync()
		{
			var session = CurrentSession;

			var batch = await trainingClient.GetCandidatesAsync(
				session.User.Id, session.Scope, WordConstants.DefaultCandidateBatchSize);

			session.Words.Clear();
			session.Words.AddRange(batch);
		}

		public WordDto? GetCurrentWord() => CurrentSession.CurrentWord;

		public async Task<WordDto?> GetNewWordAsync()
		{
			var session = CurrentSession;

			if (session.CurrentWord is not null)
			{
				session.Words.Remove(session.CurrentWord);
			}

			if (session.Words.IsNullOrEmpty())
			{
				await LoadWordsAsync();
			}

			if (session.Words.IsNullOrEmpty())
			{
				session.CurrentWord = null;
				return null;
			}

			var nextWord = session.Words[0];
			session.CurrentWord = nextWord;
			return nextWord;
		}

		public async Task<Result> AddWordAsync(AddWordRequest request)
		{
			var result = await wordService.AddAsync(request);

			if (!result.Successful)
			{
				return Result.Failure(result);
			}

			if (_session is not null
			   && (_session.Scope.IsAll || _session.Scope.DictionaryId == request.DictionaryId))
			{
				_session.Words.Clear();
			}

			return Result.Success();
		}

		public async Task ReviewCurrentWordAsync(ReviewGrade grade)
		{
			var session = CurrentSession;
			var card = session.CurrentWord;

			if (card is null)
			{
				return;
			}

			await trainingClient.ApplyReviewAsync(
				new ReviewWordRequest(card.Id, session.User.Id, card.DictionaryId, grade));
		}

		public async Task DeleteCurrentWordAsync()
		{
			var session = CurrentSession;

			if (session.CurrentWord is null)
			{
				return;
			}

			await wordService.DeleteAsync(session.CurrentWord.Id, session.User.Id);

			session.Words.Remove(session.CurrentWord);
			session.CurrentWord = null;
		}

		// Null when training scope is All (per-card algorithm may differ).
		public async Task<IReadOnlyList<ReviewGrade>?> GetSupportedGradesAsync()
		{
			var session = _session;

			if (session is null || session.Scope.IsAll)
			{
				return null;
			}

			return await trainingClient.GetSupportedGradesAsync(session.Scope.DictionaryId!.Value, session.User.Id);
		}
	}
}
