using Microsoft.Extensions.Logging;
using VocabularyTrainer.Domain.Exceptions;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Repositories;
using VocabularyTrainer.Domain.Services;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp.Presenter
{
	public partial class MainFormPresenter : IDisposable
	{
		private readonly IMainFormView _view;
		private readonly IUserService _userService;
		private readonly IWordTrainerService _wordTrainerService;
		private readonly IDictionaryService _dictionaryService;
		private readonly IWordRepository _wordRepository;
		private readonly ILogger<MainFormPresenter> _logger;

		private UserModel? _user;
		private bool _isBusy;

		public MainFormPresenter(
			IMainFormView view,
			IUserService userService,
			IWordTrainerService wordTrainerService,
			IDictionaryService dictionaryService,
			IWordRepository wordRepository,
			ILogger<MainFormPresenter> logger)
		{
			_view = view;
			_userService = userService;
			_wordTrainerService = wordTrainerService;
			_dictionaryService = dictionaryService;
			_wordRepository = wordRepository;
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
			_view.AddDictionaryRequested -= OnAddDictionaryRequested;
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
			_view.AddDictionaryRequested += OnAddDictionaryRequested;
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
	}
}
