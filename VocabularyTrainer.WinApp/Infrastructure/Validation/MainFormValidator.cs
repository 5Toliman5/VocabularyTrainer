using System.Text.RegularExpressions;

namespace VocabularyTrainer.WinApp.Infrastructure.Validation
{
    internal static partial class MainFormValidator
    {
        public static bool ValidateTextBoxInput(TextBox textBox)
        {
            var regex = TextBoxValidationRegex();
            if (string.IsNullOrEmpty(textBox.Text)) return true;
            return regex.IsMatch(textBox.Text);
        }

        [GeneratedRegex(Constants.InputTextBoxRegex)]
        private static partial Regex TextBoxValidationRegex();
    }
}
