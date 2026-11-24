using System.Net.Http.Json;
using AutoMapper;
using Common.Wrappers;
using VocabularyTrainer.Domain.Models;
using VocabularyTrainer.Domain.Models.Auth;
using CChangePasswordRequest = VocabularyTrainer.Api.Contract.Users.ChangePasswordRequest;
using CLinkIdentityRequest = VocabularyTrainer.Api.Contract.Users.LinkIdentityRequest;
using CSendLinkCodeRequest = VocabularyTrainer.Api.Contract.Users.SendLinkCodeRequest;
using CUpdateProfileRequest = VocabularyTrainer.Api.Contract.Users.UpdateProfileRequest;
using CUserIdentityResponse = VocabularyTrainer.Api.Contract.Users.UserIdentityResponse;
using CUserResponse = VocabularyTrainer.Api.Contract.Users.UserResponse;

namespace VocabularyTrainer.WinApp.ApiClient.Auth
{
	internal sealed class AccountSettingsClient(HttpClient httpClient, AccountClient accountClient, IMapper mapper)
		: IAccountSettingsClient
	{
		public async Task<List<UserIdentityDto>> GetIdentitiesAsync()
		{
			var identities = await httpClient.GetFromJsonAsync<List<CUserIdentityResponse>>("api/me/identities") ?? [];
			return mapper.Map<List<UserIdentityDto>>(identities);
		}

		public async Task<Result> UpdateDisplayNameAsync(string displayName)
		{
			using var response = await httpClient.PutAsJsonAsync("api/me", new CUpdateProfileRequest(displayName));
			var failure = await ApiFailures.ToFailureAsync(response);

			if (failure is not null) return failure;

			var user = await response.Content.ReadFromJsonAsync<CUserResponse>();
			accountClient.UpdateCurrentUser(mapper.Map<UserModel>(user!));

			return Result.Success();
		}

		public async Task<Result> ChangePasswordAsync(string currentPassword, string newPassword)
		{
			var request = new CChangePasswordRequest(currentPassword, newPassword, accountClient.RefreshToken);
			using var response = await httpClient.PostAsJsonAsync("api/me/password", request);

			return await ApiFailures.ToFailureAsync(response) ?? Result.Success();
		}

		public async Task<Result> SendLinkEmailCodeAsync(string email)
		{
			var request = new CSendLinkCodeRequest(IdentityProviders.Email, email);
			using var response = await httpClient.PostAsJsonAsync("api/me/identities/codes", request);

			return await ApiFailures.ToFailureAsync(response) ?? Result.Success();
		}

		public async Task<Result> LinkEmailAsync(string email, string code)
		{
			var request = new CLinkIdentityRequest(IdentityProviders.Email, email, code);
			using var response = await httpClient.PostAsJsonAsync("api/me/identities", request);

			return await ApiFailures.ToFailureAsync(response) ?? Result.Success();
		}

		public async Task<Result> UnlinkIdentityAsync(int identityId)
		{
			using var response = await httpClient.DeleteAsync($"api/me/identities/{identityId}");

			return await ApiFailures.ToFailureAsync(response) ?? Result.Success();
		}

		public async Task SignOutEverywhereAsync()
		{
			using var response = await httpClient.PostAsync("api/me/sessions/revoke", content: null);
			response.EnsureSuccessStatusCode();

			accountClient.ForgetSession();
		}
	}
}
