using System.Globalization;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.Infrastructure.Auth
{
	public class JwtTokenService : ITokenService
	{
		private readonly JwtOptions _options;
		private readonly SigningCredentials _signingCredentials;
		private readonly JsonWebTokenHandler _handler = new();

		public JwtTokenService(IOptions<JwtOptions> options)
		{
			_options = options.Value;
			_signingCredentials = new SigningCredentials(_options.CreateSigningKey(), SecurityAlgorithms.HmacSha256);
		}

		public AccessToken CreateAccessToken(UserModel user)
		{
			var now = DateTime.UtcNow;
			var expiresAt = now + _options.AccessTokenLifetime;

			var descriptor = new SecurityTokenDescriptor
			{
				Issuer = _options.Issuer,
				Audience = _options.Audience,
				IssuedAt = now,
				NotBefore = now,
				Expires = expiresAt,
				SigningCredentials = _signingCredentials,
				Claims = new Dictionary<string, object>
				{
					[JwtRegisteredClaimNames.Sub] = user.Id.ToString(CultureInfo.InvariantCulture),
					[JwtRegisteredClaimNames.Name] = user.DisplayName,
					[JwtRegisteredClaimNames.Jti] = Guid.NewGuid().ToString("N"),
				},
			};

			return new AccessToken(_handler.CreateToken(descriptor), expiresAt);
		}
	}
}
