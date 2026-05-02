using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp.Presenter
{
	public partial class MainFormPresenter
	{
		private async void OnMyDictionariesPageEntered(object? sender, EventArgs e)
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

		private async void OnAddDictionaryRequested(object? sender, DictionaryInputEventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!ValidateUser()) return;

				await _dictionaryService.AddAsync(new AddDictionaryRequest(_user!.Id, e.Name, e.LanguageCode));

				var dicts = await _dictionaryService.GetAllAsync(_user!.Id);
				_view.LoadDictionaries(dicts);
				_view.ClearMyWordsDictionaryInput();
			});
		}

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
	}
}
