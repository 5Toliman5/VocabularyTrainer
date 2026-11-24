using Common.Wrappers;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.Domain.Services
{
	public interface IAuthService
	{
		Task<Result> SendCodeAsync(SendVerificationCodeRequest request);

		Task<Result<AuthTokens>> RegisterAsync(RegisterRequest request);

		Task<Result<AuthTokens>> LoginAsync(PasswordLoginRequest request);

		Task<Result<AuthTokens>> LoginExternalAsync(ExternalLoginRequest request);

		Task<Result<AuthTokens>> RefreshAsync(string refreshToken);

		Task LogoutAsync(string refreshToken);

		Task<Result> ResetPasswordAsync(ResetPasswordRequest request);

		Task<Result> ChangePasswordAsync(ChangePasswordRequest request);

		Task RevokeAllSessionsAsync(int userId);
	}
}
