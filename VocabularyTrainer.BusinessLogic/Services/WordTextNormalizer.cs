using System.Globalization;
using System.Text;

namespace VocabularyTrainer.BusinessLogic.Services
{
	internal static class WordTextNormalizer
	{
		public static string Normalize(string value)
		{
			if (string.IsNullOrWhiteSpace(value))
			{
				return string.Empty;
			}

			var collapsed = CollapseWhitespace(value.Trim());
			var lowered = collapsed.ToLowerInvariant();
			return RemoveDiacritics(lowered);
		}

		private static string CollapseWhitespace(string value)
		{
			var sb = new StringBuilder(value.Length);
			var lastSpace = false;

			foreach (var ch in value)
			{
				if (char.IsWhiteSpace(ch))
				{
					if (!lastSpace) sb.Append(' ');
					lastSpace = true;
				}
				else
				{
					sb.Append(ch);
					lastSpace = false;
				}
			}

			return sb.ToString();
		}

		private static string RemoveDiacritics(string value)
		{
			var decomposed = value.Normalize(NormalizationForm.FormD);
			var sb = new StringBuilder(decomposed.Length);

			foreach (var ch in decomposed)
			{
				if (CharUnicodeInfo.GetUnicodeCategory(ch) != UnicodeCategory.NonSpacingMark)
				{
					sb.Append(ch);
				}
			}

			return sb.ToString().Normalize(NormalizationForm.FormC);
		}
	}
}
