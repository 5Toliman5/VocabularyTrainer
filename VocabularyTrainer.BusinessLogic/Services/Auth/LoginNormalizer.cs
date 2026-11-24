using System.Net.Mail;
using System.Text;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.BusinessLogic.Services.Auth
{
	// Produces the canonical ProviderKey for email / phone identities so lookups are exact matches.
	public static class LoginNormalizer
	{
		private const int MaxEmailLength = 254;
		private const int MinPhoneDigits = 8;
		private const int MaxPhoneDigits = 15;

		public static bool TryNormalize(string channel, string value, out string normalized)
		{
			normalized = null;

			if (string.Equals(channel, IdentityProviders.Email, StringComparison.Ordinal))
			{
				return TryNormalizeEmail(value, out normalized);
			}

			if (string.Equals(channel, IdentityProviders.Phone, StringComparison.Ordinal))
			{
				return TryNormalizePhone(value, out normalized);
			}

			return false;
		}

		public static bool TryResolveLogin(string login, out string provider, out string providerKey)
		{
			provider = login is not null && login.Contains('@')
				? IdentityProviders.Email
				: IdentityProviders.Phone;

			return TryNormalize(provider, login, out providerKey);
		}

		public static bool TryNormalizeEmail(string value, out string normalized)
		{
			normalized = null;

			if (string.IsNullOrWhiteSpace(value)) return false;

			var trimmed = value.Trim();

			if (trimmed.Length > MaxEmailLength) return false;

			// MailAddress also accepts "Name <a@b.c>"; requiring an exact match rejects such forms.
			if (!MailAddress.TryCreate(trimmed, out var address) || address.Address != trimmed) return false;

			if (!address.Host.Contains('.')) return false;

			normalized = address.Address.ToLowerInvariant();
			return true;
		}

		// E.164 shape check only. Country-aware validation (libphonenumber) belongs here once phone sign-in ships.
		public static bool TryNormalizePhone(string value, out string normalized)
		{
			normalized = null;

			if (string.IsNullOrWhiteSpace(value)) return false;

			var trimmed = value.Trim();

			if (!trimmed.StartsWith('+')) return false;

			var digits = new StringBuilder(MaxPhoneDigits);

			foreach (var symbol in trimmed.AsSpan(1))
			{
				if (char.IsAsciiDigit(symbol))
				{
					digits.Append(symbol);
				}
				else if (symbol is not (' ' or '-' or '(' or ')' or '.'))
				{
					return false;
				}
			}

			if (digits.Length < MinPhoneDigits || digits.Length > MaxPhoneDigits) return false;

			normalized = "+" + digits;
			return true;
		}
	}
}
