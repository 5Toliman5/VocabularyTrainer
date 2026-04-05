using VocabularyTrainer.Domain.Entities;

namespace VocabularyTrainer.Domain.Models
{
	public record AddWordRequest(Word Word, int UserId, int DictionaryId);
}
