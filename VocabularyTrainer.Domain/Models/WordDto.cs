namespace VocabularyTrainer.Domain.Models
{
	public record WordDto
	{
		public int Id { get; init; }
		public int UserId { get; init; }
		public int DictionaryId { get; init; }
		public string DictionaryName { get; init; } = string.Empty;
		public string Value { get; init; } = string.Empty;
		public string LanguageCode { get; init; } = string.Empty;
		public string? Notes { get; init; }
		public IReadOnlyList<WordTranslationDto> Translations { get; init; } = [];
		public DateTime DateAdded { get; init; }
		public DateTime DateModified { get; init; }
		public string PrimaryTranslation =>
			Translations.FirstOrDefault(t => t.Kind == TranslationKind.Translation)?.Text
				?? Translations.FirstOrDefault()?.Text
				?? string.Empty;
	}
}
