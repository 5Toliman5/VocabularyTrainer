using Common.Wrappers;
using VocabularyTrainer.Domain.Models;

namespace VocabularyTrainer.WinApp.ApiClient.Auth
{
	public interface IAccountClient
	{
		UserModel? CurrentUser { get; }

		// Signs in silently with the stored refresh token. False when there is none or it is no longer valid.
		Task<bool> TryRestoreSessionAsync();

		Task<Result> LoginAsync(string login, string password);

		Task<Result> SendRegistrationCodeAsync(string email);

		Task<Result> RegisterAsync(string email, string code, string password, string displayName);

		Task<Result> SendPasswordResetCodeAsync(string email);

		Task<Result> ResetPasswordAsync(string email, string code, string newPassword);

		Task LogoutAsync();
	}
}
