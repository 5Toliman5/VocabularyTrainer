using Microsoft.Extensions.Logging;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.Presenter.AddDictionary;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp.Presenter.Main
{
	public partial class MainFormPresenter : IDisposable
	{
		private readonly IMainFormView _view;
		private readonly IUserService _userService;
		private readonly IWordTrainerService _wordTrainerService;
		private readonly IDictionaryService _dictionaryService;
		private readonly IWordService _wordService;
		private readonly IAlgorithmCatalog _algorithmCatalog;
		private readonly ILogger<MainFormPresenter> _logger;
		private readonly AddDictionaryFormPresenter _addDictionaryFormPresenter;

		private bool _isBusy;
		private UserModel? _user;
		private List<DictionaryDto> _dictionaries = [];
		private IReadOnlyList<AlgorithmInfo> _algorithms = [];

		public MainFormPresenter(
			IMainFormView view,
			IUserService userService,
			IWordTrainerService wordTrainerService,
			IDictionaryService dictionaryService,
			IWordService wordService,
			IAlgorithmCatalog algorithmCatalog,
			AddDictionaryFormPresenter addDictionaryFormPresenter,
			ILogger<MainFormPresenter> logger)
		{
			_view = view;
			_userService = userService;
			_wordTrainerService = wordTrainerService;
			_dictionaryService = dictionaryService;
			_wordService = wordService;
			_algorithmCatalog = algorithmCatalog;
			_addDictionaryFormPresenter = addDictionaryFormPresenter;
			_logger = logger;
			SubscribeToEvents();
		}

		public void Dispose()
		{
			_view.UserChanged -= OnUserChanged;
			_view.TrainingDictionaryChanged -= OnTrainingDictionaryChanged;
			_view.AddWordRequested -= OnAddWordRequested;
			_view.ShowNextWordRequested -= OnShowNextWordRequested;
			_view.ShowTranslationRequested -= OnShowTranslationRequested;
			_view.DeleteWordRequested -= OnDeleteWordRequested;
			_view.AddDictionaryClickRequested -= OnAddDictionaryClickRequested;
			_view.UpdateDictionaryRequested -= OnUpdateDictionaryRequested;
			_view.DeleteDictionaryRequested -= OnDeleteDictionaryRequested;
			_view.MyDictionariesPageEntered -= OnMyDictionariesPageEntered;
			_view.MyWordsPageEntered -= OnMyWordsPageEntered;
			_view.ApplyWordFilterRequested -= OnApplyWordFilterRequested;
			_view.PreviousWordPageRequested -= OnPreviousWordPageRequested;
			_view.NextWordPageRequested -= OnNextWordPageRequested;
			_view.DeleteMyWordsWordRequested -= OnDeleteMyWordsWordRequested;
			_view.MyWordsSortChanged -= OnMyWordsSortChanged;
			_view.ResetWordFilterRequested -= OnResetWordFilterRequested;
		}

		private void SubscribeToEvents()
		{
			_view.UserChanged += OnUserChanged;
			_view.TrainingDictionaryChanged += OnTrainingDictionaryChanged;
			_view.AddWordRequested += OnAddWordRequested;
			_view.ShowNextWordRequested += OnShowNextWordRequested;
			_view.ShowTranslationRequested += OnShowTranslationRequested;
			_view.DeleteWordRequested += OnDeleteWordRequested;
			_view.AddDictionaryClickRequested += OnAddDictionaryClickRequested;
			_view.UpdateDictionaryRequested += OnUpdateDictionaryRequested;
			_view.DeleteDictionaryRequested += OnDeleteDictionaryRequested;
			_view.MyDictionariesPageEntered += OnMyDictionariesPageEntered;
			_view.MyWordsPageEntered += OnMyWordsPageEntered;
			_view.ApplyWordFilterRequested += OnApplyWordFilterRequested;
			_view.PreviousWordPageRequested += OnPreviousWordPageRequested;
			_view.NextWordPageRequested += OnNextWordPageRequested;
			_view.DeleteMyWordsWordRequested += OnDeleteMyWordsWordRequested;
			_view.MyWordsSortChanged += OnMyWordsSortChanged;
			_view.ResetWordFilterRequested += OnResetWordFilterRequested;
		}

		private async Task ExecuteIfFreeAsync(Func<Task> action)
		{
			if (_isBusy) return;
			try
			{
				_isBusy = true;
				await action();
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

		private bool ValidateUser()
		{
			if (_user is null)
			{
				_view.ShowError(string.Format(Constants.UserNotFoundError, _view.CurrentUserName));
				return false;
			}

			return true;
		}

		private async Task RefreshDictionariesAsync()
		{
			_dictionaries = await _dictionaryService.GetAllAsync(_user!.Id);
			_view.LoadDictionaries(_dictionaries);
		}

		private async Task EnsureAlgorithmsLoadedAsync()
		{
			if (_algorithms.Count > 0)
			{
				return;
			}

			_algorithms = await _algorithmCatalog.GetAllAsync();
			_view.LoadAlgorithms(_algorithms);
		}

		private string ResolveAlgorithmCodeForUpdate(int dictionaryId)
		{
			var selected = _view.SelectedDictionaryAlgorithmCode;

			if (!string.IsNullOrEmpty(selected))
			{
				return selected;
			}

			var dictionary = _dictionaries.FirstOrDefault(d => d.Id == dictionaryId);
			return dictionary?.AlgorithmCode ?? AlgorithmCodes.Default;
		}
	}
}

