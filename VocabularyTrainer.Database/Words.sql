CREATE TABLE [dbo].[Words]
(
	[ID]             INT NOT NULL PRIMARY KEY IDENTITY,
	[UserId]         [int] NOT NULL FOREIGN KEY REFERENCES Users(ID),
	[DictionaryId]   [int] NOT NULL FOREIGN KEY REFERENCES Dictionaries(ID),
	[Value]          [nvarchar](200) NOT NULL,
	[NormalizedText] [nvarchar](200) NOT NULL,
	[LanguageCode]   [nvarchar](10) NOT NULL,
	[DateAdded]      [datetime2] NOT NULL CONSTRAINT [DF_Words_DateAdded]    DEFAULT GETUTCDATE(),
	[DateModified]   [datetime2] NOT NULL CONSTRAINT [DF_Words_DateModified] DEFAULT GETUTCDATE(),
	CONSTRAINT [UQ_Words_User_Dict_Normalized] UNIQUE ([UserId], [DictionaryId], [NormalizedText])
)
GO

CREATE INDEX IX_Words_UserId           ON [dbo].[Words] ([UserId]);
GO

CREATE INDEX IX_Words_DictionaryId     ON [dbo].[Words] ([DictionaryId]);
GO

CREATE INDEX IX_Words_DateAdded        ON [dbo].[Words] ([DateAdded]);
GO

CREATE INDEX IX_Words_NormalizedText   ON [dbo].[Words] ([NormalizedText]);
