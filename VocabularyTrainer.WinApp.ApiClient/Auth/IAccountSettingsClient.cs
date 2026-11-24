using Common.Wrappers;
using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.WinApp.ApiClient.Auth
{
	// Settings of the signed-in account (api/me). Calls go through the authenticated HttpClient.
	public interface IAccountSettingsClient
	{
		Task<List<UserIdentityDto>> GetIdentitiesAsync();

		Task<Result> UpdateDisplayNameAsync(string displayName);

		// Keeps this device signed in and signs out every other one.
		Task<Result> ChangePasswordAsync(string currentPassword, string newPassword);

		Task<Result> SendLinkEmailCodeAsync(string email);

		Task<Result> LinkEmailAsync(string email, string code);

		Task<Result> UnlinkIdentityAsync(int identityId);

		// Signs out every device, this one included.
		Task SignOutEverywhereAsync();
	}
}
