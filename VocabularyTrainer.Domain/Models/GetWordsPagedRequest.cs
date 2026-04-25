namespace VocabularyTrainer.Domain.Models
{
	public record GetWordsPagedRequest
	{
		public int UserId { get; init; }
		public int Page { get; init; } = 1;
		public int PageSize { get; init; } = 10;
		public int? DictionaryId { get; init; }
		public string? Language { get; init; }
		public string? Search { get; init; }
		public DateTime? DateFrom { get; init; }
		public DateTime? DateTo { get; init; }
		public WordSortBy SortBy { get; init; } = WordSortBy.DateAdded;
		public bool SortDesc { get; init; } = true;
	}
}
