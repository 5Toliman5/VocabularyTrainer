CREATE TABLE [dbo].[Algorithms]
(
	[ID]   INT NOT NULL PRIMARY KEY IDENTITY,
	[Code] [nvarchar](20) NOT NULL,
	CONSTRAINT [UQ_Algorithms_Code] UNIQUE ([Code])
)
