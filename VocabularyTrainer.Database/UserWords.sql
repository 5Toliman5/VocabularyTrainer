CREATE TABLE [dbo].[UserWords]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY,
	[UserId] [int] NOT NULL FOREIGN KEY REFERENCES Users(ID),
	[WordId] [int] NOT NULL FOREIGN KEY REFERENCES Words(ID),
	[DictionaryId] [int] NOT NULL FOREIGN KEY REFERENCES Dictionaries(ID),
	[Weight] [int] NOT NULL,
	[DateAdded] [datetime2] NOT NULL DEFAULT GETUTCDATE(),
	[DateModified] [datetime2] NOT NULL DEFAULT GETUTCDATE()
)
GO

CREATE INDEX IX_UserWords_UserId       ON [dbo].[UserWords] ([UserId]);
GO

CREATE INDEX IX_UserWords_DictionaryId ON [dbo].[UserWords] ([DictionaryId]);
GO

CREATE INDEX IX_UserWords_DateAdded    ON [dbo].[UserWords] ([DateAdded]);
