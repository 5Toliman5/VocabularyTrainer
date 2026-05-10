CREATE TABLE [dbo].[WordSm2Params]
(
	[WordId]         [int] NOT NULL PRIMARY KEY FOREIGN KEY REFERENCES Words(ID) ON DELETE CASCADE,
	[Repetitions]    [int] NOT NULL CONSTRAINT [DF_WordSm2Params_Repetitions]    DEFAULT 0,
	[IntervalDays]   [int] NOT NULL CONSTRAINT [DF_WordSm2Params_IntervalDays]   DEFAULT 0,
	[EaseFactor]     DECIMAL(4, 2) NOT NULL CONSTRAINT [DF_WordSm2Params_EaseFactor] DEFAULT 2.50,
	[Lapses]         [int] NOT NULL CONSTRAINT [DF_WordSm2Params_Lapses]         DEFAULT 0,
	[LastReviewedAt] [datetime2] NULL,
	[NextDueAt]      [datetime2] NULL
)
GO

CREATE INDEX IX_WordSm2Params_NextDueAt ON [dbo].[WordSm2Params] ([NextDueAt]);
