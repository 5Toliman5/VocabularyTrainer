using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace VocabularyTrainer.Api.Infrastructure.Auth
{
	public class JwtOptions
	{
		public const string SectionName = "Jwt";
		private const int MinSigningKeyBytes = 32;

		public string Issuer { get; set; } = "VocabularyTrainer";

		public string Audience { get; set; } = "VocabularyTrainer.Clients";

		public string SigningKey { get; set; } = string.Empty;

		public TimeSpan AccessTokenLifetime { get; set; } = TimeSpan.FromMinutes(15);

		public SymmetricSecurityKey CreateSigningKey()
		{
			var keyBytes = Encoding.UTF8.GetBytes(SigningKey ?? string.Empty);

			if (keyBytes.Length < MinSigningKeyBytes)
			{
				throw new InvalidOperationException(
					$"Jwt:SigningKey must be at least {MinSigningKeyBytes} bytes long. Set it with user-secrets " +
					"or the Jwt__SigningKey environment variable."
				);
			}

			return new SymmetricSecurityKey(keyBytes);
		}
	}
}
