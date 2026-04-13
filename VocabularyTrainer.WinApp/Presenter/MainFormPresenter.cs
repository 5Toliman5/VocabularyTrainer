using Microsoft.Extensions.Logging;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp.Presenter
{
	/// <summary>Presenter that mediates between <see cref="IMainFormView"/> and the domain services.</summary>
	public class MainFormPresenter : IDisposable
	{
		private readonly IMainFormView _view;
		private readonly IUserService _userService;
		private readonly IWordTrainerService _wordTrainerService;
		private readonly IDictionaryService _dictionaryService;
		private readonly ILogger<MainFormPresenter> _logger;

		private UserModel? _user;
		private bool _translationWasShown;
		private bool _isBusy;

		/// <summary>Initializes the presenter and subscribes to all view events.</summary>
		public MainFormPresenter(
			IMainFormView view,
			IUserService userService,
			IWordTrainerService wordTrainerService,
			IDictionaryService dictionaryService,
			ILogger<MainFormPresenter> logger)
		{
			_view = view;
			_userService = userService;
			_wordTrainerService = wordTrainerService;
			_dictionaryService = dictionaryService;
			_logger = logger;
			SubscribeToEvents();
		}

		/// <summary>Unsubscribes from all view events.</summary>
		public void Dispose()
		{
			_view.UserChanged -= OnUserChanged;
			_view.TrainingDictionaryChanged -= OnTrainingDictionaryChanged;
			_view.AddWordRequested -= OnAddWordRequested;
			_view.ShowNextWordRequested -= OnShowNextWordRequested;
			_view.ShowTranslationRequested -= OnShowTranslationRequested;
			_view.DeleteWordRequested -= OnDeleteWordRequested;
			_view.AddDictionaryRequested -= OnAddDictionaryRequested;
			_view.UpdateDictionaryRequested -= OnUpdateDictionaryRequested;
			_view.DeleteDictionaryRequested -= OnDeleteDictionaryRequested;
			_view.MyWordsPageEntered -= OnMyWordsPageEntered;
		}

		/// <summary>Subscribes all presenter handler methods to the corresponding view events.</summary>
		private void SubscribeToEvents()
		{
			_view.UserChanged += OnUserChanged;
			_view.TrainingDictionaryChanged += OnTrainingDictionaryChanged;
			_view.AddWordRequested += OnAddWordRequested;
			_view.ShowNextWordRequested += OnShowNextWordRequested;
			_view.ShowTranslationRequested += OnShowTranslationRequested;
			_view.DeleteWordRequested += OnDeleteWordRequested;
			_view.AddDictionaryRequested += OnAddDictionaryRequested;
			_view.UpdateDictionaryRequested += OnUpdateDictionaryRequested;
			_view.DeleteDictionaryRequested += OnDeleteDictionaryRequested;
			_view.MyWordsPageEntered += OnMyWordsPageEntered;
		}

		/// <summary>Loads the user, their dictionaries, and the first word when the user name changes.</summary>
		private async void OnUserChanged(object? sender, string userName)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				_user = await _userService.GetAsync(userName);

				if (!ValidateUser()) return;

				_wordTrainerService.SetUser(_user!);

				var dicts = await _dictionaryService.GetAllAsync(_user!.Id);
				_view.LoadDictionaries(dicts);

				await ShowNextWordCoreAsync();
			});
		}

		/// <summary>Resets the current dictionary filter and loads the first word for the newly selected training dictionary.</summary>
		private async void OnTrainingDictionaryChanged(object? sender, int? dictionaryId)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (_user is null) return;
				_wordTrainerService.SetDictionary(dictionaryId);
				await ShowNextWordCoreAsync();
			});
		}

		/// <summary>Validates the word input and adds the word to the selected dictionary via the trainer service.</summary>
		private async void OnAddWordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!_view.ValidateAddWordInput()) return;

				if (!ValidateUser()) return;

				var dictionaryId = _view.SelectedAddingDictionaryId;
				if (dictionaryId is null)
				{
					_view.ShowAddingDictionaryError(Constants.NoDictionaryAvailable);
					return;
				}

				var word = new WordDto(_view.InputWord, _view.InputTranslation);

				await _wordTrainerService.AddWordAsync(word, dictionaryId.Value);

				_view.ClearAddWordInput();
			});
		}

		/// <summary>Advances to the next word in the training session.</summary>
		private async void OnShowNextWordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(ShowNextWordCoreAsync);
		}

		/// <summary>Reveals the translation of the current word, increases its weight, and updates the Next button caption.</summary>
		private async void OnShowTranslationRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				var currentWord = _wordTrainerService.GetCurrentWord();

				if (currentWord is null) return;

				await _wordTrainerService.UpdateCurrentWordAsync(UpdateWeightType.Increase);

				_view.DisplayTranslation(currentWord.Translation);

				_view.SetShowNextButtonText(Constants.ChangedShowNextButtonText);

				_translationWasShown = true;
			});
		}

		/// <summary>Deletes the current word and advances to the next one.</summary>
		private async void OnDeleteWordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (_wordTrainerService.GetCurrentWord() is null) return;

				await _wordTrainerService.DeleteCurrentWordAsync();

				await ShowNextWordCoreAsync();
			});
		}

		/// <summary>Creates a new dictionary for the current user and refreshes the dictionary lists.</summary>
		private async void OnAddDictionaryRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!ValidateUser()) return;

				var name = _view.InputDictionaryName.Trim();
				if (string.IsNullOrEmpty(name))
				{
					_view.ShowError(Constants.DictionaryNameRequired);
					return;
				}

				await _dictionaryService.AddAsync(new AddDictionaryRequest(_user!.Id, name, _view.InputLanguageCode));

				var dicts = await _dictionaryService.GetAllAsync(_user!.Id);
				_view.LoadDictionaries(dicts);
				_view.ClearMyWordsDictionaryInput();
			});
		}

		/// <summary>Renames (and optionally re-tags the language of) the selected dictionary and refreshes the lists.</summary>
		private async void OnUpdateDictionaryRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!ValidateUser()) return;

				var dictionaryId = _view.SelectedMyWordsDictionaryId;
				if (dictionaryId is null) return;

				var name = _view.InputDictionaryName.Trim();
				if (string.IsNullOrEmpty(name))
				{
					_view.ShowError(Constants.DictionaryNameRequired);
					return;
				}

				await _dictionaryService.UpdateAsync(
					new UpdateDictionaryRequest(dictionaryId.Value, _user!.Id, name, _view.InputLanguageCode));

				var dicts = await _dictionaryService.GetAllAsync(_user!.Id);
				_view.LoadDictionaries(dicts);
			});
		}

		/// <summary>Deletes the selected dictionary (guarded against deleting the last one) and refreshes the lists.</summary>
		private async void OnDeleteDictionaryRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!ValidateUser()) return;

				var dictionaryId = _view.SelectedMyWordsDictionaryId;
				if (dictionaryId is null) return;

				var dicts = await _dictionaryService.GetAllAsync(_user!.Id);
				if (dicts.Count <= 1)
				{
					_view.ShowError(Constants.CannotDeleteLastDictionary);
					return;
				}

				await _dictionaryService.DeleteAsync(dictionaryId.Value, _user!.Id);

				var updated = await _dictionaryService.GetAllAsync(_user!.Id);
				_view.LoadDictionaries(updated);
				_view.ClearMyWordsDictionaryInput();

				_wordTrainerService.SetDictionary(null);
			});
		}

		/// <summary>Ensures the user is loaded and refreshes the dictionary lists when the My Words tab is activated.</summary>
		private async void OnMyWordsPageEntered(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (_user is null)
				{
					_user = await _userService.GetAsync(_view.CurrentUserName);
					if (!ValidateUser()) return;
					_wordTrainerService.SetUser(_user!);
				}

				var dicts = await _dictionaryService.GetAllAsync(_user!.Id);
				_view.LoadDictionaries(dicts);
			});
		}

		/// <summary>Runs <paramref name="action"/> only when no other operation is in progress, translating domain exceptions into user-facing error messages.</summary>
		private async Task ExecuteIfFreeAsync(Func<Task> action)
		{
			if (_isBusy) return;
			try
			{
				_isBusy = true;
				await action();
			}
			catch (DuplicateNameException ex)
			{
				_logger.LogWarning(ex, "Duplicate dictionary name attempted");
				_view.ShowError(Constants.DuplicateDictionaryName);
			}
			catch (DatabaseException ex)
			{
				_logger.LogError(ex, "Database error");
				_view.ShowError(Constants.DatabaseError);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Unexpected error");
				_view.ShowError(string.Format(Constants.UnexpectedError, ex.Message));
			}
			finally
			{
				_isBusy = false;
			}
		}

		/// <summary>Decreases weight for an unanswered word, fetches the next word, and updates all related view fields.</summary>
		private async Task ShowNextWordCoreAsync()
		{
			_view.ClearShowWordOutput();

			if (_user is null) return;

			if (!_translationWasShown) await _wordTrainerService.UpdateCurrentWordAsync(UpdateWeightType.Decrease);

			_translationWasShown = false;

			var word = await _wordTrainerService.GetNewWordAsync();
			if (word is null)
			{
				_view.ShowError(Constants.NoWordsFoundError);
				return;
			}

			_view.DisplayNewWord(word.Value);
			_view.SetCurrentWordDictionary(word.DictionaryName);
			_view.SetShowNextButtonText(Constants.DefaultShowNextButtonText);
		}

		/// <summary>Shows an error and returns <c>false</c> when no user is loaded; returns <c>true</c> otherwise.</summary>
		private bool ValidateUser()
		{
			if (_user is null)
			{
				_view.ShowError(string.Format(Constants.UserNotFoundError, _view.CurrentUserName));
				return false;
			}
			return true;
		}
	}
}
