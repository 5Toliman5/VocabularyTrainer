CREATE TABLE [dbo].[RefreshTokens]
(
	[ID]           INT NOT NULL PRIMARY KEY IDENTITY,
	[UserId]       INT NOT NULL CONSTRAINT [FK_RefreshTokens_Users] FOREIGN KEY REFERENCES Users(ID) ON DELETE CASCADE,
	[TokenHash]    BINARY(32) NOT NULL CONSTRAINT [UQ_RefreshTokens_TokenHash] UNIQUE,
	[ClientType]   NVARCHAR(20) NOT NULL,
	[DeviceName]   NVARCHAR(128) NULL,
	[CreatedAt]    DATETIME2 NOT NULL,
	[ExpiresAt]    DATETIME2 NOT NULL,
	[RevokedAt]    DATETIME2 NULL,
	[ReplacedById] INT NULL
)
GO

CREATE INDEX IX_RefreshTokens_UserId ON [dbo].[RefreshTokens] ([UserId]);
