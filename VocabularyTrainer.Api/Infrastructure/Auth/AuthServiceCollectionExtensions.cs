using System.Threading.RateLimiting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using VocabularyTrainer.BusinessLogic.Services.Auth;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.Infrastructure.Auth
{
	public static class AuthServiceCollectionExtensions
	{
		public const string AuthRateLimitPolicy = "auth";

		public static IServiceCollection AddApiAuthentication(this IServiceCollection services, IConfiguration configuration)
		{
			var jwtSection = configuration.GetSection(JwtOptions.SectionName);
			var jwtOptions = jwtSection.Get<JwtOptions>() ?? new JwtOptions();
			var authSection = configuration.GetSection(AuthOptions.SectionName);
			var authOptions = authSection.Get<AuthOptions>() ?? new AuthOptions();

			services.Configure<JwtOptions>(jwtSection);
			services.Configure<AuthOptions>(authSection);
			services.AddSingleton<ITokenService, JwtTokenService>();

			services
				.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
				.AddJwtBearer(options =>
				{
					// Keep "sub"/"name" as-is instead of remapping them to legacy WS-* claim URIs.
					options.MapInboundClaims = false;
					options.TokenValidationParameters = new TokenValidationParameters
					{
						ValidIssuer = jwtOptions.Issuer,
						ValidAudience = jwtOptions.Audience,
						IssuerSigningKey = jwtOptions.CreateSigningKey(),
						ValidateIssuerSigningKey = true,
						NameClaimType = JwtRegisteredClaimNames.Name,
						ClockSkew = TimeSpan.FromSeconds(30),
					};
				});

			// Every endpoint requires a valid access token unless it opts out with [AllowAnonymous].
			services.AddAuthorizationBuilder()
				.SetFallbackPolicy(new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build());

			services.AddRateLimiter(options =>
			{
				options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
				options.AddPolicy(AuthRateLimitPolicy, context => CreateAuthPartition(context, authOptions.AuthRequestsPerMinutePerIp));
			});

			return services;
		}

		private static RateLimitPartition<string> CreateAuthPartition(HttpContext context, int permitsPerMinute)
		{
			var clientIp = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";

			return RateLimitPartition.GetFixedWindowLimiter(clientIp, _ => new FixedWindowRateLimiterOptions
			{
				PermitLimit = permitsPerMinute,
				Window = TimeSpan.FromMinutes(1),
				QueueLimit = 0,
			});
		}
	}
}
