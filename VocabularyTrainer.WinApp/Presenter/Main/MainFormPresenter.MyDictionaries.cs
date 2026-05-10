using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp.Presenter.Main
{
	public partial class MainFormPresenter
	{
		private async void OnMyDictionariesPageEntered(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (_user is null)
				{
					var userResult = await _userService.GetAsync(_view.CurrentUserName);

					if (!userResult.Successful)
					{
						_view.ShowError(string.Format(Constants.UserNotFoundError, _view.CurrentUserName));
						return;
					}

					_user = userResult.Value;
					_wordTrainerService.SetUser(_user!);
				}

				await EnsureAlgorithmsLoadedAsync();

				await RefreshDictionariesAsync();
			});
		}

		private async void OnAddDictionaryClickRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				await EnsureAlgorithmsLoadedAsync();

				var input = _addDictionaryFormPresenter.ShowModal(
					_view.DialogOwner,
					_view.NeutralCultures,
					_algorithms
				);

				if (input is null) return;
				if (!ValidateUser()) return;

				var request = new AddDictionaryRequest(_user!.Id, input.Name, input.LanguageCode, input.AlgorithmCode);
				var result = await _dictionaryService.AddAsync(request);

				if (!result.Successful)
				{
					_view.ShowError(result.ErrorKind == ResultErrorKind.Conflict
						? Constants.DuplicateDictionaryName
						: result.ErrorMessage
					);
					return;
				}

				await RefreshDictionariesAsync();
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

				var algorithmCode = ResolveAlgorithmCodeForUpdate(dictionaryId.Value);

				var request = new UpdateDictionaryRequest(dictionaryId.Value, _user!.Id, name, _view.InputLanguageCode, algorithmCode);
				var result = await _dictionaryService.UpdateAsync(request);

				if (!result.Successful)
				{
					_view.ShowError(result.ErrorKind == ResultErrorKind.Conflict
						? Constants.DuplicateDictionaryName
						: result.ErrorMessage
					);
					return;
				}

				await RefreshDictionariesAsync();
			});
		}

		private async void OnDeleteDictionaryRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!ValidateUser()) return;

				var dictionaryId = _view.SelectedMyWordsDictionaryId;
				if (dictionaryId is null) return;

				if (_dictionaries.Count <= 1)
				{
					_view.ShowError(Constants.CannotDeleteLastDictionary);
					return;
				}

				await _dictionaryService.DeleteAsync(dictionaryId.Value, _user!.Id);

				await RefreshDictionariesAsync();
				_view.ClearMyWordsDictionaryInput();

				_wordTrainerService.SetScope(DictionaryScope.All);
			});
		}
	}
}
