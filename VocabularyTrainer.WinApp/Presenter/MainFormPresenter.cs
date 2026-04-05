using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Services;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp.Presenter
{
	public class MainFormPresenter
	{
        private readonly IMainFormView _view;
        private readonly IUserService _userService;
        private readonly IWordTrainerService _wordTrainerService;

        private UserModel? _user;
		private bool _translationWasShown = false;
		private bool _isBusy = false;

		public MainFormPresenter(IMainFormView view, IUserService userService, IWordTrainerService wordTrainerService)
		{
			_view = view;
			_userService = userService;
			_wordTrainerService = wordTrainerService;
			SubscribeToEvents();
		}

		private void SubscribeToEvents()
		{
			_view.UserChanged += OnUserChanged;
			_view.AddWordRequested += OnAddWordRequested;
			_view.ShowNextWordRequested += OnShowNextWordRequested;
			_view.ShowTranslationRequested += OnShowTranslationRequested;
			_view.DeleteWordRequested += OnDeleteWordRequested;
		}

		private async void OnUserChanged(object? sender, string userName)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				_user = await _userService.GetAsync(userName);

				if (!ValidateUser()) return;

				_wordTrainerService.SetUser(_user!);

				await ShowNextWordCoreAsync();
			});
		}

		private async void OnAddWordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!_view.ValidateAddWordInput()) return;

				if (!ValidateUser()) return;

				var word = new WordDto(_view.InputWord, _view.InputTranslation);

				await _wordTrainerService.AddWordAsync(word);

				_view.ClearAddWordInput();
			});
		}

		private async void OnShowNextWordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(ShowNextWordCoreAsync);
		}

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

		private async void OnDeleteWordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (_wordTrainerService.GetCurrentWord() is null) return;

				await _wordTrainerService.DeleteCurrentWordAsync();

				await ShowNextWordCoreAsync();
			});
		}

		private async Task ExecuteIfFreeAsync(Func<Task> action)
		{
			if (_isBusy) return;
			try
			{
				_isBusy = true;
				await action();
			}
			finally
			{
				_isBusy = false;
			}
		}

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
			_view.SetShowNextButtonText(Constants.DefaultShowNextButtonText);
		}

		private bool ValidateUser()
		{
			if (_user is null)
			{
				_view.ShowError("User has not been found!");
				return false;
			}
			return true;
		}
    }
}
