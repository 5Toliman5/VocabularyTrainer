using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp.View
{
	internal sealed class LoginForm : Form, ILoginFormView
	{
		private const int LabelLeft = 16;
		private const int InputLeft = 150;
		private const int InputWidth = 270;
		private const int RowHeight = 40;

		private readonly TabControl _tabs;
		private readonly TabPage _signInPage;
		private readonly Label _statusLabel;
		private readonly Dictionary<TabPage, Button> _acceptButtons;
		private readonly ToolTip _toolTip = new() { AutoPopDelay = 15000, InitialDelay = 500, ShowAlways = true };

		private readonly TextBox _signInLogin;
		private readonly TextBox _signInPassword;

		private readonly TextBox _registerName;
		private readonly TextBox _registerEmail;
		private readonly TextBox _registerCode;
		private readonly TextBox _registerPassword;
		private readonly TextBox _registerPasswordConfirmation;

		private readonly TextBox _resetEmail;
		private readonly TextBox _resetCode;
		private readonly TextBox _resetNewPassword;

		public event EventHandler? SignInRequested;
		public event EventHandler? SendRegistrationCodeRequested;
		public event EventHandler? RegisterRequested;
		public event EventHandler? SendResetCodeRequested;
		public event EventHandler? ResetPasswordRequested;

		public string SignInLogin => _signInLogin.Text.Trim();
		public string SignInPassword => _signInPassword.Text;

		public string RegisterName => _registerName.Text.Trim();
		public string RegisterEmail => _registerEmail.Text.Trim();
		public string RegisterCode => _registerCode.Text.Trim();
		public string RegisterPassword => _registerPassword.Text;
		public string RegisterPasswordConfirmation => _registerPasswordConfirmation.Text;

		public string ResetEmail => _resetEmail.Text.Trim();
		public string ResetCode => _resetCode.Text.Trim();
		public string ResetNewPassword => _resetNewPassword.Text;

		public LoginForm()
		{
			Text = Constants.LoginDialogTitle;
			FormBorderStyle = FormBorderStyle.FixedDialog;
			MaximizeBox = false;
			MinimizeBox = false;
			StartPosition = FormStartPosition.CenterScreen;
			ClientSize = new Size(470, 380);

			_tabs = new TabControl
			{
				Location = new Point(8, 8),
				Size = new Size(454, 310),
			};

			_signInPage = new TabPage("Sign in");
			_signInLogin = AddRow(_signInPage, 0, "Email");
			_signInPassword = AddRow(_signInPage, 1, "Password", isPassword: true);
			var signInButton = AddButton(_signInPage, 2, "Sign in", (_, _) => SignInRequested?.Invoke(this, EventArgs.Empty));

			var registerPage = new TabPage("Create account");
			_registerName = AddRow(registerPage, 0, "Name");
			_registerEmail = AddRow(registerPage, 1, "Email");
			var registerSendCodeButton = AddButton(registerPage, 2, "Send code", (_, _) => SendRegistrationCodeRequested?.Invoke(this, EventArgs.Empty));
			_registerCode = AddRow(registerPage, 3, "Code from email");
			_registerPassword = AddRow(registerPage, 4, "Password", isPassword: true);
			_registerPasswordConfirmation = AddRow(registerPage, 5, "Repeat password", isPassword: true);
			var registerButton = AddButton(registerPage, 6, "Create account", (_, _) => RegisterRequested?.Invoke(this, EventArgs.Empty));

			var resetPage = new TabPage("Forgot password");
			_resetEmail = AddRow(resetPage, 0, "Email");
			var resetSendCodeButton = AddButton(resetPage, 1, "Send code", (_, _) => SendResetCodeRequested?.Invoke(this, EventArgs.Empty));
			_resetCode = AddRow(resetPage, 2, "Code from email");
			_resetNewPassword = AddRow(resetPage, 3, "New password", isPassword: true);
			var resetButton = AddButton(resetPage, 4, "Reset password", (_, _) => ResetPasswordRequested?.Invoke(this, EventArgs.Empty));

			_tabs.TabPages.AddRange([_signInPage, registerPage, resetPage]);

			_acceptButtons = new Dictionary<TabPage, Button>
			{
				[_signInPage] = signInButton,
				[registerPage] = registerButton,
				[resetPage] = resetButton,
			};

			_tabs.SelectedIndexChanged += OnTabChanged;

			_statusLabel = new Label
			{
				Location = new Point(16, 326),
				Size = new Size(440, 46),
				ForeColor = Color.Black,
			};

			Controls.AddRange([_tabs, _statusLabel]);
			AcceptButton = signInButton;

			_toolTip.SetToolTip(_signInLogin, "The email you registered with (or any email added to your account).");
			_toolTip.SetToolTip(_signInPassword, "Your account password.");
			_toolTip.SetToolTip(signInButton, "Sign in. You stay signed in on this computer until you sign out.");

			_toolTip.SetToolTip(_registerName, "How the app should call you. You can change it later on the Account tab.");
			_toolTip.SetToolTip(_registerEmail, "Your email. It is used to sign in and to recover the password.");
			_toolTip.SetToolTip(registerSendCodeButton, "Email a 6-digit confirmation code to the address above.");
			_toolTip.SetToolTip(_registerCode, "The 6-digit code from the email. It is valid for 10 minutes.");
			_toolTip.SetToolTip(_registerPassword, "At least 8 characters.");
			_toolTip.SetToolTip(_registerPasswordConfirmation, "Type the password again to avoid typos.");
			_toolTip.SetToolTip(registerButton, "Create the account and sign in.");

			_toolTip.SetToolTip(_resetEmail, "The email of your account.");
			_toolTip.SetToolTip(resetSendCodeButton, "Email a 6-digit code to reset the password.");
			_toolTip.SetToolTip(_resetCode, "The 6-digit code from the email. It is valid for 10 minutes.");
			_toolTip.SetToolTip(_resetNewPassword, "At least 8 characters. All devices will be signed out.");
			_toolTip.SetToolTip(resetButton, "Set the new password, then sign in with it.");
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				_toolTip.Dispose();
			}

			base.Dispose(disposing);
		}

		public void SetBusy(bool busy)
		{
			_tabs.Enabled = !busy;
			UseWaitCursor = busy;
		}

		public void ShowError(string message)
		{
			_statusLabel.ForeColor = Color.Firebrick;
			_statusLabel.Text = message;
		}

		public void ShowInfo(string message)
		{
			_statusLabel.ForeColor = Color.DarkGreen;
			_statusLabel.Text = message;
		}

		public void ShowSignInPage(string login)
		{
			_tabs.SelectedTab = _signInPage;
			_signInLogin.Text = login;
			_signInPassword.Clear();
			_signInPassword.Focus();
		}

		public void CompleteSuccessfully()
		{
			DialogResult = DialogResult.OK;
			Close();
		}

		private void OnTabChanged(object? sender, EventArgs e)
		{
			_statusLabel.Text = string.Empty;

			if (_tabs.SelectedTab is not null && _acceptButtons.TryGetValue(_tabs.SelectedTab, out var button))
			{
				AcceptButton = button;
			}
		}

		private static TextBox AddRow(TabPage page, int row, string caption, bool isPassword = false)
		{
			var top = 16 + row * RowHeight;

			var label = new Label
			{
				Text = caption,
				Location = new Point(LabelLeft, top + 3),
				AutoSize = true,
				ForeColor = Color.Black,
			};

			var textBox = new TextBox
			{
				Location = new Point(InputLeft, top),
				Size = new Size(InputWidth, 24),
				UseSystemPasswordChar = isPassword,
			};

			page.Controls.AddRange([label, textBox]);
			return textBox;
		}

		private static Button AddButton(TabPage page, int row, string text, EventHandler onClick)
		{
			var button = new Button
			{
				Text = text,
				Location = new Point(InputLeft, 14 + row * RowHeight),
				Size = new Size(150, 30),
			};

			button.Click += onClick;
			page.Controls.Add(button);
			return button;
		}
	}
}
