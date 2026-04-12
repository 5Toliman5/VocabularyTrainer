CREATE TABLE [dbo].[Users]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY,
	[Name] [nvarchar](64) NOT NULL UNIQUE,
	[Info] [nvarchar](max) NULL,
	[Login] [nvarchar](32) NOT NULL,
	[Password] [nvarchar](256) NOT NULL
)
