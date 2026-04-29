namespace VocabularyTrainer.WinApp
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
            MyDictionariesPage = new TabPage();
            WordCountLabel = new Label();
            DictionariesListBox = new ListBox();
            DictNameLabel = new Label();
            DictionaryNameInputTextBox = new TextBox();
            DictLanguageLabel = new Label();
            LanguageComboBox = new ComboBox();
            AddDictionaryButton = new Button();
            UpdateDictionaryButton = new Button();
            DeleteDictionaryButton = new Button();
            MyWordsPage = new TabPage();
            MyWordsDictionaryCombo = new ComboBox();
            MyWordsLanguageCombo = new ComboBox();
            MyWordsSearchBox = new TextBox();
            MyWordsApplyButton = new Button();
            MyWordsFromLabel = new Label();
            MyWordsDateFromPicker = new DateTimePicker();
            MyWordsToLabel = new Label();
            MyWordsDateToPicker = new DateTimePicker();
            MyWordsGrid = new DataGridView();
            MyWordsPrevButton = new Button();
            MyWordsPageLabel = new Label();
            MyWordsNextButton = new Button();
            MyWordsDeleteButton = new Button();
            MyWordsResetButton = new Button();
            UserPage = new TabPage();
            textBox1 = new TextBox();
            CurrentUserTextBox = new TextBox();
            CurrentUserLabel = new Label();
            AddWordsErrorProvider = new ErrorProvider(components);
            MainTabControl.SuspendLayout();
            TrainYourselfPage.SuspendLayout();
            AddNewWordsPage.SuspendLayout();
            MyDictionariesPage.SuspendLayout();
            MyWordsPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)MyWordsGrid).BeginInit();
            UserPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)AddWordsErrorProvider).BeginInit();
            SuspendLayout();
            // 
            // MainTabControl
            // 
            MainTabControl.Controls.Add(TrainYourselfPage);
            MainTabControl.Controls.Add(AddNewWordsPage);
            MainTabControl.Controls.Add(MyDictionariesPage);
            MainTabControl.Controls.Add(MyWordsPage);
            MainTabControl.Controls.Add(UserPage);
            MainTabControl.Location = new Point(2, 3);
            MainTabControl.Name = "MainTabControl";
            MainTabControl.SelectedIndex = 0;
            MainTabControl.Size = new Size(920, 532);
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
            TrainYourselfPage.Location = new Point(4, 35);
            TrainYourselfPage.Name = "TrainYourselfPage";
            TrainYourselfPage.Padding = new Padding(3);
            TrainYourselfPage.Size = new Size(912, 493);
            TrainYourselfPage.TabIndex = 0;
            TrainYourselfPage.Text = "Train yourself";
            TrainYourselfPage.Enter += TrainYourSelfPageEnter;
            // 
            // DictionaryOfWordLabel
            // 
            DictionaryOfWordLabel.Font = new Font("Comic Sans MS", 9F, FontStyle.Italic);
            DictionaryOfWordLabel.ForeColor = Color.Gray;
            DictionaryOfWordLabel.Location = new Point(145, 193);
            DictionaryOfWordLabel.Name = "DictionaryOfWordLabel";
            DictionaryOfWordLabel.Size = new Size(609, 20);
            DictionaryOfWordLabel.TabIndex = 12;
            DictionaryOfWordLabel.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DictionaryTrainingLabel
            // 
            DictionaryTrainingLabel.AutoSize = true;
            DictionaryTrainingLabel.ForeColor = Color.Black;
            DictionaryTrainingLabel.Location = new Point(9, 31);
            DictionaryTrainingLabel.Name = "DictionaryTrainingLabel";
            DictionaryTrainingLabel.Size = new Size(101, 26);
            DictionaryTrainingLabel.TabIndex = 10;
            DictionaryTrainingLabel.Text = "Dictionary";
            // 
            // DictionaryTrainingComboBox
            // 
            DictionaryTrainingComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DictionaryTrainingComboBox.ForeColor = Color.Black;
            DictionaryTrainingComboBox.Location = new Point(146, 31);
            DictionaryTrainingComboBox.Name = "DictionaryTrainingComboBox";
            DictionaryTrainingComboBox.Size = new Size(290, 34);
            DictionaryTrainingComboBox.TabIndex = 11;
            DictionaryTrainingComboBox.SelectedIndexChanged += DictionaryTrainingComboBox_SelectedIndexChanged;
            // 
            // DeleteWordButton
            // 
            DeleteWordButton.Location = new Point(358, 357);
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
            DisplayTranslationTextBox.Location = new Point(145, 217);
            DisplayTranslationTextBox.Name = "DisplayTranslationTextBox";
            DisplayTranslationTextBox.ReadOnly = true;
            DisplayTranslationTextBox.Size = new Size(609, 45);
            DisplayTranslationTextBox.TabIndex = 3;
            DisplayTranslationTextBox.TextAlign = HorizontalAlignment.Center;
            DisplayTranslationTextBox.KeyDown += TextBox_SwitchFocus;
            // 
            // ShowNextButton
            // 
            ShowNextButton.Location = new Point(479, 284);
            ShowNextButton.Name = "ShowNextButton";
            ShowNextButton.Size = new Size(275, 42);
            ShowNextButton.TabIndex = 2;
            ShowNextButton.Text = "I remember!";
            ShowNextButton.UseVisualStyleBackColor = true;
            ShowNextButton.Click += ShowNextWord;
            // 
            // ShowTranslationButton
            // 
            ShowTranslationButton.Location = new Point(145, 284);
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
            DisplayWordTextBox.Location = new Point(146, 135);
            DisplayWordTextBox.Name = "DisplayWordTextBox";
            DisplayWordTextBox.ReadOnly = true;
            DisplayWordTextBox.Size = new Size(609, 45);
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
            AddNewWordsPage.Location = new Point(4, 24);
            AddNewWordsPage.Name = "AddNewWordsPage";
            AddNewWordsPage.Padding = new Padding(3);
            AddNewWordsPage.Size = new Size(912, 504);
            AddNewWordsPage.TabIndex = 1;
            AddNewWordsPage.Text = "Add new words";
            // 
            // DictionaryAddingLabel
            // 
            DictionaryAddingLabel.AutoSize = true;
            DictionaryAddingLabel.ForeColor = Color.Black;
            DictionaryAddingLabel.Location = new Point(6, 17);
            DictionaryAddingLabel.Name = "DictionaryAddingLabel";
            DictionaryAddingLabel.Size = new Size(101, 26);
            DictionaryAddingLabel.TabIndex = 10;
            DictionaryAddingLabel.Text = "Dictionary";
            // 
            // DictionaryAddingComboBox
            // 
            DictionaryAddingComboBox.DropDownStyle = ComboBoxStyle.DropDownList;
            DictionaryAddingComboBox.ForeColor = Color.Black;
            DictionaryAddingComboBox.Location = new Point(143, 16);
            DictionaryAddingComboBox.Name = "DictionaryAddingComboBox";
            DictionaryAddingComboBox.Size = new Size(290, 34);
            DictionaryAddingComboBox.TabIndex = 11;
            // 
            // AddDataButton
            // 
            AddDataButton.BackColor = Color.Transparent;
            AddDataButton.ForeColor = Color.Black;
            AddDataButton.Location = new Point(204, 223);
            AddDataButton.Name = "AddDataButton";
            AddDataButton.Size = new Size(557, 54);
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
            EnterHereLabel.Size = new Size(140, 26);
            EnterHereLabel.TabIndex = 6;
            EnterHereLabel.Text = "Enter here the";
            // 
            // TranslationLabel
            // 
            TranslationLabel.AutoSize = true;
            TranslationLabel.Location = new Point(44, 170);
            TranslationLabel.Name = "TranslationLabel";
            TranslationLabel.Size = new Size(110, 26);
            TranslationLabel.TabIndex = 4;
            TranslationLabel.Text = "Translation";
            // 
            // WordToLearnLabel
            // 
            WordToLearnLabel.AutoSize = true;
            WordToLearnLabel.Location = new Point(14, 117);
            WordToLearnLabel.Name = "WordToLearnLabel";
            WordToLearnLabel.Size = new Size(135, 26);
            WordToLearnLabel.TabIndex = 3;
            WordToLearnLabel.Text = "Word to learn";
            // 
            // InputTranslationTextBox
            // 
            InputTranslationTextBox.Location = new Point(204, 162);
            InputTranslationTextBox.Name = "InputTranslationTextBox";
            InputTranslationTextBox.Size = new Size(557, 34);
            InputTranslationTextBox.TabIndex = 1;
            InputTranslationTextBox.Tag = "translation";
            InputTranslationTextBox.TextChanged += ValidateTextBox;
            InputTranslationTextBox.KeyDown += TextBox_SwitchFocus;
            // 
            // InputWordTextBox
            // 
            InputWordTextBox.Location = new Point(204, 109);
            InputWordTextBox.Name = "InputWordTextBox";
            InputWordTextBox.Size = new Size(557, 34);
            InputWordTextBox.TabIndex = 0;
            InputWordTextBox.Tag = "word to learn";
            InputWordTextBox.TextChanged += ValidateTextBox;
            InputWordTextBox.KeyDown += TextBox_SwitchFocus;
            // 
            // MyDictionariesPage
            // 
            MyDictionariesPage.BackColor = Color.Transparent;
            MyDictionariesPage.Controls.Add(WordCountLabel);
            MyDictionariesPage.Controls.Add(DictionariesListBox);
            MyDictionariesPage.Controls.Add(DictNameLabel);
            MyDictionariesPage.Controls.Add(DictionaryNameInputTextBox);
            MyDictionariesPage.Controls.Add(DictLanguageLabel);
            MyDictionariesPage.Controls.Add(LanguageComboBox);
            MyDictionariesPage.Controls.Add(AddDictionaryButton);
            MyDictionariesPage.Controls.Add(UpdateDictionaryButton);
            MyDictionariesPage.Controls.Add(DeleteDictionaryButton);
            MyDictionariesPage.ForeColor = Color.Black;
            MyDictionariesPage.Location = new Point(4, 24);
            MyDictionariesPage.Name = "MyDictionariesPage";
            MyDictionariesPage.Padding = new Padding(3);
            MyDictionariesPage.Size = new Size(912, 504);
            MyDictionariesPage.TabIndex = 2;
            MyDictionariesPage.Text = "My dictionaries";
            MyDictionariesPage.Enter += MyDictionariesPage_Enter;
            // 
            // WordCountLabel
            // 
            WordCountLabel.AutoSize = true;
            WordCountLabel.Font = new Font("Comic Sans MS", 9F, FontStyle.Italic);
            WordCountLabel.ForeColor = Color.Gray;
            WordCountLabel.Location = new Point(6, 152);
            WordCountLabel.Name = "WordCountLabel";
            WordCountLabel.Size = new Size(0, 17);
            WordCountLabel.TabIndex = 8;
            // 
            // DictionariesListBox
            // 
            DictionariesListBox.ForeColor = Color.Black;
            DictionariesListBox.Location = new Point(6, 10);
            DictionariesListBox.Name = "DictionariesListBox";
            DictionariesListBox.Size = new Size(266, 134);
            DictionariesListBox.TabIndex = 0;
            DictionariesListBox.SelectedIndexChanged += DictionariesListBox_SelectedIndexChanged;
            // 
            // DictNameLabel
            // 
            DictNameLabel.AutoSize = true;
            DictNameLabel.ForeColor = Color.Black;
            DictNameLabel.Location = new Point(278, 16);
            DictNameLabel.Name = "DictNameLabel";
            DictNameLabel.Size = new Size(62, 26);
            DictNameLabel.TabIndex = 1;
            DictNameLabel.Text = "Name";
            // 
            // DictionaryNameInputTextBox
            // 
            DictionaryNameInputTextBox.Location = new Point(361, 13);
            DictionaryNameInputTextBox.MaxLength = 50;
            DictionaryNameInputTextBox.Name = "DictionaryNameInputTextBox";
            DictionaryNameInputTextBox.Size = new Size(411, 34);
            DictionaryNameInputTextBox.TabIndex = 2;
            // 
            // DictLanguageLabel
            // 
            DictLanguageLabel.AutoSize = true;
            DictLanguageLabel.ForeColor = Color.Black;
            DictLanguageLabel.Location = new Point(278, 64);
            DictLanguageLabel.Name = "DictLanguageLabel";
            DictLanguageLabel.Size = new Size(92, 26);
            DictLanguageLabel.TabIndex = 3;
            DictLanguageLabel.Text = "Language";
            // 
            // LanguageComboBox
            // 
            LanguageComboBox.ForeColor = Color.Black;
            LanguageComboBox.Location = new Point(400, 61);
            LanguageComboBox.Name = "LanguageComboBox";
            LanguageComboBox.Size = new Size(372, 34);
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
            // MyWordsPage
            // 
            MyWordsPage.BackColor = Color.Transparent;
            MyWordsPage.Controls.Add(MyWordsDictionaryCombo);
            MyWordsPage.Controls.Add(MyWordsLanguageCombo);
            MyWordsPage.Controls.Add(MyWordsSearchBox);
            MyWordsPage.Controls.Add(MyWordsApplyButton);
            MyWordsPage.Controls.Add(MyWordsFromLabel);
            MyWordsPage.Controls.Add(MyWordsDateFromPicker);
            MyWordsPage.Controls.Add(MyWordsToLabel);
            MyWordsPage.Controls.Add(MyWordsDateToPicker);
            MyWordsPage.Controls.Add(MyWordsGrid);
            MyWordsPage.Controls.Add(MyWordsPrevButton);
            MyWordsPage.Controls.Add(MyWordsPageLabel);
            MyWordsPage.Controls.Add(MyWordsNextButton);
            MyWordsPage.Controls.Add(MyWordsDeleteButton);
            MyWordsPage.Controls.Add(MyWordsResetButton);
            MyWordsPage.ForeColor = Color.Black;
            MyWordsPage.Location = new Point(4, 35);
            MyWordsPage.Name = "MyWordsPage";
            MyWordsPage.Padding = new Padding(3);
            MyWordsPage.Size = new Size(912, 493);
            MyWordsPage.TabIndex = 3;
            MyWordsPage.Text = "My words";
            MyWordsPage.Enter += MyWordsPage_Enter;
            // 
            // MyWordsDictionaryCombo
            // 
            MyWordsDictionaryCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            MyWordsDictionaryCombo.ForeColor = Color.Black;
            MyWordsDictionaryCombo.Location = new Point(8, 8);
            MyWordsDictionaryCombo.Name = "MyWordsDictionaryCombo";
            MyWordsDictionaryCombo.Size = new Size(290, 34);
            MyWordsDictionaryCombo.TabIndex = 0;
            // 
            // MyWordsLanguageCombo
            // 
            MyWordsLanguageCombo.DropDownStyle = ComboBoxStyle.DropDownList;
            MyWordsLanguageCombo.ForeColor = Color.Black;
            MyWordsLanguageCombo.Location = new Point(304, 8);
            MyWordsLanguageCombo.Name = "MyWordsLanguageCombo";
            MyWordsLanguageCombo.Size = new Size(115, 34);
            MyWordsLanguageCombo.TabIndex = 1;
            // 
            // MyWordsSearchBox
            // 
            MyWordsSearchBox.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MyWordsSearchBox.ForeColor = Color.Black;
            MyWordsSearchBox.Location = new Point(509, 9);
            MyWordsSearchBox.Name = "MyWordsSearchBox";
            MyWordsSearchBox.PlaceholderText = "Search...";
            MyWordsSearchBox.Size = new Size(284, 30);
            MyWordsSearchBox.TabIndex = 2;
            // 
            // MyWordsApplyButton
            // 
            MyWordsApplyButton.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MyWordsApplyButton.ForeColor = Color.Black;
            MyWordsApplyButton.Location = new Point(799, 6);
            MyWordsApplyButton.Name = "MyWordsApplyButton";
            MyWordsApplyButton.Size = new Size(105, 41);
            MyWordsApplyButton.TabIndex = 3;
            MyWordsApplyButton.Text = "Search";
            MyWordsApplyButton.UseVisualStyleBackColor = true;
            MyWordsApplyButton.Click += MyWordsApplyButton_Click;
            // 
            // MyWordsFromLabel
            // 
            MyWordsFromLabel.AutoSize = true;
            MyWordsFromLabel.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MyWordsFromLabel.ForeColor = Color.Black;
            MyWordsFromLabel.Location = new Point(8, 62);
            MyWordsFromLabel.Name = "MyWordsFromLabel";
            MyWordsFromLabel.Size = new Size(47, 23);
            MyWordsFromLabel.TabIndex = 4;
            MyWordsFromLabel.Text = "From";
            // 
            // MyWordsDateFromPicker
            // 
            MyWordsDateFromPicker.Checked = false;
            MyWordsDateFromPicker.Font = new Font("Comic Sans MS", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MyWordsDateFromPicker.Location = new Point(87, 65);
            MyWordsDateFromPicker.Name = "MyWordsDateFromPicker";
            MyWordsDateFromPicker.ShowCheckBox = true;
            MyWordsDateFromPicker.Size = new Size(155, 26);
            MyWordsDateFromPicker.TabIndex = 5;
            // 
            // MyWordsToLabel
            // 
            MyWordsToLabel.AutoSize = true;
            MyWordsToLabel.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MyWordsToLabel.ForeColor = Color.Black;
            MyWordsToLabel.Location = new Point(261, 62);
            MyWordsToLabel.Name = "MyWordsToLabel";
            MyWordsToLabel.Size = new Size(29, 23);
            MyWordsToLabel.TabIndex = 6;
            MyWordsToLabel.Text = "To";
            // 
            // MyWordsDateToPicker
            // 
            MyWordsDateToPicker.Checked = false;
            MyWordsDateToPicker.Font = new Font("Comic Sans MS", 10.2F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MyWordsDateToPicker.Location = new Point(309, 62);
            MyWordsDateToPicker.Name = "MyWordsDateToPicker";
            MyWordsDateToPicker.ShowCheckBox = true;
            MyWordsDateToPicker.Size = new Size(155, 26);
            MyWordsDateToPicker.TabIndex = 7;
            // 
            // MyWordsGrid
            // 
            MyWordsGrid.AllowUserToAddRows = false;
            MyWordsGrid.AllowUserToDeleteRows = false;
            MyWordsGrid.AllowUserToOrderColumns = true;
            MyWordsGrid.AllowUserToResizeRows = false;
            MyWordsGrid.BackgroundColor = SystemColors.Menu;
            MyWordsGrid.ColumnHeadersHeight = 29;
            MyWordsGrid.Location = new Point(8, 106);
            MyWordsGrid.Name = "MyWordsGrid";
            MyWordsGrid.RowHeadersWidth = 51;
            MyWordsGrid.Size = new Size(896, 291);
            MyWordsGrid.TabIndex = 8;
            // 
            // MyWordsPrevButton
            // 
            MyWordsPrevButton.Enabled = false;
            MyWordsPrevButton.ForeColor = Color.Black;
            MyWordsPrevButton.Location = new Point(8, 425);
            MyWordsPrevButton.Name = "MyWordsPrevButton";
            MyWordsPrevButton.Size = new Size(70, 53);
            MyWordsPrevButton.TabIndex = 9;
            MyWordsPrevButton.Text = "< Prev";
            MyWordsPrevButton.UseVisualStyleBackColor = true;
            MyWordsPrevButton.Click += MyWordsPrevButton_Click;
            // 
            // MyWordsPageLabel
            // 
            MyWordsPageLabel.AutoSize = true;
            MyWordsPageLabel.ForeColor = Color.Black;
            MyWordsPageLabel.Location = new Point(87, 435);
            MyWordsPageLabel.Name = "MyWordsPageLabel";
            MyWordsPageLabel.Size = new Size(108, 26);
            MyWordsPageLabel.TabIndex = 10;
            MyWordsPageLabel.Text = "Page 1 of 1";
            // 
            // MyWordsNextButton
            // 
            MyWordsNextButton.Enabled = false;
            MyWordsNextButton.ForeColor = Color.Black;
            MyWordsNextButton.Location = new Point(394, 422);
            MyWordsNextButton.Name = "MyWordsNextButton";
            MyWordsNextButton.Size = new Size(70, 58);
            MyWordsNextButton.TabIndex = 11;
            MyWordsNextButton.Text = ">";
            MyWordsNextButton.UseVisualStyleBackColor = true;
            MyWordsNextButton.Click += MyWordsNextButton_Click;
            // 
            // MyWordsDeleteButton
            // 
            MyWordsDeleteButton.ForeColor = Color.Black;
            MyWordsDeleteButton.Location = new Point(701, 418);
            MyWordsDeleteButton.Name = "MyWordsDeleteButton";
            MyWordsDeleteButton.Size = new Size(203, 58);
            MyWordsDeleteButton.TabIndex = 12;
            MyWordsDeleteButton.Text = "Delete selected";
            MyWordsDeleteButton.UseVisualStyleBackColor = true;
            MyWordsDeleteButton.Click += MyWordsDeleteButton_Click;
            // 
            // MyWordsResetButton
            // 
            MyWordsResetButton.Font = new Font("Comic Sans MS", 12F, FontStyle.Regular, GraphicsUnit.Point, 0);
            MyWordsResetButton.ForeColor = Color.Black;
            MyWordsResetButton.Location = new Point(479, 59);
            MyWordsResetButton.Name = "MyWordsResetButton";
            MyWordsResetButton.Size = new Size(120, 35);
            MyWordsResetButton.TabIndex = 13;
            MyWordsResetButton.Text = "Reset";
            MyWordsResetButton.UseVisualStyleBackColor = true;
            MyWordsResetButton.Click += MyWordsResetButton_Click;
            // 
            // UserPage
            // 
            UserPage.BackColor = Color.Transparent;
            UserPage.Controls.Add(textBox1);
            UserPage.Controls.Add(CurrentUserTextBox);
            UserPage.Controls.Add(CurrentUserLabel);
            UserPage.ForeColor = Color.Black;
            UserPage.Location = new Point(4, 24);
            UserPage.Name = "UserPage";
            UserPage.Padding = new Padding(3);
            UserPage.Size = new Size(912, 504);
            UserPage.TabIndex = 4;
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
            CurrentUserTextBox.Size = new Size(227, 34);
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
            CurrentUserLabel.Size = new Size(123, 26);
            CurrentUserLabel.TabIndex = 7;
            CurrentUserLabel.Text = "Current user";
            // 
            // AddWordsErrorProvider
            // 
            AddWordsErrorProvider.ContainerControl = this;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(12F, 26F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(924, 541);
            Controls.Add(MainTabControl);
            Font = new Font("Comic Sans MS", 14.25F);
            ForeColor = Color.White;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Location = new Point(800, 1300);
            Margin = new Padding(5, 6, 5, 6);
            MaximumSize = new Size(940, 580);
            MinimumSize = new Size(940, 580);
            Name = "MainForm";
            Text = "VocabularyTrainer";
            MainTabControl.ResumeLayout(false);
            TrainYourselfPage.ResumeLayout(false);
            TrainYourselfPage.PerformLayout();
            AddNewWordsPage.ResumeLayout(false);
            AddNewWordsPage.PerformLayout();
            MyDictionariesPage.ResumeLayout(false);
            MyDictionariesPage.PerformLayout();
            MyWordsPage.ResumeLayout(false);
            MyWordsPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)MyWordsGrid).EndInit();
            UserPage.ResumeLayout(false);
            UserPage.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)AddWordsErrorProvider).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private TabControl MainTabControl;
		private TabPage TrainYourselfPage;
		private TabPage AddNewWordsPage;
		private TabPage MyDictionariesPage;
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
		// My words browser
		private ComboBox MyWordsDictionaryCombo;
		private ComboBox MyWordsLanguageCombo;
		private TextBox MyWordsSearchBox;
		private Button MyWordsApplyButton;
		private Label MyWordsFromLabel;
		private DateTimePicker MyWordsDateFromPicker;
		private Label MyWordsToLabel;
		private DateTimePicker MyWordsDateToPicker;
		private DataGridView MyWordsGrid;
		private Button MyWordsPrevButton;
		private Label MyWordsPageLabel;
		private Button MyWordsNextButton;
		private Button MyWordsDeleteButton;
		private Button MyWordsResetButton;
		// User page
		private Label CurrentUserLabel;
		private TextBox CurrentUserTextBox;
		private TextBox textBox1;
		// Shared
		private ErrorProvider AddWordsErrorProvider;
	}
}
