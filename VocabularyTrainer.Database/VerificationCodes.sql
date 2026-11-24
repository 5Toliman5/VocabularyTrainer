CREATE TABLE [dbo].[VerificationCodes]
(
	[ID]           INT NOT NULL PRIMARY KEY IDENTITY,
	[Channel]      NVARCHAR(20) NOT NULL,
	[Target]       NVARCHAR(256) NOT NULL,
	[Purpose]      NVARCHAR(20) NOT NULL,
	[CodeHash]     BINARY(32) NOT NULL,
	[CreatedAt]    DATETIME2 NOT NULL,
	[ExpiresAt]    DATETIME2 NOT NULL,
	[AttemptCount] INT NOT NULL CONSTRAINT [DF_VerificationCodes_AttemptCount] DEFAULT 0,
	[ConsumedAt]   DATETIME2 NULL
)
GO

CREATE INDEX IX_VerificationCodes_Target ON [dbo].[VerificationCodes] ([Channel], [Target], [Purpose], [CreatedAt]);
