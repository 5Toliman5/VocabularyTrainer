using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.BusinessLogic.Services
{
	public class TrainingSession(UserModel user)
	{
		public UserModel User { get; } = user;
		public List<WordDto> Words { get; } = [];
		public WordDto? CurrentWord { get; set; }
		public int? DictionaryId { get; set; }
	}
}
