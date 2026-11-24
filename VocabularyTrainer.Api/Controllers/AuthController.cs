using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Api.Contract.Auth;
using VocabularyTrainer.Api.Infrastructure;
using VocabularyTrainer.Api.Infrastructure.Auth;
using DomainClientInfo = VocabularyTrainer.Domain.Models.Auth.ClientInfo;
using DomainExternalLoginRequest = VocabularyTrainer.Domain.Models.Auth.ExternalLoginRequest;
using DomainLoginRequest = VocabularyTrainer.Domain.Models.Auth.PasswordLoginRequest;
using DomainPurpose = VocabularyTrainer.Domain.Models.Auth.VerificationPurpose;
using DomainRegisterRequest = VocabularyTrainer.Domain.Models.Auth.RegisterRequest;
using DomainResetPasswordRequest = VocabularyTrainer.Domain.Models.Auth.ResetPasswordRequest;
using DomainSendCodeRequest = VocabularyTrainer.Domain.Models.Auth.SendVerificationCodeRequest;

namespace VocabularyTrainer.Api.Controllers
{
	[AllowAnonymous]
	[Route("api/auth")]
	public class AuthController(IApiAuthService service, IMapper mapper) : BaseApiController
	{
		/// <summary>Sends a one-time code (registration or password reset) to an email.</summary>
		[HttpPost("codes")]
		[EnableRateLimiting(AuthServiceCollectionExtensions.AuthRateLimitPolicy)]
		public async Task<IActionResult> SendCode([FromBody] SendCodeRequest request)
		{
			var domainRequest = new DomainSendCodeRequest(
				request.Channel,
				request.Target,
				mapper.Map<DomainPurpose>(request.Purpose)
			);

			var result = await service.SendCodeAsync(domainRequest);

			if (!result.Successful)
				return ResolveFailure(result);

			return Accepted();
		}

		[HttpPost("register")]
		[EnableRateLimiting(AuthServiceCollectionExtensions.AuthRateLimitPolicy)]
		public async Task<IActionResult> Register([FromBody] RegisterRequest request)
		{
			var domainRequest = new DomainRegisterRequest(
				request.Channel,
				request.Target,
				request.Code,
				request.Password,
				request.DisplayName,
				mapper.Map<DomainClientInfo>(request.Client)
			);

			var result = await service.RegisterAsync(domainRequest);

			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(mapper.Map<AuthTokensResponse>(result.Value));
		}

		[HttpPost("login")]
		[EnableRateLimiting(AuthServiceCollectionExtensions.AuthRateLimitPolicy)]
		public async Task<IActionResult> Login([FromBody] LoginRequest request)
		{
			var domainRequest = new DomainLoginRequest(request.Login, request.Password, mapper.Map<DomainClientInfo>(request.Client));
			var result = await service.LoginAsync(domainRequest);

			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(mapper.Map<AuthTokensResponse>(result.Value));
		}

		/// <summary>Signs in (creating the account on first use) with a credential issued by an external provider.</summary>
		[HttpPost("external/{provider}")]
		[EnableRateLimiting(AuthServiceCollectionExtensions.AuthRateLimitPolicy)]
		public async Task<IActionResult> ExternalLogin(string provider, [FromBody] ExternalLoginRequest request)
		{
			var domainRequest = new DomainExternalLoginRequest(
				provider,
				request.Credential,
				request.DisplayNameHint,
				mapper.Map<DomainClientInfo>(request.Client)
			);

			var result = await service.LoginExternalAsync(domainRequest);

			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(mapper.Map<AuthTokensResponse>(result.Value));
		}

		/// <summary>Exchanges a refresh token for a new token pair. The presented refresh token becomes invalid.</summary>
		[HttpPost("refresh")]
		public async Task<IActionResult> Refresh([FromBody] RefreshTokenRequest request)
		{
			var result = await service.RefreshAsync(request.RefreshToken);

			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(mapper.Map<AuthTokensResponse>(result.Value));
		}

		[HttpPost("logout")]
		public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
		{
			await service.LogoutAsync(request.RefreshToken);
			return NoContent();
		}

		/// <summary>Sets a new password using a code from <c>POST api/auth/codes</c>; signs out every session.</summary>
		[HttpPost("password/reset")]
		[EnableRateLimiting(AuthServiceCollectionExtensions.AuthRateLimitPolicy)]
		public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
		{
			var domainRequest = new DomainResetPasswordRequest(request.Channel, request.Target, request.Code, request.NewPassword);
			var result = await service.ResetPasswordAsync(domainRequest);

			if (!result.Successful)
				return ResolveFailure(result);

			return NoContent();
		}
	}
}
