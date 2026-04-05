namespace WinApp
{
	partial class MainForm
	{
		/// <summary>
		///  Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		///  Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            MainTabControl = new TabControl();
            TrainYourselfPage = new TabPage();
            DictionaryOfWordLabel = new Label();
            DictionaryTrainingLabel = new Label();
            DictionaryTrainingComboBox = new ComboBox();
            DeleteWordButton = new Button();
            DisplayTranslationTextBox = new TextBox();
            ShowNextButton = new Button();
            ShowTranslationButton = new Button();
            DisplayWordTextBox = new TextBox();
            AddNewWordsPage = new TabPage();
            DictionaryAddingLabel = new Label();
            DictionaryAddingComboBox = new ComboBox();
            AddDataButton = new Button();
            EnterHereLabel = new Label();
            TranslationLabel = new Label();
            WordToLearnLabel = new Label();
            InputTranslationTextBox = new TextBox();
            InputWordTextBox = new TextBox();
            MyWordsPage = new TabPage();
            WordCountLabel = new Label();
            DictionariesListBox = new ListBox();
            DictNameLabel = new Label();
            DictionaryNameInputTextBox = new TextBox();
            DictLanguageLabel = new Label();
            LanguageComboBox = new ComboBox();
            AddDictionaryButton = new Button();
            UpdateDictionaryButton = new Button();
            DeleteDictionaryButton = new Button();
            UserPage = new TabPage();
            textBox1 = new TextBox();
            CurrentUserTextBox = new TextBox();
            CurrentUserLabel = new Label();
            AddWordsErrorProvider = new ErrorProvider(components);
            MainTabControl.SuspendLayout();
            TrainYourselfPage.SuspendLayout();
            AddNewWordsPage.SuspendLayout();
            MyWordsPage.SuspendLayout();
            UserPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AddWordsErrorProvider).BeginInit();
            SuspendLayout();
            // 
            // MainTabControl
            // 
            MainTabControl.Controls.Add(TrainYourselfPage);
            MainTabControl.Controls.Add(AddNewWordsPage);
            MainTabControl.Controls.Add(MyWordsPage);
            MainTabControl.Controls.Add(UserPage);
            MainTabControl.Location = new Point(2, 3);
            MainTabControl.Name = "MainTabControl";
            MainTabControl.SelectedIndex = 0;
            MainTabControl.Size = new Size(799, 423);
            MainTabControl.TabIndex = 0;
            // 
            // TrainYourselfPage
            // 
            TrainYourselfPage.BackColor = Color.Transparent;
            TrainYourselfPage.Controls.Add(DictionaryOfWordLabel);
            TrainYourselfPage.Controls.Add(DictionaryTrainingLabel);
            TrainYourselfPage.Controls.Add(DictionaryTrainingComboBox);
            TrainYourselfPage.Controls.Add(DeleteWordButton);
            TrainYourselfPage.Controls.Add(DisplayTranslationTextBox);
            TrainYourselfPage.Controls.Add(ShowNextButton);
            TrainYourselfPage.Controls.Add(ShowTranslationButton);
            TrainYourselfPage.Controls.Add(DisplayWordTextBox);
            TrainYourselfPage.ForeColor = Color.Black;
            TrainYourselfPage.Location = new Point(4, 42);
            TrainYourselfPage.Name = "TrainYourselfPage";
            TrainYourselfPage.Padding = new Padding(3);
            TrainYourselfPage.Size = new Size(791, 377);
            TrainYourselfPage.TabIndex = 0;
            TrainYourselfPage.Text = "Train yourself";
            TrainYourselfPage.Enter += TrainYourSelfPageEnter;
            // 
            // DictionaryOfWordLabel
            // 
            DictionaryOfWordLabel.Font = new Font("Comic Sans MS", 9F, FontStyle.Italic);
            DictionaryOfWordLabel.ForeColor = Color.Gray;
            DictionaryOfWordLabel.Location = new Point(101, 142);
            DictionaryOfWordLabel.Name = "DictionaryOfWordLabel";
            DictionaryOfWordLabel.Size = new Size(609, 20);
            DictionaryOfWordLabel.TabIndex = 12;
            DictionaryOfWordLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DictionaryTrainingLabel
            // 
            DictionaryTrainingLabel.AutoSize = true;
            DictionaryTrainingLabel.ForeColor = Color.Black;
            DictionaryTrainingLabel.Location = new Point(6, 17);
            DictionaryTrainingLabel.Name = "DictionaryTrainingLabel";
            DictionaryTrainingLabel.Size = new Size(130, 33);
            DictionaryTrainingLabel.TabIndex = 10;
            DictionaryTrainingLabel.Text = "Dictionary";
            // 
            // DictionaryTrainingComboBox
            // 
            DictionaryTrainingComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DictionaryTrainingComboBox.ForeColor = Color.Black;
            DictionaryTrainingComboBox.Location = new Point(145, 12);
            DictionaryTrainingComboBox.Name = "DictionaryTrainingComboBox";
            DictionaryTrainingComboBox.Size = new Size(290, 41);
            DictionaryTrainingComboBox.TabIndex = 11;
            DictionaryTrainingComboBox.SelectedIndexChanged += DictionaryTrainingComboBox_SelectedIndexChanged;
            // 
            // DeleteWordButton
            // 
            DeleteWordButton.Location = new Point(314, 306);
            DeleteWordButton.Name = "DeleteWordButton";
            DeleteWordButton.Size = new Size(189, 42);
            DeleteWordButton.TabIndex = 4;
            DeleteWordButton.Text = "Delete";
            DeleteWordButton.UseVisualStyleBackColor = true;
            DeleteWordButton.Click += DeleteWord;
            // 
            // DisplayTranslationTextBox
            // 
            DisplayTranslationTextBox.BackColor = SystemColors.Control;
            DisplayTranslationTextBox.Font = new Font("Comic Sans MS", 20.25F);
            DisplayTranslationTextBox.Location = new Point(101, 166);
            DisplayTranslationTextBox.Name = "DisplayTranslationTextBox";
            DisplayTranslationTextBox.ReadOnly = true;
            DisplayTranslationTextBox.Size = new Size(609, 55);
            DisplayTranslationTextBox.TabIndex = 3;
            DisplayTranslationTextBox.TextAlign = HorizontalAlignment.Center;
            DisplayTranslationTextBox.KeyDown += TextBox_SwitchFocus;
            // 
            // ShowNextButton
            // 
            ShowNextButton.Location = new Point(435, 233);
            ShowNextButton.Name = "ShowNextButton";
            ShowNextButton.Size = new Size(275, 42);
            ShowNextButton.TabIndex = 2;
            ShowNextButton.Text = "I remember!";
            ShowNextButton.UseVisualStyleBackColor = true;
            ShowNextButton.Click += ShowNextWord;
            // 
            // ShowTranslationButton
            // 
            ShowTranslationButton.Location = new Point(101, 233);
            ShowTranslationButton.Name = "ShowTranslationButton";
            ShowTranslationButton.Size = new Size(275, 42);
            ShowTranslationButton.TabIndex = 1;
            ShowTranslationButton.Text = "Translation";
            ShowTranslationButton.UseVisualStyleBackColor = true;
            ShowTranslationButton.Click += ShowTranslation;
            // 
            // DisplayWordTextBox
            // 
            DisplayWordTextBox.Font = new Font("Comic Sans MS", 20.25F);
            DisplayWordTextBox.Location = new Point(101, 83);
            DisplayWordTextBox.Name = "DisplayWordTextBox";
            DisplayWordTextBox.ReadOnly = true;
            DisplayWordTextBox.Size = new Size(609, 55);
            DisplayWordTextBox.TabIndex = 0;
            DisplayWordTextBox.TextAlign = HorizontalAlignment.Center;
            DisplayWordTextBox.KeyDown += TextBox_SwitchFocus;
            // 
            // AddNewWordsPage
            // 
            AddNewWordsPage.BackColor = Color.Transparent;
            AddNewWordsPage.Controls.Add(DictionaryAddingLabel);
            AddNewWordsPage.Controls.Add(DictionaryAddingComboBox);
            AddNewWordsPage.Controls.Add(AddDataButton);
            AddNewWordsPage.Controls.Add(EnterHereLabel);
            AddNewWordsPage.Controls.Add(TranslationLabel);
            AddNewWordsPage.Controls.Add(WordToLearnLabel);
            AddNewWordsPage.Controls.Add(InputTranslationTextBox);
            AddNewWordsPage.Controls.Add(InputWordTextBox);
            AddNewWordsPage.ForeColor = Color.Black;
            AddNewWordsPage.Location = new Point(4, 29);
            AddNewWordsPage.Name = "AddNewWordsPage";
            AddNewWordsPage.Padding = new Padding(3);
            AddNewWordsPage.Size = new Size(791, 390);
            AddNewWordsPage.TabIndex = 1;
            AddNewWordsPage.Text = "Add new words";
            // 
            // DictionaryAddingLabel
            // 
            DictionaryAddingLabel.AutoSize = true;
            DictionaryAddingLabel.ForeColor = Color.Black;
            DictionaryAddingLabel.Location = new Point(6, 17);
            DictionaryAddingLabel.Name = "DictionaryAddingLabel";
            DictionaryAddingLabel.Size = new Size(130, 33);
            DictionaryAddingLabel.TabIndex = 10;
            DictionaryAddingLabel.Text = "Dictionary";
            // 
            // DictionaryAddingComboBox
            // 
            DictionaryAddingComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DictionaryAddingComboBox.ForeColor = Color.Black;
            DictionaryAddingComboBox.Location = new Point(143, 16);
            DictionaryAddingComboBox.Name = "DictionaryAddingComboBox";
            DictionaryAddingComboBox.Size = new Size(290, 41);
            DictionaryAddingComboBox.TabIndex = 11;
            // 
            // AddDataButton
            // 
            AddDataButton.BackColor = Color.Transparent;
            AddDataButton.ForeColor = Color.Black;
            AddDataButton.Location = new Point(38, 283);
            AddDataButton.Name = "AddDataButton";
            AddDataButton.Size = new Size(723, 54);
            AddDataButton.TabIndex = 7;
            AddDataButton.Text = "Add the data!";
            AddDataButton.UseVisualStyleBackColor = false;
            AddDataButton.Click += AddNewWord;
            // 
            // EnterHereLabel
            // 
            EnterHereLabel.AutoSize = true;
            EnterHereLabel.Location = new Point(6, 68);
            EnterHereLabel.Name = "EnterHereLabel";
            EnterHereLabel.Size = new Size(179, 33);
            EnterHereLabel.TabIndex = 6;
            EnterHereLabel.Text = "Enter here the";
            // 
            // TranslationLabel
            // 
            TranslationLabel.AutoSize = true;
            TranslationLabel.Location = new Point(52, 165);
            TranslationLabel.Name = "TranslationLabel";
            TranslationLabel.Size = new Size(141, 33);
            TranslationLabel.TabIndex = 4;
            TranslationLabel.Text = "Translation";
            // 
            // WordToLearnLabel
            // 
            WordToLearnLabel.AutoSize = true;
            WordToLearnLabel.Location = new Point(27, 112);
            WordToLearnLabel.Name = "WordToLearnLabel";
            WordToLearnLabel.Size = new Size(171, 33);
            WordToLearnLabel.TabIndex = 3;
            WordToLearnLabel.Text = "Word to learn";
            // 
            // InputTranslationTextBox
            // 
            InputTranslationTextBox.Location = new Point(204, 162);
            InputTranslationTextBox.Name = "InputTranslationTextBox";
            InputTranslationTextBox.Size = new Size(557, 41);
            InputTranslationTextBox.TabIndex = 1;
            InputTranslationTextBox.Tag = "translation";
            InputTranslationTextBox.TextChanged += ValidateTextBox;
            InputTranslationTextBox.KeyDown += TextBox_SwitchFocus;
            // 
            // InputWordTextBox
            // 
            InputWordTextBox.Location = new Point(204, 109);
            InputWordTextBox.Name = "InputWordTextBox";
            InputWordTextBox.Size = new Size(557, 41);
            InputWordTextBox.TabIndex = 0;
            InputWordTextBox.Tag = "word to learn";
            InputWordTextBox.TextChanged += ValidateTextBox;
            InputWordTextBox.KeyDown += TextBox_SwitchFocus;
            // 
            // MyWordsPage
            // 
            MyWordsPage.BackColor = Color.Transparent;
            MyWordsPage.Controls.Add(WordCountLabel);
            MyWordsPage.Controls.Add(DictionariesListBox);
            MyWordsPage.Controls.Add(DictNameLabel);
            MyWordsPage.Controls.Add(DictionaryNameInputTextBox);
            MyWordsPage.Controls.Add(DictLanguageLabel);
            MyWordsPage.Controls.Add(LanguageComboBox);
            MyWordsPage.Controls.Add(AddDictionaryButton);
            MyWordsPage.Controls.Add(UpdateDictionaryButton);
            MyWordsPage.Controls.Add(DeleteDictionaryButton);
            MyWordsPage.ForeColor = Color.Black;
            MyWordsPage.Location = new Point(4, 42);
            MyWordsPage.Name = "MyWordsPage";
            MyWordsPage.Padding = new Padding(3);
            MyWordsPage.Size = new Size(791, 377);
            MyWordsPage.TabIndex = 2;
            MyWordsPage.Text = "My dictionaries";
            MyWordsPage.Enter += MyWordsPage_Enter;
            // 
            // WordCountLabel
            // 
            WordCountLabel.AutoSize = true;
            WordCountLabel.Font = new Font("Comic Sans MS", 9F, FontStyle.Italic);
            WordCountLabel.ForeColor = Color.Gray;
            WordCountLabel.Location = new Point(6, 152);
            WordCountLabel.Name = "WordCountLabel";
            WordCountLabel.Size = new Size(0, 22);
            WordCountLabel.TabIndex = 8;
            // 
            // DictionariesListBox
            // 
            DictionariesListBox.ForeColor = Color.Black;
            DictionariesListBox.ItemHeight = 33;
            DictionariesListBox.Location = new Point(6, 10);
            DictionariesListBox.Name = "DictionariesListBox";
            DictionariesListBox.Size = new Size(266, 136);
            DictionariesListBox.TabIndex = 0;
            DictionariesListBox.SelectedIndexChanged += DictionariesListBox_SelectedIndexChanged;
            // 
            // DictNameLabel
            // 
            DictNameLabel.AutoSize = true;
            DictNameLabel.ForeColor = Color.Black;
            DictNameLabel.Location = new Point(278, 16);
            DictNameLabel.Name = "DictNameLabel";
            DictNameLabel.Size = new Size(77, 33);
            DictNameLabel.TabIndex = 1;
            DictNameLabel.Text = "Name";
            // 
            // DictionaryNameInputTextBox
            // 
            DictionaryNameInputTextBox.Location = new Point(361, 13);
            DictionaryNameInputTextBox.MaxLength = 50;
            DictionaryNameInputTextBox.Name = "DictionaryNameInputTextBox";
            DictionaryNameInputTextBox.Size = new Size(411, 41);
            DictionaryNameInputTextBox.TabIndex = 2;
            // 
            // DictLanguageLabel
            // 
            DictLanguageLabel.AutoSize = true;
            DictLanguageLabel.ForeColor = Color.Black;
            DictLanguageLabel.Location = new Point(278, 64);
            DictLanguageLabel.Name = "DictLanguageLabel";
            DictLanguageLabel.Size = new Size(116, 33);
            DictLanguageLabel.TabIndex = 3;
            DictLanguageLabel.Text = "Language";
            // 
            // LanguageComboBox
            // 
            LanguageComboBox.ForeColor = Color.Black;
            LanguageComboBox.Location = new Point(400, 61);
            LanguageComboBox.Name = "LanguageComboBox";
            LanguageComboBox.Size = new Size(372, 41);
            LanguageComboBox.TabIndex = 4;
            // 
            // AddDictionaryButton
            // 
            AddDictionaryButton.ForeColor = Color.Black;
            AddDictionaryButton.Location = new Point(278, 105);
            AddDictionaryButton.Name = "AddDictionaryButton";
            AddDictionaryButton.Size = new Size(136, 43);
            AddDictionaryButton.TabIndex = 5;
            AddDictionaryButton.Text = "Add";
            AddDictionaryButton.UseVisualStyleBackColor = true;
            AddDictionaryButton.Click += AddDictionary;
            // 
            // UpdateDictionaryButton
            // 
            UpdateDictionaryButton.ForeColor = Color.Black;
            UpdateDictionaryButton.Location = new Point(450, 105);
            UpdateDictionaryButton.Name = "UpdateDictionaryButton";
            UpdateDictionaryButton.Size = new Size(136, 43);
            UpdateDictionaryButton.TabIndex = 6;
            UpdateDictionaryButton.Text = "Save";
            UpdateDictionaryButton.UseVisualStyleBackColor = true;
            UpdateDictionaryButton.Click += UpdateDictionary;
            // 
            // DeleteDictionaryButton
            // 
            DeleteDictionaryButton.ForeColor = Color.Black;
            DeleteDictionaryButton.Location = new Point(648, 105);
            DeleteDictionaryButton.Name = "DeleteDictionaryButton";
            DeleteDictionaryButton.Size = new Size(136, 43);
            DeleteDictionaryButton.TabIndex = 7;
            DeleteDictionaryButton.Text = "Delete";
            DeleteDictionaryButton.UseVisualStyleBackColor = true;
            DeleteDictionaryButton.Click += DeleteDictionary;
            // 
            // UserPage
            // 
            UserPage.BackColor = Color.Transparent;
            UserPage.Controls.Add(textBox1);
            UserPage.Controls.Add(CurrentUserTextBox);
            UserPage.Controls.Add(CurrentUserLabel);
            UserPage.ForeColor = Color.Black;
            UserPage.Location = new Point(4, 42);
            UserPage.Name = "UserPage";
            UserPage.Padding = new Padding(3);
            UserPage.Size = new Size(791, 377);
            UserPage.TabIndex = 3;
            UserPage.Text = "Select user";
            // 
            // textBox1
            // 
            textBox1.Location = new Point(6, 78);
            textBox1.Multiline = true;
            textBox1.Name = "textBox1";
            textBox1.ReadOnly = true;
            textBox1.Size = new Size(437, 100);
            textBox1.TabIndex = 13;
            textBox1.Text = "TO IMPLEMENT:\r\n1. MULTIPLE USERS";
            // 
            // CurrentUserTextBox
            // 
            CurrentUserTextBox.Location = new Point(158, 25);
            CurrentUserTextBox.Name = "CurrentUserTextBox";
            CurrentUserTextBox.ReadOnly = true;
            CurrentUserTextBox.Size = new Size(227, 41);
            CurrentUserTextBox.TabIndex = 12;
            CurrentUserTextBox.Tag = "";
            CurrentUserTextBox.Text = "Toliman";
            // 
            // CurrentUserLabel
            // 
            CurrentUserLabel.AutoSize = true;
            CurrentUserLabel.ForeColor = Color.Black;
            CurrentUserLabel.Location = new Point(6, 28);
            CurrentUserLabel.Name = "CurrentUserLabel";
            CurrentUserLabel.Size = new Size(154, 33);
            CurrentUserLabel.TabIndex = 7;
            CurrentUserLabel.Text = "Current user";
            // 
            // AddWordsErrorProvider
            // 
            AddWordsErrorProvider.ContainerControl = this;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(15F, 33F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(802, 413);
            Controls.Add(MainTabControl);
            Font = new Font("Comic Sans MS", 14.25F);
            ForeColor = Color.White;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Location = new Point(800, 1300);
            Margin = new Padding(5, 6, 5, 6);
            MaximumSize = new Size(820, 460);
            MinimumSize = new Size(820, 460);
            Name = "MainForm";
            Text = "VocabularyTrainer";
            MainTabControl.ResumeLayout(false);
            TrainYourselfPage.ResumeLayout(false);
            TrainYourselfPage.PerformLayout();
            AddNewWordsPage.ResumeLayout(false);
            AddNewWordsPage.PerformLayout();
            MyWordsPage.ResumeLayout(false);
            MyWordsPage.PerformLayout();
            UserPage.ResumeLayout(false);
            UserPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AddWordsErrorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl MainTabControl;
		private TabPage TrainYourselfPage;
		private TabPage AddNewWordsPage;
		private TabPage MyWordsPage;
		private TabPage UserPage;
		// Train yourself
		private Label DictionaryTrainingLabel;
		private ComboBox DictionaryTrainingComboBox;
		private Label DictionaryOfWordLabel;
		private TextBox DisplayWordTextBox;
		private TextBox DisplayTranslationTextBox;
		private Button ShowTranslationButton;
		private Button ShowNextButton;
		private Button DeleteWordButton;
		// Add new words
		private Label DictionaryAddingLabel;
		private ComboBox DictionaryAddingComboBox;
		private Button AddDataButton;
		private Label EnterHereLabel;
		private Label TranslationLabel;
		private Label WordToLearnLabel;
		private TextBox InputTranslationTextBox;
		private TextBox InputWordTextBox;
		// My dictionaries
		private ListBox DictionariesListBox;
		private Label WordCountLabel;
		private Label DictNameLabel;
		private TextBox DictionaryNameInputTextBox;
		private Label DictLanguageLabel;
		private ComboBox LanguageComboBox;
		private Button AddDictionaryButton;
		private Button UpdateDictionaryButton;
		private Button DeleteDictionaryButton;
		// User page
		private Label CurrentUserLabel;
		private TextBox CurrentUserTextBox;
		private TextBox textBox1;
		// Shared
		private ErrorProvider AddWordsErrorProvider;
	}
}
