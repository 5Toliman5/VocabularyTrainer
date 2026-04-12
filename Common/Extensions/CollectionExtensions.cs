namespace Common.Extensions
{
	/// <summary>Extension methods for null-safe collection emptiness checks.</summary>
	public static class CollectionExtensions
	{
		/// <summary>Returns <see langword="true"/> if the collection is <see langword="null"/> or contains no elements.</summary>
		public static bool IsNullOrEmpty<T>(this ICollection<T>? collection)
		{
			return collection is null || collection.Count == 0;
		}

		/// <summary>Returns <see langword="true"/> if the sequence is <see langword="null"/> or yields no elements.</summary>
		public static bool IsNullOrEmpty<T>(this IEnumerable<T>? collection)
		{
			return collection is null || !collection.Any();
		}

		/// <summary>Returns <see langword="true"/> if the array is <see langword="null"/> or has zero length.</summary>
		public static bool IsNullOrEmpty<T>(this T[] collection)
		{
			return collection is null || collection.Length == 0;
		}
	}
}
