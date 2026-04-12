CREATE TABLE [dbo].[Dictionaries]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY,
	[UserId] [int] NOT NULL FOREIGN KEY REFERENCES Users(ID),
	[Name] [nvarchar](50) NOT NULL,
	[LanguageCode] [nvarchar](10) NULL,
	CONSTRAINT [UQ_Dictionaries_UserId_Name] UNIQUE ([UserId], [Name])
)
