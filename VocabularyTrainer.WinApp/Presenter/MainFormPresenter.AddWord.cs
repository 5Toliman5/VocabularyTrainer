using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp.Presenter
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

				var word = new WordDto(_view.InputWord, _view.InputTranslation);

				await _wordTrainerService.AddWordAsync(word, dictionaryId.Value);

				_view.ClearAddWordInput();
			});
		}
	}
}
