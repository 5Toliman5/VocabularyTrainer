using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.BusinessLogic.Services
{
	/// <summary>Holds all mutable state for a single user's training session.</summary>
	public class TrainingSession(UserModel user)
	{
		/// <summary>The user this session belongs to.</summary>
		public UserModel User { get; } = user;

		/// <summary>Words queued for training in the current session.</summary>
		public List<WordDto> Words { get; } = [];

		/// <summary>The word currently being trained, or <see langword="null"/> if no word is active.</summary>
		public WordDto? CurrentWord { get; set; }

		/// <summary>The dictionary filter applied to this session, or <see langword="null"/> for all dictionaries.</summary>
		public int? DictionaryId { get; set; }
	}
}
