using System.Linq.Expressions;
using VocabularyTrainer.Domain.Entities;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.DataAccess.Repositories
{
	internal static class WordProjections
	{
		public static readonly Expression<Func<Word, WordDto>> ToDto = w => new WordDto
		{
			Id = w.Id,
			UserId = w.UserId,
			DictionaryId = w.DictionaryId,
			DictionaryName = w.Dictionary.Name,
			Value = w.Value,
			LanguageCode = w.LanguageCode,
			DateAdded = w.DateAdded,
			DateModified = w.DateModified,
			Translations = w.Translations
				.OrderBy(t => t.Id)
				.Select(t => new WordTranslationDto(t.Text, t.Kind))
				.ToList(),
		};
	}
}
