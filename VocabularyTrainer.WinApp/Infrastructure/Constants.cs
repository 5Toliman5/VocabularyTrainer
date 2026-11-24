namespace VocabularyTrainer.WinApp.Infrastructure
{
	public static class Constants
	{
		public const string EmptyInput = "Please, enter the {0}.";
		public const string TranslationRequired = "At least one translation is required.";
		public const string InputTextBoxRegex = @"^[\p{L}\p{M}'\- ]+$";
		public const string DefaultShowNextButtonText = "I remember!";
		public const string ChangedShowNextButtonText = "Next word";
		public const string NoWordsFoundError = "No words have been found for the selected dictionary.";
		public const string DictionaryNameRequired = "Dictionary name cannot be empty.";
		public const string AddDictionaryDialogTitle = "Add Dictionary";
		public const string ValidationCaption = "Validation";
		public const string DuplicateDictionaryName = "A dictionary with this name already exists.";
		public const string CannotDeleteLastDictionary = "Cannot delete the last dictionary.";
		public const string DeleteDictionaryConfirmation = "Delete dictionary '{0}' and all its words ({1})? Their training progress is lost too. This cannot be undone.";
		public const string DatabaseError = "A database error occurred. Please check your connection and try again.";
		public const string UnexpectedError = "An unexpected error occurred: {0}";
		public const string NoDictionaryAvailable = "No dictionaries found. Please create a dictionary first.";
		public const string DeleteWordConfirmation = "Delete '{0}'?";
		public const string NoWordsMatchFilter = "No words match the current filters.";
		public const int MyWordsPageSize = 10;

		public const string ClientType = "WinApp";
		public const string LoginDialogTitle = "Vocabulary Trainer — Sign in";
		public const string SessionExpired = "Your session has expired. Please sign in again.";
		public const string ServerUnavailable = "Cannot reach the server. Please check your connection and try again.";
		public const string LoginAndPasswordRequired = "Please enter your email and password.";
		public const string EmailRequired = "Please enter your email.";
		public const string RegistrationFieldsRequired = "Please fill in name, email, code and password.";
		public const string ResetFieldsRequired = "Please fill in email, code and new password.";
		public const string PasswordsDoNotMatch = "Passwords do not match.";
		public const string CodeSent = "A code has been sent to {0}. It is valid for 10 minutes.";
		public const string PasswordResetDone = "Password changed. Please sign in with the new password.";

		public const string InfoCaption = "Vocabulary Trainer";
		public const string ConfirmCaption = "Please confirm";
		public const string DisplayNameRequired = "Please enter a name.";
		public const string DisplayNameSaved = "Name saved.";
		public const string NewPasswordRequired = "Please enter a new password.";
		public const string PasswordChanged = "Password changed. You stay signed in here; other devices were signed out.";
		public const string LinkEmailFieldsRequired = "Please enter the email and the code from it.";
		public const string EmailLinked = "Email added. You can now sign in with it.";
		public const string SelectSignInMethod = "Please select a sign-in method in the list.";
		public const string UnlinkIdentityConfirmation = "Remove this sign-in method? You will no longer be able to sign in with it.";
		public const string SignOutEverywhereConfirmation = "Sign out on all devices, including this one?";
	}
}
