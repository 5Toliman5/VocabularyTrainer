using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp.Presenter
{
	public partial class MainFormPresenter
	{
		private int _myWordsPage = 1;
		private WordSortBy _myWordsSortBy = WordSortBy.DateAdded;
		private bool _myWordsSortDesc = true;

		private static readonly IReadOnlyDictionary<string, WordSortBy> ColumnSortMap =
			new Dictionary<string, WordSortBy>
			{
				["Value"]          = WordSortBy.Value,
				["Translation"]    = WordSortBy.Translation,
				["DictionaryName"] = WordSortBy.DictionaryName,
				["LanguageCode"]   = WordSortBy.Language,
				["DateAdded"]      = WordSortBy.DateAdded,
				["Weight"]         = WordSortBy.Weight,
			};

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

				_myWordsPage = 1;
				await LoadMyWordsPageAsync();
			});
		}

		private async void OnApplyWordFilterRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				_myWordsPage = 1;
				await LoadMyWordsPageAsync();
			});
		}

		private async void OnPreviousWordPageRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (_myWordsPage > 1) _myWordsPage--;
				await LoadMyWordsPageAsync();
			});
		}

		private async void OnNextWordPageRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				_myWordsPage++;
				await LoadMyWordsPageAsync();
			});
		}

		private async void OnDeleteMyWordsWordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				var word = _view.SelectedMyWordsWord;
				if (word is null || _user is null) return;

				await _wordRepository.DeleteAsync(new UserWordKey(word.Id, _user.Id, word.DictionaryId));
				await LoadMyWordsPageAsync();
			});
		}

		private async void OnResetWordFilterRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				_myWordsSortBy = WordSortBy.DateAdded;
				_myWordsSortDesc = true;
				_view.ResetMyWordsFilters();
				_myWordsPage = 1;
				await LoadMyWordsPageAsync();
			});
		}

		private async void OnMyWordsSortChanged(object? sender, string columnName)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!ColumnSortMap.TryGetValue(columnName, out var sortBy)) return;

				if (_myWordsSortBy == sortBy)
					_myWordsSortDesc = !_myWordsSortDesc;
				else
				{
					_myWordsSortBy = sortBy;
					_myWordsSortDesc = false;
				}

				_myWordsPage = 1;
				await LoadMyWordsPageAsync();
			});
		}

		private async Task LoadMyWordsPageAsync()
		{
			if (_user is null) return;

			var request = new GetWordsPagedRequest
			{
				UserId       = _user.Id,
				Page         = _myWordsPage,
				PageSize     = Constants.MyWordsPageSize,
				DictionaryId = _view.MyWordsDictionaryId,
				Language     = _view.MyWordsLanguage,
				Search       = _view.MyWordsSearch,
				DateFrom     = _view.MyWordsDateFrom,
				DateTo       = _view.MyWordsDateTo,
				SortBy       = _myWordsSortBy,
				SortDesc     = _myWordsSortDesc,
			};

			var result = await _wordRepository.GetPagedAsync(request);

			if (result.Items.Count == 0 && _myWordsPage > 1)
			{
				_myWordsPage--;
				request = request with { Page = _myWordsPage };
				result = await _wordRepository.GetPagedAsync(request);
			}

			_view.LoadMyWordsPage(result);
		}
	}
}
