/* ===========================================================================
   Phase 1 migration script (UNIFIED)
   - dbo.Algorithms lookup table (with seed)
   - dbo.Dictionaries.AlgorithmId FK -> Algorithms.ID (replaces any legacy
     denormalised AlgorithmCode string column)
   - Per-user dbo.Words (UserId, DictionaryId, NormalizedText, LanguageCode,
     Notes, DateAdded, DateModified) - replaces old (Value, Translation) Words
     + UserWords junction
   - dbo.WordTranslations (1:N from Words)
   - Pluggable training algorithms via satellite tables:
       * dbo.WordWeightBasedParams (carries old UserWords.Weight)
       * dbo.WordSm2Params         (empty, lazy-created on first review)
   - Drops dbo.UserWords

   Idempotent: re-running after a successful run is a no-op.
   Run in a single connection. Wraps everything in one transaction.

   IMPORTANT: take a full backup of the database before running.
   =========================================================================== */

SET XACT_ABORT ON;
SET NOCOUNT  ON;

BEGIN TRANSACTION;

/* ===========================================================================
   PART A. Algorithms lookup + Dictionaries.AlgorithmId FK
   =========================================================================== */

/* ---------- A1. Algorithms table ----------------------------------------- */
IF OBJECT_ID('dbo.Algorithms', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Algorithms
    (
        ID   INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
        Code NVARCHAR(20) NOT NULL,
        CONSTRAINT UQ_Algorithms_Code UNIQUE (Code)
    );
END;

/* The application also auto-seeds these on API startup (AlgorithmsSeeder).
   Seeding here too keeps a fresh DB usable before the API runs. */
IF NOT EXISTS (SELECT 1 FROM dbo.Algorithms WHERE Code = N'WeightBased')
    INSERT INTO dbo.Algorithms (Code) VALUES (N'WeightBased');

IF NOT EXISTS (SELECT 1 FROM dbo.Algorithms WHERE Code = N'Sm2')
    INSERT INTO dbo.Algorithms (Code) VALUES (N'Sm2');

/* ---------- A2. Dictionaries.AlgorithmId column (nullable, temp) --------- */
IF COL_LENGTH('dbo.Dictionaries', 'AlgorithmId') IS NULL
BEGIN
    ALTER TABLE dbo.Dictionaries ADD AlgorithmId INT NULL;
END;

/* ---------- A3. Backfill AlgorithmId ------------------------------------- */
/* If a legacy revision of this script created Dictionaries.AlgorithmCode,
   use it as the source. Otherwise default everything to 'WeightBased'.

   Wrapped in EXEC sp_executesql so the SQL Server parser does not bind
   the AlgorithmId column at compile time — necessary because the column
   may have just been added by A2 in this same batch and isn't visible to
   the parser yet (deferred name resolution applies to TABLES but not to
   COLUMNS of existing tables). The dynamic SQL is parsed at execution
   time, after A2 has run, so AlgorithmId resolves cleanly. */
IF COL_LENGTH('dbo.Dictionaries', 'AlgorithmId') IS NOT NULL
BEGIN
    DECLARE @WeightBasedId INT = (SELECT ID FROM dbo.Algorithms WHERE Code = N'WeightBased');

    IF COL_LENGTH('dbo.Dictionaries', 'AlgorithmCode') IS NOT NULL
    BEGIN
        EXEC sp_executesql
            N'UPDATE d
                 SET d.AlgorithmId = ISNULL(a.ID, @WBId)
                FROM dbo.Dictionaries d
                LEFT JOIN dbo.Algorithms  a ON a.Code = d.AlgorithmCode
               WHERE d.AlgorithmId IS NULL;',
            N'@WBId INT',
            @WBId = @WeightBasedId;
    END
    ELSE
    BEGIN
        EXEC sp_executesql
            N'UPDATE dbo.Dictionaries SET AlgorithmId = @WBId WHERE AlgorithmId IS NULL;',
            N'@WBId INT',
            @WBId = @WeightBasedId;
    END;
END;

/* ---------- A4. AlgorithmId NOT NULL + FK + index ------------------------ */
IF EXISTS (
    SELECT 1
      FROM sys.columns
     WHERE object_id   = OBJECT_ID('dbo.Dictionaries')
       AND name        = 'AlgorithmId'
       AND is_nullable = 1)
BEGIN
    ALTER TABLE dbo.Dictionaries ALTER COLUMN AlgorithmId INT NOT NULL;
END;

IF NOT EXISTS (
    SELECT 1
      FROM sys.foreign_keys
     WHERE name              = 'FK_Dictionaries_Algorithms'
       AND parent_object_id  = OBJECT_ID('dbo.Dictionaries'))
BEGIN
    ALTER TABLE dbo.Dictionaries
        ADD CONSTRAINT FK_Dictionaries_Algorithms
            FOREIGN KEY (AlgorithmId) REFERENCES dbo.Algorithms(ID);
END;

IF NOT EXISTS (
    SELECT 1
      FROM sys.indexes
     WHERE name      = 'IX_Dictionaries_AlgorithmId'
       AND object_id = OBJECT_ID('dbo.Dictionaries'))
BEGIN
    CREATE INDEX IX_Dictionaries_AlgorithmId ON dbo.Dictionaries (AlgorithmId);
END;

/* ---------- A5. Drop legacy AlgorithmCode column (if present) ------------ */
IF EXISTS (
    SELECT 1
      FROM sys.default_constraints
     WHERE name              = 'DF_Dictionaries_AlgorithmCode'
       AND parent_object_id  = OBJECT_ID('dbo.Dictionaries'))
BEGIN
    ALTER TABLE dbo.Dictionaries DROP CONSTRAINT DF_Dictionaries_AlgorithmCode;
END;

IF COL_LENGTH('dbo.Dictionaries', 'AlgorithmCode') IS NOT NULL
BEGIN
    ALTER TABLE dbo.Dictionaries DROP COLUMN AlgorithmCode;
END;

/* ===========================================================================
   PART B. Words refactor (per-user Words, satellites, drop UserWords)

   The whole block is gated on UserWords still existing, so re-runs after
   a successful migration (or runs on a freshly sqlproj-deployed DB) are
   true no-ops.
   =========================================================================== */

IF OBJECT_ID('dbo.UserWords', 'U') IS NOT NULL
BEGIN

    /* ------ B1. New Words table created side-by-side --------------------- */
    IF OBJECT_ID('dbo.Words_New', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.Words_New
        (
            ID             INT             NOT NULL IDENTITY(1,1) PRIMARY KEY,
            UserId         INT             NOT NULL,
            DictionaryId   INT             NOT NULL,
            Value          NVARCHAR(200)   NOT NULL,
            NormalizedText NVARCHAR(200)   NOT NULL,
            LanguageCode   NVARCHAR(10)    NOT NULL,
            Notes          NVARCHAR(MAX)   NULL,
            DateAdded      DATETIME2       NOT NULL CONSTRAINT DF_WordsNew_DateAdded    DEFAULT GETUTCDATE(),
            DateModified   DATETIME2       NOT NULL CONSTRAINT DF_WordsNew_DateModified DEFAULT GETUTCDATE(),
            CONSTRAINT FK_WordsNew_Users        FOREIGN KEY (UserId)       REFERENCES dbo.Users(ID),
            CONSTRAINT FK_WordsNew_Dictionaries FOREIGN KEY (DictionaryId) REFERENCES dbo.Dictionaries(ID)
        );
    END;

    /* ------ B2. Backfill new Words from UserWords + old Words ------------ */
    IF NOT EXISTS (SELECT 1 FROM dbo.Words_New)
    BEGIN
        ;WITH ranked AS
        (
            SELECT
                uw.UserId,
                uw.DictionaryId,
                w.Value,
                LOWER(LTRIM(RTRIM(w.Value)))     AS NormalizedText,
                ISNULL(d.LanguageCode, 'unknown') AS LanguageCode,
                uw.DateAdded,
                uw.DateModified,
                w.Translation,
                uw.Weight,
                ROW_NUMBER() OVER (
                    PARTITION BY uw.UserId, uw.DictionaryId, LOWER(LTRIM(RTRIM(w.Value)))
                    ORDER BY uw.DateAdded
                ) AS rn
            FROM dbo.UserWords    uw
            JOIN dbo.Words        w ON w.ID = uw.WordId
            JOIN dbo.Dictionaries d ON d.ID = uw.DictionaryId
        )
        INSERT INTO dbo.Words_New
            (UserId, DictionaryId, Value, NormalizedText, LanguageCode,
             Notes, DateAdded, DateModified)
        SELECT
            UserId, DictionaryId, Value, NormalizedText, LanguageCode,
            NULL, DateAdded, DateModified
        FROM ranked
        WHERE rn = 1;
    END;

    /* ------ B3. WordTranslations table ---------------------------------- */
    IF OBJECT_ID('dbo.WordTranslations', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.WordTranslations
        (
            ID     INT NOT NULL IDENTITY(1,1) PRIMARY KEY,
            WordId INT NOT NULL,
            Text   NVARCHAR(500) NOT NULL,
            Kind   NVARCHAR(20)  NOT NULL CONSTRAINT DF_WordTranslations_Kind DEFAULT 'Translation',
            CONSTRAINT FK_WordTranslations_WordsNew FOREIGN KEY (WordId)
                REFERENCES dbo.Words_New(ID) ON DELETE CASCADE
        );

        CREATE INDEX IX_WordTranslations_WordId ON dbo.WordTranslations (WordId);
    END;

    /* ------ B4. Backfill translations from old Words.Translation -------- */
    IF NOT EXISTS (SELECT 1 FROM dbo.WordTranslations)
    BEGIN
        INSERT INTO dbo.WordTranslations (WordId, Text, Kind)
        SELECT DISTINCT
            wn.ID,
            oldW.Translation,
            'Translation'
        FROM dbo.Words_New wn
        JOIN dbo.UserWords uw   ON uw.UserId = wn.UserId AND uw.DictionaryId = wn.DictionaryId
        JOIN dbo.Words     oldW ON oldW.ID  = uw.WordId
                                AND LOWER(LTRIM(RTRIM(oldW.Value))) = wn.NormalizedText
        WHERE oldW.Translation IS NOT NULL AND LEN(oldW.Translation) > 0;
    END;

    /* ------ B5. Algorithm satellites ------------------------------------ */
    IF OBJECT_ID('dbo.WordWeightBasedParams', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.WordWeightBasedParams
        (
            WordId INT NOT NULL PRIMARY KEY,
            Weight INT NOT NULL CONSTRAINT DF_WordWeightBasedParams_Weight DEFAULT 0,
            CONSTRAINT FK_WordWeightBasedParams_WordsNew FOREIGN KEY (WordId)
                REFERENCES dbo.Words_New(ID) ON DELETE CASCADE
        );
    END;

    IF OBJECT_ID('dbo.WordSm2Params', 'U') IS NULL
    BEGIN
        CREATE TABLE dbo.WordSm2Params
        (
            WordId         INT NOT NULL PRIMARY KEY,
            Repetitions    INT NOT NULL CONSTRAINT DF_WordSm2Params_Repetitions    DEFAULT 0,
            IntervalDays   INT NOT NULL CONSTRAINT DF_WordSm2Params_IntervalDays   DEFAULT 0,
            EaseFactor     DECIMAL(4,2) NOT NULL CONSTRAINT DF_WordSm2Params_EaseFactor DEFAULT 2.50,
            Lapses         INT NOT NULL CONSTRAINT DF_WordSm2Params_Lapses         DEFAULT 0,
            LastReviewedAt DATETIME2 NULL,
            NextDueAt      DATETIME2 NULL,
            CONSTRAINT FK_WordSm2Params_WordsNew FOREIGN KEY (WordId)
                REFERENCES dbo.Words_New(ID) ON DELETE CASCADE
        );

        CREATE INDEX IX_WordSm2Params_NextDueAt ON dbo.WordSm2Params (NextDueAt);
    END;

    /* ------ B6. Backfill weight-based params from UserWords.Weight ------ */
    IF NOT EXISTS (SELECT 1 FROM dbo.WordWeightBasedParams)
    BEGIN
        INSERT INTO dbo.WordWeightBasedParams (WordId, Weight)
        SELECT
            wn.ID,
            MAX(uw.Weight)
        FROM dbo.Words_New wn
        JOIN dbo.UserWords uw ON uw.UserId = wn.UserId AND uw.DictionaryId = wn.DictionaryId
        JOIN dbo.Words     ow ON ow.ID = uw.WordId
                              AND LOWER(LTRIM(RTRIM(ow.Value))) = wn.NormalizedText
        GROUP BY wn.ID;
    END;

    /* ------ B7. Drop UserWords + old Words, rename Words_New ------------ */
    DROP TABLE dbo.UserWords;
    DROP TABLE dbo.Words;

    EXEC sp_rename 'dbo.Words_New', 'Words';

    /* The renamed table's constraints keep their old names with "_New"
       suffix. Rename them so future SSDT compares stay clean. */
    IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_WordsNew_DateAdded')
        EXEC sp_rename 'DF_WordsNew_DateAdded',    'DF_Words_DateAdded',    'OBJECT';
    IF EXISTS (SELECT 1 FROM sys.default_constraints WHERE name = 'DF_WordsNew_DateModified')
        EXEC sp_rename 'DF_WordsNew_DateModified', 'DF_Words_DateModified', 'OBJECT';
    IF EXISTS (SELECT 1 FROM sys.foreign_keys      WHERE name = 'FK_WordsNew_Users')
        EXEC sp_rename 'FK_WordsNew_Users',        'FK_Words_Users',        'OBJECT';
    IF EXISTS (SELECT 1 FROM sys.foreign_keys      WHERE name = 'FK_WordsNew_Dictionaries')
        EXEC sp_rename 'FK_WordsNew_Dictionaries', 'FK_Words_Dictionaries', 'OBJECT';
    IF EXISTS (SELECT 1 FROM sys.foreign_keys      WHERE name = 'FK_WordTranslations_WordsNew')
        EXEC sp_rename 'FK_WordTranslations_WordsNew', 'FK_WordTranslations_Words', 'OBJECT';
    IF EXISTS (SELECT 1 FROM sys.foreign_keys      WHERE name = 'FK_WordWeightBasedParams_WordsNew')
        EXEC sp_rename 'FK_WordWeightBasedParams_WordsNew', 'FK_WordWeightBasedParams_Words', 'OBJECT';
    IF EXISTS (SELECT 1 FROM sys.foreign_keys      WHERE name = 'FK_WordSm2Params_WordsNew')
        EXEC sp_rename 'FK_WordSm2Params_WordsNew', 'FK_WordSm2Params_Words', 'OBJECT';

END;  /* end of Words refactor block */

/* ===========================================================================
   PART C. Indexes / unique constraint on the new Words table.
   Always-evaluated, idempotent. Safe on both freshly-migrated and
   sqlproj-deployed DBs.
   =========================================================================== */

IF OBJECT_ID('dbo.Words', 'U') IS NOT NULL
   AND COL_LENGTH('dbo.Words', 'UserId') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'UQ_Words_User_Dict_Normalized' AND object_id = OBJECT_ID('dbo.Words'))
        ALTER TABLE dbo.Words
            ADD CONSTRAINT UQ_Words_User_Dict_Normalized UNIQUE (UserId, DictionaryId, NormalizedText);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Words_UserId'         AND object_id = OBJECT_ID('dbo.Words'))
        CREATE INDEX IX_Words_UserId         ON dbo.Words (UserId);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Words_DictionaryId'   AND object_id = OBJECT_ID('dbo.Words'))
        CREATE INDEX IX_Words_DictionaryId   ON dbo.Words (DictionaryId);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Words_DateAdded'      AND object_id = OBJECT_ID('dbo.Words'))
        CREATE INDEX IX_Words_DateAdded      ON dbo.Words (DateAdded);

    IF NOT EXISTS (SELECT 1 FROM sys.indexes WHERE name = 'IX_Words_NormalizedText' AND object_id = OBJECT_ID('dbo.Words'))
        CREATE INDEX IX_Words_NormalizedText ON dbo.Words (NormalizedText);
END;

COMMIT TRANSACTION;
GO

PRINT 'Phase 1 migration completed successfully.';
