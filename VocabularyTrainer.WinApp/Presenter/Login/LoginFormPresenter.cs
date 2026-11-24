using Common.Wrappers;
using Microsoft.Extensions.Logging;
using VocabularyTrainer.WinApp.ApiClient;
using VocabularyTrainer.WinApp.ApiClient.Auth;
using VocabularyTrainer.WinApp.Infrastructure;
using VocabularyTrainer.WinApp.View;

namespace VocabularyTrainer.WinApp.Presenter.Login
{
	public sealed class LoginFormPresenter(IAccountClient accountClient, ILogger<LoginFormPresenter> logger)
	{
		private ILoginFormView? _view;
		private bool _isBusy;

		// True once the user is signed in (IAccountClient.CurrentUser is set); false if the dialog was closed.
		public bool ShowModal(IWin32Window? owner)
		{
			using var form = new LoginForm();
			_view = form;

			form.SignInRequested += OnSignInRequested;
			form.SendRegistrationCodeRequested += OnSendRegistrationCodeRequested;
			form.RegisterRequested += OnRegisterRequested;
			form.SendResetCodeRequested += OnSendResetCodeRequested;
			form.ResetPasswordRequested += OnResetPasswordRequested;

			try
			{
				return form.ShowDialog(owner) == DialogResult.OK;
			}
			finally
			{
				form.SignInRequested -= OnSignInRequested;
				form.SendRegistrationCodeRequested -= OnSendRegistrationCodeRequested;
				form.RegisterRequested -= OnRegisterRequested;
				form.SendResetCodeRequested -= OnSendResetCodeRequested;
				form.ResetPasswordRequested -= OnResetPasswordRequested;
				_view = null;
			}
		}

		private async void OnSignInRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async view =>
			{
				if (string.IsNullOrEmpty(view.SignInLogin) || string.IsNullOrEmpty(view.SignInPassword))
				{
					view.ShowError(Constants.LoginAndPasswordRequired);
					return;
				}

				var result = await accountClient.LoginAsync(view.SignInLogin, view.SignInPassword);
				CompleteOrShowError(view, result);
			});
		}

		private async void OnSendRegistrationCodeRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(view => SendCodeAsync(view, view.RegisterEmail, accountClient.SendRegistrationCodeAsync));
		}

		private async void OnRegisterRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async view =>
			{
				if (IsAnyEmpty(view.RegisterName, view.RegisterEmail, view.RegisterCode, view.RegisterPassword))
				{
					view.ShowError(Constants.RegistrationFieldsRequired);
					return;
				}

				if (view.RegisterPassword != view.RegisterPasswordConfirmation)
				{
					view.ShowError(Constants.PasswordsDoNotMatch);
					return;
				}

				var result = await accountClient.RegisterAsync(
					view.RegisterEmail,
					view.RegisterCode,
					view.RegisterPassword,
					view.RegisterName
				);

				CompleteOrShowError(view, result);
			});
		}

		private async void OnSendResetCodeRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(view => SendCodeAsync(view, view.ResetEmail, accountClient.SendPasswordResetCodeAsync));
		}

		private async void OnResetPasswordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async view =>
			{
				if (IsAnyEmpty(view.ResetEmail, view.ResetCode, view.ResetNewPassword))
				{
					view.ShowError(Constants.ResetFieldsRequired);
					return;
				}

				var result = await accountClient.ResetPasswordAsync(view.ResetEmail, view.ResetCode, view.ResetNewPassword);

				if (!result.Successful)
				{
					view.ShowError(result.ErrorMessage);
					return;
				}

				view.ShowSignInPage(view.ResetEmail);
				view.ShowInfo(Constants.PasswordResetDone);
			});
		}

		private static async Task SendCodeAsync(ILoginFormView view, string email, Func<string, Task<Result>> send)
		{
			if (string.IsNullOrEmpty(email))
			{
				view.ShowError(Constants.EmailRequired);
				return;
			}

			var result = await send(email);

			if (!result.Successful)
			{
				view.ShowError(result.ErrorMessage);
				return;
			}

			view.ShowInfo(string.Format(Constants.CodeSent, email));
		}

		private static void CompleteOrShowError(ILoginFormView view, Result result)
		{
			if (!result.Successful)
			{
				view.ShowError(result.ErrorMessage);
				return;
			}

			view.CompleteSuccessfully();
		}

		private static bool IsAnyEmpty(params string[] values) => values.Any(string.IsNullOrEmpty);

		private async Task ExecuteIfFreeAsync(Func<ILoginFormView, Task> action)
		{
			var view = _view;

			if (_isBusy || view is null) return;

			try
			{
				_isBusy = true;
				view.SetBusy(true);
				await action(view);
			}
			catch (Exception ex) when (ApiFailures.IsUnreachable(ex))
			{
				logger.LogError(ex, "Auth request failed");
				view.ShowError(Constants.ServerUnavailable);
			}
			catch (Exception ex)
			{
				logger.LogError(ex, "Unexpected error during sign-in");
				view.ShowError(string.Format(Constants.UnexpectedError, ex.Message));
			}
			finally
			{
				_isBusy = false;
				view.SetBusy(false);
			}
		}
	}
}
