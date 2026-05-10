namespace VocabularyTrainer.Api.Contract.Words
{
	public record WordPageItem
	(
		int Id,
		int DictionaryId,
		string DictionaryName,
		string Value,
		string PrimaryTranslation,
		string LanguageCode,
		DateTime DateAdded,
		DateTime DateModified
	);
}
