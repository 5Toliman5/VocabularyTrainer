using System.Globalization;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp.Presenter.AddDictionary
{
	public sealed class AddDictionaryFormPresenter
	{
		public DictionaryInputEventArgs? ShowModal(
			IWin32Window owner,
			IReadOnlyList<CultureInfo> neutralCultures,
			IReadOnlyList<AlgorithmInfo> algorithms)
		{
			using var form = new AddDictionaryForm();
			form.Initialize(neutralCultures, algorithms);

			void OnConfirm(object? sender, EventArgs e)
			{
				if (string.IsNullOrWhiteSpace(form.DictionaryName))
				{
					form.ShowValidationError(Constants.DictionaryNameRequired);
					return;
				}

				form.CompleteSuccessfully();
			}

			form.ConfirmRequested += OnConfirm;

			try
			{
				return form.ShowDialog(owner) == DialogResult.OK
					? new DictionaryInputEventArgs(form.DictionaryName, form.DictionaryLanguageCode, form.DictionaryAlgorithmCode)
					: null;
			}
			finally
			{
				form.ConfirmRequested -= OnConfirm;
			}
		}
	}
}
