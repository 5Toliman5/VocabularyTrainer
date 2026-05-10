CREATE TABLE [dbo].[WordTranslations]
(
	[ID]     INT NOT NULL PRIMARY KEY IDENTITY,
	[WordId] [int] NOT NULL FOREIGN KEY REFERENCES Words(ID) ON DELETE CASCADE,
	[Text]   [nvarchar](500) NOT NULL,
	[Kind]   [nvarchar](20) NOT NULL CONSTRAINT [DF_WordTranslations_Kind] DEFAULT 'Translation'
)
GO

CREATE INDEX IX_WordTranslations_WordId ON [dbo].[WordTranslations] ([WordId]);
