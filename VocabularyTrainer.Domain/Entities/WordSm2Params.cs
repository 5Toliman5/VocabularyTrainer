namespace VocabularyTrainer.Domain.Entities
{
	public class WordSm2Params
	{
		public int WordId { get; set; }
		public int Repetitions { get; set; }
		public int IntervalDays { get; set; }
		public decimal EaseFactor { get; set; } = 2.50m;
		public int Lapses { get; set; }
		public DateTime? LastReviewedAt { get; set; }
		public DateTime? NextDueAt { get; set; }
		public Word Word { get; set; } = null!;
	}
}
