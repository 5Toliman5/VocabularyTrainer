using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp.Presenter.Main
{
	public partial class MainFormPresenter
	{
		private async void OnAccountPageEntered(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(RefreshIdentitiesAsync);
		}

		private async void OnSaveDisplayNameRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				var displayName = _view.InputDisplayName;

				if (string.IsNullOrEmpty(displayName))
				{
					_view.ShowError(Constants.DisplayNameRequired);
					return;
				}

				var result = await _accountSettingsClient.UpdateDisplayNameAsync(displayName);

				if (!result.Successful)
				{
					_view.ShowError(result.ErrorMessage);
					return;
				}

				_user = _accountClient.CurrentUser ?? _user;
				_view.SetCurrentUser(_user.DisplayName);
				_view.ShowInfo(Constants.DisplayNameSaved);
			});
		}

		private async void OnChangePasswordRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (string.IsNullOrEmpty(_view.InputNewPassword))
				{
					_view.ShowError(Constants.NewPasswordRequired);
					return;
				}

				if (_view.InputNewPassword != _view.InputNewPasswordConfirmation)
				{
					_view.ShowError(Constants.PasswordsDoNotMatch);
					return;
				}

				var result = await _accountSettingsClient.ChangePasswordAsync(_view.InputCurrentPassword, _view.InputNewPassword);

				if (!result.Successful)
				{
					_view.ShowError(result.ErrorMessage);
					return;
				}

				_view.ClearPasswordInputs();
				_view.ShowInfo(Constants.PasswordChanged);
			});
		}

		private async void OnSendLinkEmailCodeRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				var email = _view.InputLinkEmail;

				if (string.IsNullOrEmpty(email))
				{
					_view.ShowError(Constants.EmailRequired);
					return;
				}

				var result = await _accountSettingsClient.SendLinkEmailCodeAsync(email);

				if (!result.Successful)
				{
					_view.ShowError(result.ErrorMessage);
					return;
				}

				_view.ShowInfo(string.Format(Constants.CodeSent, email));
			});
		}

		private async void OnLinkEmailRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (string.IsNullOrEmpty(_view.InputLinkEmail) || string.IsNullOrEmpty(_view.InputLinkCode))
				{
					_view.ShowError(Constants.LinkEmailFieldsRequired);
					return;
				}

				var result = await _accountSettingsClient.LinkEmailAsync(_view.InputLinkEmail, _view.InputLinkCode);

				if (!result.Successful)
				{
					_view.ShowError(result.ErrorMessage);
					return;
				}

				_view.ClearLinkEmailInputs();
				await RefreshIdentitiesAsync();
				_view.ShowInfo(Constants.EmailLinked);
			});
		}

		private async void OnUnlinkIdentityRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				var identityId = _view.SelectedIdentityId;

				if (identityId is null)
				{
					_view.ShowError(Constants.SelectSignInMethod);
					return;
				}

				if (!_view.Confirm(Constants.UnlinkIdentityConfirmation)) return;

				var result = await _accountSettingsClient.UnlinkIdentityAsync(identityId.Value);

				if (!result.Successful)
				{
					_view.ShowError(result.ErrorMessage);
					return;
				}

				await RefreshIdentitiesAsync();
			});
		}

		private async void OnSignOutEverywhereRequested(object? sender, EventArgs e)
		{
			await ExecuteIfFreeAsync(async () =>
			{
				if (!_view.Confirm(Constants.SignOutEverywhereConfirmation)) return;

				await _accountSettingsClient.SignOutEverywhereAsync();
				_view.RestartApplication();
			});
		}

		private async Task RefreshIdentitiesAsync()
		{
			var identities = await _accountSettingsClient.GetIdentitiesAsync();
			_view.LoadIdentities(identities);
		}
	}
}
