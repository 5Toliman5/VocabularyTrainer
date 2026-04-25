namespace VocabularyTrainer.Api.Contract.Words
{
	public record WordPageItem
	(
		int Id,
		string Value,
		string Translation,
		int Weight,
		int DictionaryId,
		string DictionaryName,
		string? LanguageCode,
		DateTime DateAdded,
		DateTime DateModified
	);
}
