using Common.Wrappers;
using VocabularyTrainer.Api.BusinessLogic.Services.Abstractions;
using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.Domain.Services;

namespace VocabularyTrainer.Api.BusinessLogic.Services
{
	public class ApiAuthService(IAuthService authService) : IApiAuthService
	{
		public Task<Result> SendCodeAsync(SendVerificationCodeRequest request) => authService.SendCodeAsync(request);

		public Task<Result<AuthTokens>> RegisterAsync(RegisterRequest request) => authService.RegisterAsync(request);

		public Task<Result<AuthTokens>> LoginAsync(PasswordLoginRequest request) => authService.LoginAsync(request);

		public Task<Result<AuthTokens>> LoginExternalAsync(ExternalLoginRequest request) => authService.LoginExternalAsync(request);

		public Task<Result<AuthTokens>> RefreshAsync(string refreshToken) => authService.RefreshAsync(refreshToken);

		public Task LogoutAsync(string refreshToken) => authService.LogoutAsync(refreshToken);

		public Task<Result> ResetPasswordAsync(ResetPasswordRequest request) => authService.ResetPasswordAsync(request);

		public Task<Result> ChangePasswordAsync(ChangePasswordRequest request) => authService.ChangePasswordAsync(request);

		public Task RevokeAllSessionsAsync(int userId) => authService.RevokeAllSessionsAsync(userId);
	}
}
