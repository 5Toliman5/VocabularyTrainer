namespace VocabularyTrainer.WinApp.Infrastructure
{
	/// <summary>UI string constants and configuration values shared across the application.</summary>
	public static class Constants
	{
		/// <summary>Error message shown when a required text input is empty. Accepts one format argument: the field name.</summary>
		public const string EmptyInput = "Please, enter the {0}.";

		/// <summary>Regex pattern that accepts only letters, combining marks, apostrophes, hyphens, and spaces.</summary>
		public const string InputTextBoxRegex = @"^[\p{L}\p{M}'\- ]+$";

		/// <summary>Button label shown before the user has revealed the translation.</summary>
		public const string DefaultShowNextButtonText = "I remember!";

		/// <summary>Button label shown after the translation has been revealed.</summary>
		public const string ChangedShowNextButtonText = "Next word";

		/// <summary>Error shown when no words are found for the selected dictionary.</summary>
		public const string NoWordsFoundError = "No words have been found for the selected dictionary.";

		/// <summary>Error shown when a username cannot be resolved. Accepts one format argument: the entered name.</summary>
		public const string UserNotFoundError = "User '{0}' has not been found.";

		/// <summary>Error shown when the user submits an empty dictionary name.</summary>
		public const string DictionaryNameRequired = "Dictionary name cannot be empty.";

		/// <summary>Error shown when a dictionary name conflicts with an existing one.</summary>
		public const string DuplicateDictionaryName = "A dictionary with this name already exists.";

		/// <summary>Error shown when the user attempts to delete their only remaining dictionary.</summary>
		public const string CannotDeleteLastDictionary = "Cannot delete the last dictionary.";

		/// <summary>Generic database error message shown to the user.</summary>
		public const string DatabaseError = "A database error occurred. Please check your connection and try again.";

		/// <summary>Fallback error message for unexpected exceptions. Accepts one format argument: the exception message.</summary>
		public const string UnexpectedError = "An unexpected error occurred: {0}";

		/// <summary>Error shown when no dictionaries exist and the user tries to add a word.</summary>
		public const string NoDictionaryAvailable = "No dictionaries found. Please create a dictionary first.";
	}
}
