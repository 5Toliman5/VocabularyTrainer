using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace VocabularyTrainer.Domain.Models
{
	// Wire: IParsable accepts "all" or a dictionary id string.
	public readonly record struct DictionaryScope : IParsable<DictionaryScope>
	{
		public const string AllToken = "all";

		public static DictionaryScope All { get; } = new(null);

		public static DictionaryScope Single(int dictionaryId) => new(dictionaryId);

		// Lifts a nullable id (UI / wire convenience) into a scope.
		public static DictionaryScope FromNullableId(int? dictionaryId) =>
			dictionaryId is null ? All : Single(dictionaryId.Value);

		public int? DictionaryId { get; }

		public bool IsAll => DictionaryId is null;

		private DictionaryScope(int? dictionaryId) => DictionaryId = dictionaryId;

		public override string ToString() =>
			DictionaryId is { } id ? id.ToString(CultureInfo.InvariantCulture) : AllToken;

		public static DictionaryScope Parse(string s, IFormatProvider? provider) =>
			TryParse(s, provider, out var scope)
				? scope
				: throw new FormatException($"'{s}' is not a valid DictionaryScope. Use '{AllToken}' or a positive integer id.");

		public static bool TryParse(
			[NotNullWhen(true)] string? s,
			IFormatProvider? provider,
			out DictionaryScope result)
		{
			if (string.IsNullOrWhiteSpace(s))
			{
				result = default;
				return false;
			}

			if (string.Equals(s, AllToken, StringComparison.OrdinalIgnoreCase))
			{
				result = All;
				return true;
			}

			if (int.TryParse(s, NumberStyles.Integer, provider ?? CultureInfo.InvariantCulture, out var id) && id > 0)
			{
				result = Single(id);
				return true;
			}

			result = default;
			return false;
		}
	}
}
