using System.Text.RegularExpressions;

namespace VocabularyTrainer.WinApp.Infrastructure.Validation
{
    internal static partial class MainFormValidator
    {
        /// <summary>Returns <c>true</c> when the text box is empty or its content matches the allowed input pattern; <c>false</c> otherwise.</summary>
        public static bool ValidateTextBoxInput(TextBox textBox)
        {
            var regex = TextBoxValidationRegex();
            if (string.IsNullOrEmpty(textBox.Text)) return true;
            return regex.IsMatch(textBox.Text);
        }

        /// <summary>Source-generated regex compiled from <see cref="Constants.InputTextBoxRegex"/>.</summary>
        [GeneratedRegex(Constants.InputTextBoxRegex)]
        private static partial Regex TextBoxValidationRegex();
    }
}
