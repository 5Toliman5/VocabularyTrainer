/* ===========================================================================
   Phase 2 migration script: authentication
   - dbo.Users reshaped into an account: DisplayName, PasswordHash (nullable,
     accounts created via an external provider have no password), CreatedAt,
     IsBlocked. Legacy Name/Login/Password/Info columns are dropped.
   - dbo.UserIdentities: ways to sign in (email, phone, telegram, google, ...)
   - dbo.RefreshTokens: rotating refresh tokens (stored as SHA-256 hashes)
   - dbo.VerificationCodes: one-time codes (stored as SHA-256 hashes)

   Existing users keep their dictionaries and words but have no identity, so
   they cannot sign in until one is attached. See PART C at the bottom.

   Idempotent: re-running after a successful run is a no-op.
   IMPORTANT: take a full backup of the database before running.
   =========================================================================== */

SET XACT_ABORT ON;
SET NOCOUNT  ON;

BEGIN TRANSACTION;

/* ===========================================================================
   PART A. Users -> account
   =========================================================================== */

IF COL_LENGTH('dbo.Users', 'DisplayName') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD DisplayName NVARCHAR(64) NULL;
END;

IF COL_LENGTH('dbo.Users', 'PasswordHash') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD PasswordHash NVARCHAR(256) NULL;
END;

IF COL_LENGTH('dbo.Users', 'CreatedAt') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD CreatedAt DATETIME2 NOT NULL
        CONSTRAINT DF_Users_CreatedAt DEFAULT SYSUTCDATETIME();
END;

IF COL_LENGTH('dbo.Users', 'IsBlocked') IS NULL
BEGIN
    ALTER TABLE dbo.Users ADD IsBlocked BIT NOT NULL
        CONSTRAINT DF_Users_IsBlocked DEFAULT 0;
END;

/* Dynamic SQL: DisplayName may have been added in this same batch, and
   column names of existing tables are bound at compile time. */
IF COL_LENGTH('dbo.Users', 'Name') IS NOT NULL
BEGIN
    EXEC sp_executesql N'UPDATE dbo.Users SET DisplayName = Name WHERE DisplayName IS NULL;';
END;

EXEC sp_executesql N'UPDATE dbo.Users SET DisplayName = CONCAT(N''User '', ID) WHERE DisplayName IS NULL;';

IF EXISTS (
    SELECT 1
      FROM sys.columns
     WHERE object_id   = OBJECT_ID('dbo.Users')
       AND name        = 'DisplayName'
       AND is_nullable = 1)
BEGIN
    ALTER TABLE dbo.Users ALTER COLUMN DisplayName NVARCHAR(64) NOT NULL;
END;

/* The legacy UNIQUE(Name) constraint has a system-generated name. */
DECLARE @nameConstraint SYSNAME = (
    SELECT TOP 1 kc.name
      FROM sys.key_constraints kc
      JOIN sys.index_columns ic ON ic.object_id = kc.parent_object_id AND ic.index_id = kc.unique_index_id
      JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
     WHERE kc.parent_object_id = OBJECT_ID('dbo.Users')
       AND kc.type = 'UQ'
       AND c.name = 'Name');

IF @nameConstraint IS NOT NULL
BEGIN
    DECLARE @dropConstraintSql NVARCHAR(400) = N'ALTER TABLE dbo.Users DROP CONSTRAINT ' + QUOTENAME(@nameConstraint);
    EXEC sp_executesql @dropConstraintSql;
END;

IF COL_LENGTH('dbo.Users', 'Name') IS NOT NULL ALTER TABLE dbo.Users DROP COLUMN Name;
IF COL_LENGTH('dbo.Users', 'Login') IS NOT NULL ALTER TABLE dbo.Users DROP COLUMN Login;
IF COL_LENGTH('dbo.Users', 'Password') IS NOT NULL ALTER TABLE dbo.Users DROP COLUMN Password;
IF COL_LENGTH('dbo.Users', 'Info') IS NOT NULL ALTER TABLE dbo.Users DROP COLUMN Info;

/* ===========================================================================
   PART B. New auth tables
   =========================================================================== */

IF OBJECT_ID('dbo.UserIdentities', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.UserIdentities
    (
        ID          INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        UserId      INT NOT NULL CONSTRAINT FK_UserIdentities_Users REFERENCES dbo.Users(ID) ON DELETE CASCADE,
        Provider    NVARCHAR(20) NOT NULL,
        ProviderKey NVARCHAR(256) NOT NULL,
        CreatedAt   DATETIME2 NOT NULL CONSTRAINT DF_UserIdentities_CreatedAt DEFAULT SYSUTCDATETIME(),
        LastLoginAt DATETIME2 NULL,
        CONSTRAINT UQ_UserIdentities_Provider_ProviderKey UNIQUE (Provider, ProviderKey)
    );

    CREATE INDEX IX_UserIdentities_UserId ON dbo.UserIdentities (UserId);
END;

IF OBJECT_ID('dbo.RefreshTokens', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RefreshTokens
    (
        ID           INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        UserId       INT NOT NULL CONSTRAINT FK_RefreshTokens_Users REFERENCES dbo.Users(ID) ON DELETE CASCADE,
        TokenHash    BINARY(32) NOT NULL CONSTRAINT UQ_RefreshTokens_TokenHash UNIQUE,
        ClientType   NVARCHAR(20) NOT NULL,
        DeviceName   NVARCHAR(128) NULL,
        CreatedAt    DATETIME2 NOT NULL,
        ExpiresAt    DATETIME2 NOT NULL,
        RevokedAt    DATETIME2 NULL,
        ReplacedById INT NULL
    );

    CREATE INDEX IX_RefreshTokens_UserId ON dbo.RefreshTokens (UserId);
END;

IF OBJECT_ID('dbo.VerificationCodes', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.VerificationCodes
    (
        ID           INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Channel      NVARCHAR(20) NOT NULL,
        Target       NVARCHAR(256) NOT NULL,
        Purpose      NVARCHAR(20) NOT NULL,
        CodeHash     BINARY(32) NOT NULL,
        CreatedAt    DATETIME2 NOT NULL,
        ExpiresAt    DATETIME2 NOT NULL,
        AttemptCount INT NOT NULL CONSTRAINT DF_VerificationCodes_AttemptCount DEFAULT 0,
        ConsumedAt   DATETIME2 NULL
    );

    CREATE INDEX IX_VerificationCodes_Target ON dbo.VerificationCodes (Channel, Target, Purpose, CreatedAt);
END;

COMMIT TRANSACTION;

/* ===========================================================================
   PART C (manual, optional). Keep an existing user's words.
   Attach an email to the legacy account, then use "Forgot password" in the
   app to set a password for it:

   INSERT INTO dbo.UserIdentities (UserId, Provider, ProviderKey)
   VALUES (1, N'email', N'you@example.com');   -- email must be lower-case
   =========================================================================== */
