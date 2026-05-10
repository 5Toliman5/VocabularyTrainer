using System.Globalization;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.View
{
	public interface IAddDictionaryFormView
	{
		event EventHandler? ConfirmRequested;

		string DictionaryName { get; }
		string DictionaryLanguageCode { get; }
		string DictionaryAlgorithmCode { get; }

		void Initialize(IReadOnlyList<CultureInfo> neutralCultures, IReadOnlyList<AlgorithmInfo> algorithms);
		void ShowValidationError(string message);
		void CompleteSuccessfully();
	}
}
