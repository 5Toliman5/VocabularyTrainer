using VocabularyTrainer.Domain.Models.Auth;
using VocabularyTrainer.WinApp.Infrastructure;

namespace VocabularyTrainer.WinApp
{
	// Built in code rather than in the designer: layout panels size themselves to the font, so the page
	// stays readable regardless of DPI scaling.
	public partial class MainForm
	{
		private Label _signedInAsValue = null!;
		private TextBox _displayNameTextBox = null!;
		private TextBox _currentPasswordTextBox = null!;
		private TextBox _newPasswordTextBox = null!;
		private TextBox _newPasswordConfirmationTextBox = null!;
		private ListBox _identitiesListBox = null!;
		private TextBox _linkEmailTextBox = null!;
		private TextBox _linkCodeTextBox = null!;

		public event EventHandler? AccountPageEntered;
		public event EventHandler? SaveDisplayNameRequested;
		public event EventHandler? ChangePasswordRequested;
		public event EventHandler? SendLinkEmailCodeRequested;
		public event EventHandler? LinkEmailRequested;
		public event EventHandler? UnlinkIdentityRequested;
		public event EventHandler? SignOutEverywhereRequested;

		public string InputDisplayName => _displayNameTextBox.Text.Trim();
		public string InputCurrentPassword => _currentPasswordTextBox.Text;
		public string InputNewPassword => _newPasswordTextBox.Text;
		public string InputNewPasswordConfirmation => _newPasswordConfirmationTextBox.Text;
		public string InputLinkEmail => _linkEmailTextBox.Text.Trim();
		public string InputLinkCode => _linkCodeTextBox.Text.Trim();

		public int? SelectedIdentityId => (_identitiesListBox.SelectedItem as IdentityItem)?.Id;

		public void SetCurrentUser(string displayName)
		{
			_signedInAsValue.Text = displayName;
			_displayNameTextBox.Text = displayName;
		}

		public void LoadIdentities(IReadOnlyList<UserIdentityDto> identities)
		{
			_identitiesListBox.Items.Clear();

			foreach (var identity in identities)
			{
				_identitiesListBox.Items.Add(new IdentityItem(identity.Id, FormatIdentity(identity)));
			}
		}

		public void ClearPasswordInputs()
		{
			_currentPasswordTextBox.Clear();
			_newPasswordTextBox.Clear();
			_newPasswordConfirmationTextBox.Clear();
		}

		public void ClearLinkEmailInputs()
		{
			_linkEmailTextBox.Clear();
			_linkCodeTextBox.Clear();
		}

		public void ShowInfo(string message) =>
			MessageBox.Show(message, Constants.InfoCaption, MessageBoxButtons.OK, MessageBoxIcon.Information);

		public bool Confirm(string message) =>
			MessageBox.Show(message, Constants.ConfirmCaption, MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes;

		private void InitializeAccountPage()
		{
			var columns = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				RowCount = 1,
				Font = new Font(Font.FontFamily, 10F),
			};

			columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
			columns.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

			columns.Controls.Add(CreateStack(CreateProfileGroup(), CreatePasswordGroup()), 0, 0);
			columns.Controls.Add(CreateStack(CreateSignInMethodsGroup(), CreateSessionsGroup()), 1, 0);

			UserPage.Controls.Add(columns);
			UserPage.Enter += (_, _) => AccountPageEntered?.Invoke(this, EventArgs.Empty);
		}

		private GroupBox CreateProfileGroup()
		{
			var grid = CreateGrid();

			_signedInAsValue = new Label { AutoSize = true, Font = new Font(Font.FontFamily, 10F, FontStyle.Bold) };
			AddRow(grid, "Signed in as", _signedInAsValue);

			_displayNameTextBox = new TextBox { MaxLength = 64 };
			AddRow(grid, "Name", _displayNameTextBox);
			_toolTip.SetToolTip(_displayNameTextBox, "How the app greets you. Visible only to you; 1–64 characters.");

			var saveButton = CreateButton("Save name", () => SaveDisplayNameRequested?.Invoke(this, EventArgs.Empty));
			_toolTip.SetToolTip(saveButton, "Save the new name for this account on all devices.");
			AddButtons(grid, saveButton);

			return CreateGroup("Profile", grid);
		}

		private GroupBox CreatePasswordGroup()
		{
			var grid = CreateGrid();

			_currentPasswordTextBox = new TextBox { UseSystemPasswordChar = true };
			AddRow(grid, "Current password", _currentPasswordTextBox);
			_toolTip.SetToolTip(_currentPasswordTextBox, "Your current password. Leave empty only if the account never had one (e.g. created via Telegram).");

			_newPasswordTextBox = new TextBox { UseSystemPasswordChar = true };
			AddRow(grid, "New password", _newPasswordTextBox);
			_toolTip.SetToolTip(_newPasswordTextBox, "At least 8 characters.");

			_newPasswordConfirmationTextBox = new TextBox { UseSystemPasswordChar = true };
			AddRow(grid, "Repeat new password", _newPasswordConfirmationTextBox);
			_toolTip.SetToolTip(_newPasswordConfirmationTextBox, "Type the new password again to avoid typos.");

			var changeButton = CreateButton("Change password", () => ChangePasswordRequested?.Invoke(this, EventArgs.Empty));
			_toolTip.SetToolTip(changeButton, "Set the new password. This device stays signed in; all other devices are signed out.");
			AddButtons(grid, changeButton);

			return CreateGroup("Password", grid);
		}

		private GroupBox CreateSignInMethodsGroup()
		{
			var grid = CreateGrid();

			_identitiesListBox = new ListBox { Height = 62, IntegralHeight = false };
			grid.Controls.Add(_identitiesListBox);
			grid.SetColumnSpan(_identitiesListBox, 2);
			_identitiesListBox.Dock = DockStyle.Fill;
			_toolTip.SetToolTip(_identitiesListBox, "Ways you can sign in to this account. Any of them opens the same words and dictionaries on every device.");

			var removeButton = CreateButton("Remove selected", () => UnlinkIdentityRequested?.Invoke(this, EventArgs.Empty));
			_toolTip.SetToolTip(removeButton, "Stop using the selected method to sign in. The last remaining method cannot be removed.");
			AddButtons(grid, removeButton);

			_linkEmailTextBox = new TextBox { MaxLength = 254 };
			AddRow(grid, "Add email", _linkEmailTextBox);
			_toolTip.SetToolTip(_linkEmailTextBox, "Another email address you want to sign in with. It must not belong to a different account.");

			var sendCodeButton = CreateButton("Send code", () => SendLinkEmailCodeRequested?.Invoke(this, EventArgs.Empty));
			_toolTip.SetToolTip(sendCodeButton, "Email a 6-digit confirmation code to the address above.");
			AddButtons(grid, sendCodeButton);

			_linkCodeTextBox = new TextBox { MaxLength = 6 };
			AddRow(grid, "Code from email", _linkCodeTextBox);
			_toolTip.SetToolTip(_linkCodeTextBox, "The 6-digit code from the email. It is valid for 10 minutes.");

			var linkButton = CreateButton("Add email", () => LinkEmailRequested?.Invoke(this, EventArgs.Empty));
			_toolTip.SetToolTip(linkButton, "Confirm the code and add the email as a sign-in method.");
			AddButtons(grid, linkButton);

			return CreateGroup("Sign-in methods", grid);
		}

		private GroupBox CreateSessionsGroup()
		{
			var grid = CreateGrid();

			var signOutButton = CreateButton("Sign out", () => LogoutRequested?.Invoke(this, EventArgs.Empty));
			_toolTip.SetToolTip(signOutButton, "Sign out on this computer. Your data stays in your account.");

			var signOutEverywhereButton = CreateButton("Sign out everywhere", () => SignOutEverywhereRequested?.Invoke(this, EventArgs.Empty));
			_toolTip.SetToolTip(signOutEverywhereButton, "Sign out on all devices, including this one. Use it if you think someone else has access.");

			AddButtons(grid, signOutButton, signOutEverywhereButton);

			return CreateGroup("Sessions", grid);
		}

		// onClick reads the event field at click time; passing the event itself here would capture null.
		private static Button CreateButton(string text, Action onClick)
		{
			var button = new Button
			{
				Text = text,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				Padding = new Padding(6, 2, 6, 2),
				UseVisualStyleBackColor = true,
			};

			button.Click += (_, _) => onClick();
			return button;
		}

		private static TableLayoutPanel CreateStack(params Control[] groups)
		{
			var stack = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 1,
			};

			stack.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));

			foreach (var group in groups)
			{
				stack.Controls.Add(group);
			}

			return stack;
		}

		private static TableLayoutPanel CreateGrid()
		{
			var grid = new TableLayoutPanel
			{
				Dock = DockStyle.Fill,
				ColumnCount = 2,
				AutoSize = true,
			};

			grid.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
			grid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
			return grid;
		}

		private static void AddRow(TableLayoutPanel grid, string caption, Control input)
		{
			var label = new Label
			{
				Text = caption,
				AutoSize = true,
				Anchor = AnchorStyles.Left,
				ForeColor = Color.Black,
			};

			input.Anchor = AnchorStyles.Left | AnchorStyles.Right;
			grid.Controls.Add(label);
			grid.Controls.Add(input);
		}

		private static void AddButtons(TableLayoutPanel grid, params Button[] buttons)
		{
			var row = new FlowLayoutPanel
			{
				AutoSize = true,
				Dock = DockStyle.Fill,
				WrapContents = true,
			};

			row.Controls.AddRange(buttons);
			grid.Controls.Add(row);
			grid.SetColumnSpan(row, 2);
		}

		private static GroupBox CreateGroup(string title, Control content)
		{
			var group = new GroupBox
			{
				Text = title,
				Dock = DockStyle.Top,
				AutoSize = true,
				AutoSizeMode = AutoSizeMode.GrowAndShrink,
				Padding = new Padding(8),
				ForeColor = Color.Black,
			};

			group.Controls.Add(content);
			return group;
		}

		private static string FormatIdentity(UserIdentityDto identity)
		{
			var provider = identity.Provider switch
			{
				IdentityProviders.Email => "Email",
				IdentityProviders.Phone => "Phone",
				IdentityProviders.Telegram => "Telegram",
				IdentityProviders.Google => "Google",
				IdentityProviders.Apple => "Apple",
				IdentityProviders.Facebook => "Facebook",
				_ => identity.Provider,
			};

			return $"{provider}: {identity.ProviderKey}";
		}

		private sealed class IdentityItem(int id, string text)
		{
			public int Id => id;
			public override string ToString() => text;
		}
	}
}
