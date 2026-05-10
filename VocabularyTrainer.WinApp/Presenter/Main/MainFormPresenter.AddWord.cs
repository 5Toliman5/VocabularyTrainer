using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp.Presenter.Main
{
	public partial class MainFormPresenter
	{
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

				IReadOnlyList<WordTranslationDto> translations =
					[new WordTranslationDto(_view.InputTranslation, TranslationKind.Translation)];

				var request = new AddWordRequest(_view.InputWord, translations, _user!.Id, dictionaryId.Value);

				var result = await _wordTrainerService.AddWordAsync(request);
				if (!result.Successful)
				{
					_view.ShowError(result.ErrorMessage ?? "Failed to add the word.");
					return;
				}

				_view.ClearAddWordInput();
			});
		}
	}
}
