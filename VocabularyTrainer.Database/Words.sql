CREATE TABLE [dbo].[Words]
(
	[ID] INT NOT NULL PRIMARY KEY IDENTITY,
	[Value] [nvarchar](100) NOT NULL,
	[Translation] [nvarchar](100) NOT NULL,
)
GO

CREATE INDEX IX_Words_Value ON [dbo].[Words] ([Value]);
