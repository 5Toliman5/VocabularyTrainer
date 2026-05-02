using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp.Presenter
{
	public partial class MainFormPresenter
	{
		private bool _translationWasShown;

		private async void OnUserChanged(object? sender, string userName)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				var userResult = await _userService.GetAsync(userName);
				if (!userResult.Successful)
				{
					_view.ShowError(string.Format(Constants.UserNotFoundError, userName));
					return;
				}
				_user = userResult.Value;
				_wordTrainerService.SetUser(_user!);

				var dicts = await _dictionaryService.GetAllAsync(_user!.Id);
				_view.LoadDictionaries(dicts);

				await ShowNextWordCoreAsync();
			});
		}

		private async void OnTrainingDictionaryChanged(object? sender, int? dictionaryId)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (_user is null) return;
				_wordTrainerService.SetDictionary(dictionaryId);
				await ShowNextWordCoreAsync();
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
	}
}
