using VocabularyTrainer.Domain.Models.Auth;

namespace VocabularyTrainer.WinApp.View
{
	public partial interface IMainFormView
	{
		event EventHandler? AccountPageEntered;
		event EventHandler? SaveDisplayNameRequested;
		event EventHandler? ChangePasswordRequested;
		event EventHandler? SendLinkEmailCodeRequested;
		event EventHandler? LinkEmailRequested;
		event EventHandler? UnlinkIdentityRequested;
		event EventHandler? SignOutEverywhereRequested;

		string InputDisplayName { get; }
		string InputCurrentPassword { get; }
		string InputNewPassword { get; }
		string InputNewPasswordConfirmation { get; }
		string InputLinkEmail { get; }
		string InputLinkCode { get; }
		int? SelectedIdentityId { get; }

		void LoadIdentities(IReadOnlyList<UserIdentityDto> identities);
		void ClearPasswordInputs();
		void ClearLinkEmailInputs();
		void ShowInfo(string message);
		bool Confirm(string message);
	}
}
