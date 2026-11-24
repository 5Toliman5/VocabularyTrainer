/* ===========================================================================
   Deleting a dictionary deletes its words.
   Words.DictionaryId -> Dictionaries.ID becomes ON DELETE CASCADE; translations
   and algorithm params already cascade from Words, so the whole subtree goes.

   Idempotent: re-running after a successful run is a no-op.
   =========================================================================== */

SET XACT_ABORT ON;
SET NOCOUNT  ON;

BEGIN TRANSACTION;

DECLARE @fkName SYSNAME;
DECLARE @deleteAction NVARCHAR(60);

SELECT TOP 1 @fkName = fk.name, @deleteAction = fk.delete_referential_action_desc
  FROM sys.foreign_keys fk
  JOIN sys.foreign_key_columns fkc ON fkc.constraint_object_id = fk.object_id
  JOIN sys.columns c ON c.object_id = fkc.parent_object_id AND c.column_id = fkc.parent_column_id
 WHERE fk.parent_object_id = OBJECT_ID('dbo.Words')
   AND fk.referenced_object_id = OBJECT_ID('dbo.Dictionaries')
   AND c.name = 'DictionaryId';

IF @fkName IS NOT NULL AND @deleteAction <> N'CASCADE'
BEGIN
    DECLARE @dropSql NVARCHAR(400) = N'ALTER TABLE dbo.Words DROP CONSTRAINT ' + QUOTENAME(@fkName);
    EXEC sp_executesql @dropSql;
    SET @fkName = NULL;
END;

IF @fkName IS NULL
BEGIN
    ALTER TABLE dbo.Words
        ADD CONSTRAINT FK_Words_Dictionaries
            FOREIGN KEY (DictionaryId) REFERENCES dbo.Dictionaries(ID) ON DELETE CASCADE;
END;

COMMIT TRANSACTION;
