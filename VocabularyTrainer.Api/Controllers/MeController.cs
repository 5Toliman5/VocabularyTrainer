using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Api.Contract.Users;
using VocabularyTrainer.Api.Infrastructure;
using DomainChangePasswordRequest = VocabularyTrainer.Domain.Models.Auth.ChangePasswordRequest;
using DomainLinkIdentityRequest = VocabularyTrainer.Domain.Models.Auth.LinkIdentityRequest;

namespace VocabularyTrainer.Api.Controllers
{
	[Route("api/me")]
	public class MeController(IApiUserService service, IApiAuthService authService, IMapper mapper) : BaseApiController
	{
		[HttpGet]
		public async Task<IActionResult> Get()
		{
			var result = await service.GetByIdAsync(CurrentUserId);

			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(mapper.Map<UserResponse>(result.Value));
		}

		[HttpPut]
		public async Task<IActionResult> UpdateProfile([FromBody] UpdateProfileRequest request)
		{
			var result = await service.UpdateDisplayNameAsync(CurrentUserId, request.DisplayName);

			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(mapper.Map<UserResponse>(result.Value));
		}

		/// <summary>Changes the password and signs out every other session of this account.</summary>
		[HttpPost("password")]
		public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
		{
			var domainRequest = new DomainChangePasswordRequest(
				CurrentUserId,
				request.CurrentPassword,
				request.NewPassword,
				request.CurrentRefreshToken
			);

			var result = await authService.ChangePasswordAsync(domainRequest);

			if (!result.Successful)
				return ResolveFailure(result);

			return NoContent();
		}

		/// <summary>Signs out every session of this account, including the caller's.</summary>
		[HttpPost("sessions/revoke")]
		public async Task<IActionResult> RevokeAllSessions()
		{
			await authService.RevokeAllSessionsAsync(CurrentUserId);
			return NoContent();
		}

		[HttpGet("identities")]
		public async Task<IEnumerable<UserIdentityResponse>> GetIdentities()
		{
			var identities = await service.GetIdentitiesAsync(CurrentUserId);
			return mapper.Map<IEnumerable<UserIdentityResponse>>(identities);
		}

		/// <summary>Sends a code to an email that should be added to the current account as a sign-in method.</summary>
		[HttpPost("identities/codes")]
		public async Task<IActionResult> SendLinkCode([FromBody] SendLinkCodeRequest request)
		{
			var result = await service.SendLinkCodeAsync(CurrentUserId, request.Channel, request.Target);

			if (!result.Successful)
				return ResolveFailure(result);

			return Accepted();
		}

		[HttpPost("identities")]
		public async Task<IActionResult> LinkIdentity([FromBody] LinkIdentityRequest request)
		{
			var domainRequest = new DomainLinkIdentityRequest(CurrentUserId, request.Channel, request.Target, request.Code);
			var result = await service.LinkIdentityAsync(domainRequest);

			if (!result.Successful)
				return ResolveFailure(result);

			return Ok(mapper.Map<UserIdentityResponse>(result.Value));
		}

		[HttpDelete("identities/{identityId:int}")]
		public async Task<IActionResult> UnlinkIdentity(int identityId)
		{
			var result = await service.UnlinkIdentityAsync(CurrentUserId, identityId);

			if (!result.Successful)
				return ResolveFailure(result);

			return NoContent();
		}
	}
}
