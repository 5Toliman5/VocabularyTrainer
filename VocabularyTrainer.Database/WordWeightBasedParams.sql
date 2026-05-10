CREATE TABLE [dbo].[WordWeightBasedParams]
(
	[WordId] [int] NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES Words(ID) ON DELETE CASCADE,
	[Weight] [int] NOT NULL CONSTRAINT [DF_WordWeightBasedParams_Weight] DEFAULT 0
)
