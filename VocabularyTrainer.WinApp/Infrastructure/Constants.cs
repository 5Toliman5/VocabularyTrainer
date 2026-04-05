namespace VocabularyTrainer.WinApp.Infrastructure
{
	public static class Constants
	{
		public const string EmptyInput = "Please, enter the {0}.";
		public const string InputTextBoxRegex = @"\b[\p{L}\p{M}'-]+\b";
		public const string DefaultShowNextButtonText = "I remember!";
		public const string ChangedShowNextButtonText = "Next word";
		public const string NoWordsFoundError = "No words have been found for the selected dictionary.";
		public const string UserNotFoundError = "User '{0}' has not been found.";
		public const string DictionaryNameRequired = "Dictionary name cannot be empty.";
		public const string DuplicateDictionaryName = "A dictionary with this name already exists.";
		public const string CannotDeleteLastDictionary = "Cannot delete the last dictionary.";
		public const string DatabaseError = "A database error occurred. Please check your connection and try again.";
		public const string UnexpectedError = "An unexpected error occurred: {0}";
		public const string NoDictionaryAvailable = "No dictionaries found. Please create a dictionary first.";
	}
}
