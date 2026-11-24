namespace VocabularyTrainer.WinApp.View
{
	public interface ILoginFormView
	{
		event EventHandler? SignInRequested;
		event EventHandler? SendRegistrationCodeRequested;
		event EventHandler? RegisterRequested;
		event EventHandler? SendResetCodeRequested;
		event EventHandler? ResetPasswordRequested;

		string SignInLogin { get; }
		string SignInPassword { get; }

		string RegisterName { get; }
		string RegisterEmail { get; }
		string RegisterCode { get; }
		string RegisterPassword { get; }
		string RegisterPasswordConfirmation { get; }

		string ResetEmail { get; }
		string ResetCode { get; }
		string ResetNewPassword { get; }

		void SetBusy(bool busy);
		void ShowError(string message);
		void ShowInfo(string message);
		void ShowSignInPage(string login);
		void CompleteSuccessfully();
	}
}
