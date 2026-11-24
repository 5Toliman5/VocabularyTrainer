CREATE TABLE [dbo].[UserIdentities]
(
	[ID]          INT NOT NULL PRIMARY KEY IDENTITY,
	[UserId]      INT NOT NULL CONSTRAINT [FK_UserIdentities_Users] FOREIGN KEY REFERENCES Users(ID) ON DELETE CASCADE,
	[Provider]    NVARCHAR(20) NOT NULL,
	[ProviderKey] NVARCHAR(256) NOT NULL,
	[CreatedAt]   DATETIME2 NOT NULL CONSTRAINT [DF_UserIdentities_CreatedAt] DEFAULT SYSUTCDATETIME(),
	[LastLoginAt] DATETIME2 NULL,
	CONSTRAINT [UQ_UserIdentities_Provider_ProviderKey] UNIQUE ([Provider], [ProviderKey])
)
GO

CREATE INDEX IX_UserIdentities_UserId ON [dbo].[UserIdentities] ([UserId]);
