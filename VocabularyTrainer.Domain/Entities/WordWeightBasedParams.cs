namespace VocabularyTrainer.Domain.Entities
{
	public class WordWeightBasedParams
	{
		public int WordId { get; set; }

		public int Weight { get; set; }

		public Word Word { get; set; } = null!;
	}
}
